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
        [Description("An applied Zone Spec must have an IsInZone Condition in the FilterConditions.")]
        public static bool IsZoneSpec(this Specification spec)
        {
            return spec.FilterConditions.Any(c => c is IsInZone);
        }

        [Description("An applied Zone Spec must have a fully-defined IsInZone Condition in the FilterConditions (i.e. its ClosedVolumes property is populated).")]
        public static bool IsAppliedZoneSpec(this Specification spec)
        {
            return spec.FilterConditions.OfType<IsInZone>().Any(c => c.ClosedVolumes.Any());
        }
    }
}
