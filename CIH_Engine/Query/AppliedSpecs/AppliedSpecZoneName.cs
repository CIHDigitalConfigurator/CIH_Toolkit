using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using System.ComponentModel;
using System.Collections;
using BH.oM.Data.Specifications;
using BH.oM.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        [Description("Returns the ZoneName(s) associated with this Applied Zone Specification.")]
        public static List<string> AppliedSpecZoneName(this Specification spec)
        {
            if (spec.IsAppliedZoneSpec())
                return spec.FilterConditions.OfType<IsInZone>().Select(c => c.ZoneName).ToList();

            return null;
        }
    }
}
