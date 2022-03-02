using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.CIH.Conditions;
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

        // Engineer to Order workflow. Verify against specs.
        public static SpecificationResult IVerifySpecification(List<object> objects, ISpecification specification)
        {
            return VerifySpecification(objects, specification as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult VerifySpecification(List<object> objects, Specification specification)
        {
            // First apply filter to get relevant objects
            ConditionResult filterResult = VerifyConditions(objects, specification.FilterConditions);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = VerifyConditions(filterResult.PassedObjects, specification.CheckConditions);


            return PopulateSpecificationResult(specification, filterResult, checkResult);
        }

        /***************************************************/

        // Fallback
        private static SpecificationResult VerifySpecification(List<object> objects, ISpecification specification)
        {
            BH.Engine.Base.Compute.RecordError($"No method found to apply specification of type {specification.GetType()}.");
            return new SpecificationResult();
        }
    }
}
