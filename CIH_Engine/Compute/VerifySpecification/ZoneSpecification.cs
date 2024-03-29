﻿using BH.oM.Base;
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

        private static SpecificationResult VerifySpecification(List<object> objects, ZoneSpecification spatialSpec)
        {
            BH.Engine.Base.Compute.RecordError($"Zone Specifications cannot be applied directly: they have to be paired with a specific location. Read the description of the method `Create.{nameof(Create.SpatialSpecification)}`.");
            return null;
        }
    }
}
