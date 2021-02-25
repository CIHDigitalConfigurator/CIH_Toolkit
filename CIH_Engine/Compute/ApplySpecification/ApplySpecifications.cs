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

        public static SpecificationResult ApplySpecifications(List<object> objects, List<ISpecification> specifications)
        {
            SpecificationResult combinedResult = new SpecificationResult();

            // Find unique objects.
            HashSet<object> allObjs = new HashSet<object>(objects); // TODO: evaluate whether to use our HashComparer.
            objects = allObjs.ToList();

            List<object> passedObjs = new List<object>();
            List<object> failedObjs = new List<object>();
            List<object> NotAssessed = new List<object>();
            Dictionary<object, ObjectFailures> objFailDict = new Dictionary<object, ObjectFailures>();  // one "Failures" per failed object, stating what specification(s) that object failed.

            foreach (var spec in specifications)
            {
                SpecificationResult specRes = IApplySpecification(objects, spec);
                SpecificationFailure failuresForThisSpec = new SpecificationFailure();

                foreach (var obj in specRes.PassedObjects)
                {
                    NotAssessed.Remove(obj);
                    passedObjs.Add(obj);
                }

                foreach (var obj in specRes.FailedObjects)
                {
                    NotAssessed.Remove(obj);
                    passedObjs.Remove(obj);
                    failedObjs.Add(obj);

                    ObjectFailures f;
                    if (!objFailDict.TryGetValue(obj, out f))
                    {
                        f = new ObjectFailures();
                        f.Object = obj;
                        f.FailedSpecifications = new HashSet<ISpecification>();
                        f.FailInfo = new List<SpecificationFailure>();
                    }

                    f.FailedSpecifications.Add(spec);
                    var singleSpecFailure = specRes.ObjectFailures.First();
                    f.FailInfo.Add(new SpecificationFailure() { ParentSpecification = spec, FailedCheckCondition = singleSpecFailure.FailInfo.First().FailedCheckCondition, FailInfo = singleSpecFailure.FailInfo.FirstOrDefault()?.FailInfo });

                    objFailDict[obj] = f;
                }
            }

            var passedSomething = failedObjs.Except(passedObjs).ToList();
            objFailDict.Remove(passedSomething);

            NotAssessed = (allObjs.Except(failedObjs)).Except(passedObjs).ToList();
            failedObjs = passedSomething;

            combinedResult = new SpecificationResult()
            {
                PassedObjects = passedObjs,
                FailedObjects = failedObjs.ToList(),
                NotAssessedObjects = NotAssessed.ToList(),
                Specifications = specifications.Distinct().ToList(),
                ObjectFailures = objFailDict.Values.ToList()
            };

            return combinedResult;
        }
    }
}
