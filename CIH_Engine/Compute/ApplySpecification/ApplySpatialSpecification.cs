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
        public static SpecificationResult ApplySpecification(List<object> objects, ISpatialSpecification specification)
        {
            SpecificationResult specRes = new SpecificationResult();
            ICondition checkCondition = null;

            // First apply filter to get relevant objects
            BoundingBoxCondition bbc = null;

            Element0DSpecification spec0D = specification as Element0DSpecification;
            if (spec0D != null)
            {
                bbc = new BoundingBoxCondition() { BoundingBox = Query.GetBoundingBox(spec0D.ReferencePoint, spec0D.LocalXDimension, spec0D.LocalYDimension, spec0D.LocalZDimension) };
                checkCondition = spec0D.Condition;
            }

            Element1DSpecification spec1D = specification as Element1DSpecification;
            if (spec1D != null)
            {
                bbc = new BoundingBoxCondition() { BoundingBox = Query.GetBoundingBox(spec1D.ReferenceLine, spec1D.LocalYDimension, spec1D.LocalZDimension) };
                checkCondition = spec1D.Condition;
            }

            Element2DSpecification spec2D = specification as Element2DSpecification;
            if (spec2D != null)
            {
                bbc = new BoundingBoxCondition() { BoundingBox = Query.GetBoundingBox(spec2D.ReferenceElement, spec2D.LocalZDimension) };
                checkCondition = spec2D.Condition;
            }

            ConditionResult filterResult = IApplyCondition(objects, bbc);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = IApplyCondition(filterResult.PassedObjects, checkCondition);

            return PopulateSpecificationResult(specification, filterResult, checkResult);
        }


        public static SpecificationResult ApplySpecification(List<object> objects, Element2DSpecification spec)
        {
            BoundingBoxCondition bbc = new BoundingBoxCondition() { BoundingBox = Query.GetBoundingBox(spec.ReferenceElement, spec.LocalZDimension) };

            // First apply filter to get relevant objects
            ConditionResult filterResult = IApplyCondition(objects, bbc);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = IApplyCondition(filterResult.PassedObjects, spec.Condition);

            return PopulateSpecificationResult(spec, filterResult, checkResult);
        }
    }
}
