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
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult VerifySpecification(List<object> objects, LogicalSpecification logicalSpec)
        {
            return VerifySpecifications(objects, logicalSpec.Specifications, logicalSpec.PassRequirement);
        }
    }
}
