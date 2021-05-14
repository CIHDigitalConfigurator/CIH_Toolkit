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
        public static IGeometry GeometryAppliedSpec(this Specification spec)
        {
            if (spec.IsAppliedZoneSpec())
                return new CompositeGeometry() { Elements = spec.FilterConditions.OfType<IsInZone>().SelectMany(c => c.ClosedVolumes).ToList() };

            return null;
        }
    }
}
