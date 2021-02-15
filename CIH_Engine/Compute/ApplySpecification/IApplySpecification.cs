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

        public static SpecificationResult IApplySpecification(List<object> objects, ISpecification specification)
        {
            return ApplySpecification(objects, specification as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult ApplySpecification(List<object> objects, Specification specification)
        {

            // First apply filter to get relevant objects
            ConditionResult filterResult = ApplyConditions(objects, specification.FilterConditions);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = ApplyConditions(filterResult.PassedObjects, specification.CheckConditions);

            return PopulateSpecificationResult(specification, filterResult, checkResult);
        }

        /***************************************************/

        // Fallback
        private static SpecificationResult ApplySpecification(List<object> objects, ISpecification specification)
        {
            BH.Engine.Reflection.Compute.RecordError($"No method found to apply specification of type {specification.GetType()}.");
            return new SpecificationResult();
        }
    }
}
