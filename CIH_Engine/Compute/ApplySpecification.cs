using BH.oM.Base;
using BH.oM.Data;
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
    }
}
