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
        public static SpecificationResult ApplySpecification(List<object> objects, Specification specification)
        {
            SpecificationResult specRes = new SpecificationResult();

            // First apply filter to get relevant objects
            ConditionResult filterResult = ApplyConditions(objects, specification.FilterConditions);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = ApplyConditions(filterResult.PassedObjects, specification.CheckConditions);

            // Populate the result.
            specRes.PassedObjects.AddRange(checkResult.PassedObjects);
            specRes.FailedObjects.AddRange(checkResult.FailedObjects);
            specRes.NotAssessedObjects.AddRange(filterResult.FailedObjects);

            specRes.Specifications = new List<Specification>() { specification };
            specRes.ObjectFailures = new List<ObjectFailures>();

            for (int i = 0; i < specRes.FailedObjects.Count(); i++)
            {
                ObjectFailures f = new ObjectFailures();
                f.Object = specRes.FailedObjects.ElementAtOrDefault(i);
                f.FailedSpecifications = new HashSet<Specification>() { specification };
                f.FailInfo = new List<SpecificationFailure>() { new SpecificationFailure() { ParentSpecification = specification, FailedCheckCondition = checkResult.Condition, FailInfo = new List<object>() { checkResult.FailInfo } } };
                specRes.ObjectFailures.Add(f);
            }

            return specRes;
        }

        public static SpecificationResult ApplySpecifications(List<object> objects, List<Specification> specifications)
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
                SpecificationResult specRes = ApplySpecification(objects, spec);
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
                        f.FailedSpecifications = new HashSet<Specification>();
                        f.FailInfo = new List<SpecificationFailure>();
                    }

                    f.FailedSpecifications.Add(spec);
                    var singleSpecFailure = specRes.ObjectFailures.First();
                    f.FailInfo.Add(new SpecificationFailure() { ParentSpecification = spec, FailedCheckCondition = singleSpecFailure.FailInfo.First().FailedCheckCondition, FailInfo = singleSpecFailure.FailInfo.First().FailInfo });

                    objFailDict[obj] = f;
                }
            }

            NotAssessed = (allObjs.Except(failedObjs)).Except(passedObjs).ToList();

            combinedResult = new SpecificationResult()
            {
                PassedObjects = passedObjs,
                FailedObjects = failedObjs.ToList(),
                NotAssessedObjects = NotAssessed.ToList(),
                Specifications = specifications,
                ObjectFailures = objFailDict.Values.ToList()
            };

            return combinedResult;
        }
    }
}
