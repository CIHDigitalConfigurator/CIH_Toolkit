using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.CIH;
using System.ComponentModel;
using BH.oM.Data.Specifications;

namespace BH.Engine.CIH
{
    public static partial class Modify
    {
        [Description("For each given specification, update any of its 'IsInZone' Conditions," +
             "with the `ClosedVolumes` computed from the input ZoneReferenceElement and ZoneDimensions (matched by ZoneName).")]
        public static void ApplyToZone(List<Specification> specifications, List<IObject> zoneReferenceElements, List<ZoneDimensions> zoneDimensions)
        {
            // Create all the zones, matching the zoneReferenceElements and zoneDimensions with the same ZoneName.
            List<Zone> zones = Create.Zones(zoneReferenceElements, zoneDimensions);

            // Group zones by zoneName.
            Dictionary<string, List<Zone>> zonesByName = zones.GroupBy(z => z.ZoneName).ToDictionary(g => g.Key, g => g.ToList());

            for (int i = 0; i < specifications.Count; i++)
            {
                Specification spec = specifications[i];

                // If the spec is null or has no `IsInZone` condition in either the FilterConditions or CheckCondition, skip it.
                if (spec == null || !spec.IsZoneSpec())
                    continue;

                bool success = true;
                success &= SetClosedVolume(spec.FilterConditions, zonesByName);
                success &= SetClosedVolume(spec.CheckConditions, zonesByName);

                if (!success)
                    BH.Engine.Reflection.Compute.RecordWarning($"Could not create the Zone volumes for Specification {spec.Clause} of name `{spec.SpecName}`.");

            }
        }

        [Description("For each given specification, update any of its 'IsInZone' Conditions," +
            "with the `ClosedVolumes` property extracted from the input Zones (matched by ZoneName).")]
        public static List<bool> ApplyToZone(List<Specification> specifications, List<Zone> zones)
        {
            return specifications.Select(spec => ApplyToZone(spec, zones)).ToList();
        }

        private static bool ApplyToZone(Specification specification, List<Zone> zones)
        {
            bool success = true;
            Dictionary<string, List<Zone>> zonesByName = zones.GroupBy(z => z.ZoneName).ToDictionary(g => g.Key, g => g.ToList());

            success &= SetClosedVolume(specification.FilterConditions, zonesByName);
            success &= SetClosedVolume(specification.CheckConditions, zonesByName);

            if (!success)
                BH.Engine.Reflection.Compute.RecordWarning($"Could not create the Zone volumes for Specification {specification.Clause} of name `{specification.SpecName}`.");

            return success;
        }

        [Description("Update the given IsInZone condition, by adding the input Zones' ClosedVolumes (matched by ZoneName).")]
        private static bool SetClosedVolume(List<ICondition> conditions, Dictionary<string, List<Zone>> zonesByName)
        {
            bool success = true;

            for (int i = 0; i < conditions.Count(); i++)
            {
                IsInZone isInZoneCondition = conditions[i] as IsInZone;
                if (isInZoneCondition == null)
                    continue;

                // Select only zones with the same name of the current Condition ("pertainingZones")
                List<Zone> pertainingZones = new List<Zone>();
                if (!zonesByName.TryGetValue(isInZoneCondition.ZoneName, out pertainingZones))
                {
                    BH.Engine.Reflection.Compute.RecordWarning($"A {nameof(IsInZone)} condition specified a {nameof(IsInZone.ZoneName)} `{isInZoneCondition.ZoneName}` that could not be found among any of the Reference Elements' target zones.");
                    success = false;
                    continue;
                }

                // Add the closed volumes of the pertainingZones to the current Condition.
                if (isInZoneCondition.ClosedVolumes == null)
                    isInZoneCondition.ClosedVolumes = new List<oM.Geometry.IGeometry>();

                isInZoneCondition.ClosedVolumes.AddRange(pertainingZones.Select(z => z.ClosedVolume));

                // Update the input list.
                conditions[i] = isInZoneCondition;
            }

            return success;
        }
    }
}
