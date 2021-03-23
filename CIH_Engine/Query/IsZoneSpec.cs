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
        public static bool IsAppliedZoneSpec(this Specification spec)
        {
            // An applied Zone Spec must have an IsInZone Condition in both the FilterConditions and CheckConditions.
            return spec.FilterConditions.Any(c => c is IsInZone) && spec.CheckConditions.Any(c => c is IsInZone);
        }

        public static bool IsWellFormedZoneSpec(this Specification spec)
        {
            if (!spec.IsAppliedZoneSpec())
                return false;

            // An applied Zone Spec must have the same ZoneName in the IsInZone Condition of both its FilterConditions and its CheckConditions.


            var zoneNames_FilterCond = spec.FilterConditions.Where(c => c is IsInZone).Select(c => ((IsInZone)c).ZoneName).Distinct();
            var zoneNames_CheckCond = spec.CheckConditions.Where(c => c is IsInZone).Select(c => ((IsInZone)c).ZoneName).Distinct();

            if (zoneNames_FilterCond.Count() > 1 || zoneNames_CheckCond.Count() > 1)
            {
                BH.Engine.Reflection.Compute.RecordError("Specifications that include an IsInZone condition must target only 1 zoneName.");
                return false;
            }

            if (zoneNames_FilterCond.Count() == 1 || zoneNames_CheckCond.Count() == 1)
            {
                if (zoneNames_FilterCond.First() != zoneNames_CheckCond.First())
                {
                    BH.Engine.Reflection.Compute.RecordError("The FilterConditions and CheckConditions target different ZoneNames.");
                    return false;
                }
            }

            return true;
        }


        public static string AppliedSpecZoneName(this Specification spec)
        {
            if (spec.IsAppliedZoneSpec() && spec.IsWellFormedZoneSpec())
                return spec.FilterConditions.OfType<IsInZone>().First().ZoneName;

            return null;
        }

        public static IGeometry GeometryAppliedSpec(this Specification spec)
        {
            if (spec.IsAppliedZoneSpec() && spec.IsWellFormedZoneSpec())
                return new CompositeGeometry() { Elements = spec.CheckConditions.OfType<IsInZone>().SelectMany(c => c.ClosedVolumes).ToList() };

            return null;
        }
    }
}
