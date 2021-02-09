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
        public static SpecificationResult ApplySpecification(List<object> objects, Element1DSpecification specification)
        {
            SpecificationResult specRes = new SpecificationResult();

            // First apply filter to get relevant objects
            BoundingBoxCondition bbc = new BoundingBoxCondition() { BoundingBox = Query.GetBoundingBox(specification.ReferenceLine, specification.LocalYDimension, specification.LocalZDimension) };
            ConditionResult filterResult = ApplyCondition(objects, bbc);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = ApplyCondition(filterResult.PassedObjects, specification.Condition);

            return PopulateSpecificationResult(specification, filterResult, checkResult);
        }
    }
}
