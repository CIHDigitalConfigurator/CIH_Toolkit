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
            SpecificationResult result = new SpecificationResult();

            // First apply filter to get relevant objects
            ConditionResult filterResult = ApplyConditions(objects, specification.FilterConditions);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = ApplyConditions(filterResult.PassedObjects, specification.CheckConditions);

            // Populate the result.
            result.PassedObjects.AddRange(checkResult.PassedObjects);
            result.FailedObjects.AddRange(checkResult.FailedObjects);
            result.NotAssessedObjects.AddRange(filterResult.FailedObjects);

            result.Specification = specification;

            return result;
        }

        public static CombinedSpecificationsResult ApplySpecifications(List<object> objects, List<Specification> specifications)
        {
            // Find unique objects.
            HashSet<object> allObjs = new HashSet<object>(objects); // TODO: evaluate whether to use our HashComparer.
            objects = allObjs.ToList();

            List<object> passedObjs = new List<object>();
            HashSet<object> failedObjs = new HashSet<object>();
            HashSet<object> NotAssessed = new HashSet<object>();
            Dictionary<object, HashSet<ISpecification>> Failures = new Dictionary<object, HashSet<ISpecification>>();

            foreach (var spec in specifications)
            {
                SpecificationResult specRes = ApplySpecification(objects, spec);

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
                    AddFailure(obj, spec, ref Failures);
                }
            }

            passedObjs = (allObjs.Except(failedObjs)).Except(NotAssessed).ToList();

            CombinedSpecificationsResult combinedRes = new CombinedSpecificationsResult()
            {
                PassedObjects = passedObjs,
                FailedObjects = failedObjs.ToList(),
                NotAssessedObjects = NotAssessed.ToList(),
                Failures = Failures.Select(f => new oM.Data.Specifications.Failures() { Object = f.Key, FailedSpecifications = f.Value.ToList() }).ToList(),
                Specifications = specifications
            };

            return combinedRes;
        }

        private static void AddFailure(object obj, ISpecification specification, ref Dictionary<object, HashSet<ISpecification>> Failures)
        {
            if (obj == null || specification == null)
                return;

            HashSet<ISpecification> failedSpecs = new HashSet<ISpecification>();
            Failures.TryGetValue(obj, out failedSpecs);
            if (failedSpecs == null)
                failedSpecs = new HashSet<ISpecification>() { specification };
            else
                failedSpecs.Add(specification);

            Failures[obj] = failedSpecs;
        }
    }
}
