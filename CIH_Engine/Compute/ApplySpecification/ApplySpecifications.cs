using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.Data.Conditions;
using BH.oM.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SpecificationResult ApplySpecifications(List<object> objects, List<ISpecification> specifications, PassRequirement passRequirement = PassRequirement.AllMustPass)
        {
            SpecificationResult combinedResult = new SpecificationResult();

            // Find unique objects.
            HashSet<object> allObjs = new HashSet<object>(objects); // TODO: evaluate whether to use our HashComparer.
            objects = allObjs.ToList();

            HashSet<object> passedObjs = new HashSet<object>();
            HashSet<object> failedObjs = new HashSet<object>();
            HashSet<object> NotAssessed = new HashSet<object>() { objects };
            Dictionary<object, ObjectFailures> objFailDict = new Dictionary<object, ObjectFailures>();  // one "Failures" per failed object, stating what specification(s) that object failed.

            foreach (var spec in specifications)
            {
                SpecificationResult specRes = IApplySpecification(objects, spec);
                CheckFailure failuresForThisSpec = new CheckFailure();

                foreach (var obj in specRes.PassedObjects)
                {
                    NotAssessed.Remove(obj);

                    if (passRequirement == PassRequirement.AtLeastOnePasses || (passRequirement == PassRequirement.AllMustPass && !failedObjs.Contains(obj)))
                    {
                        passedObjs.Add(obj);

                        if (failedObjs.Contains(obj))
                        {
                            failedObjs.Remove(obj);
                            objFailDict.Remove(obj);
                        }
                    }
                }

                foreach (var obj in specRes.FailedObjects)
                {
                    NotAssessed.Remove(obj);

                    if (passRequirement == PassRequirement.AtLeastOnePasses && passedObjs.Contains(obj))
                        continue; // continue considering the object as Passed.
                    
                    passedObjs.Remove(obj);
                    failedObjs.Add(obj);

                    ObjectFailures f;
                    if (!objFailDict.TryGetValue(obj, out f))
                    {
                        f = new ObjectFailures();
                        f.Object = obj;
                    }

                    f.FailedSpecifications.Add(spec);
                    var singleSpecFailure = specRes.ObjectFailures.First();

                    var existingCheckFailure = f.CheckFailures.FirstOrDefault(cf => cf.ParentSpecification == spec && (cf.FailedCheckCondition == singleSpecFailure.CheckFailures.First() || cf.FailInfo == singleSpecFailure.CheckFailures.First().FailInfo));
                    var checkFailure_thisSpec = f.CheckFailures.FirstOrDefault(cf => cf.ParentSpecification == spec);

                    bool areNotEqual = checkFailure_thisSpec != null;

                    string text1 = checkFailure_thisSpec?.FailInfo.ToString();
                    string text2 = singleSpecFailure?.CheckFailures.First().FailInfo.ToString();

                    if (!string.IsNullOrWhiteSpace(text1) && !string.IsNullOrWhiteSpace(text2)) {
                        string asdasd = string.Join("", text1.Except(text2));
                        areNotEqual &= !string.IsNullOrWhiteSpace(asdasd);
                    }
                    
                    if (!f.CheckFailures.Any() || checkFailure_thisSpec == null || areNotEqual)
                     f.CheckFailures.Add(new CheckFailure() { Object = obj, ParentSpecification = spec, FailedCheckCondition = singleSpecFailure.CheckFailures.First().FailedCheckCondition, FailInfo = singleSpecFailure.CheckFailures.FirstOrDefault()?.FailInfo });

                    objFailDict[obj] = f;
                }
            }

            combinedResult = new SpecificationResult()
            {
                PassedObjects = passedObjs.ToList(),
                FailedObjects = failedObjs.ToList(),
                NotAssessedObjects = NotAssessed.ToList(),
                Specifications = specifications.Distinct().ToList(),
                ObjectFailures = objFailDict.Values.ToList()
            };

            return combinedResult;
        }
    }
}
