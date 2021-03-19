using BH.oM.Data.Conditions;
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
                if (spec == null || (!spec.FilterConditions.OfType<IsInZone>().Any() && !spec.CheckConditions.OfType<IsInZone>().Any()))
                    continue;

                SetClosedVolume(spec.FilterConditions, zonesByName);
                SetClosedVolume(spec.CheckConditions, zonesByName);
            }
        }

        [Description("For each given specification, update any of its 'IsInZone' Conditions," +
            "with the `ClosedVolumes` property extracted from the input Zones (matched by ZoneName).")]
        public static void ApplyToZone(List<Specification> specifications, List<Zone> zones)
        {
            specifications.ForEach(spec => ApplyToZone(spec, zones));
        }

        private static void ApplyToZone(Specification specification, List<Zone> zones)
        {
            Dictionary<string, List<Zone>> zonesByName = zones.GroupBy(z => z.ZoneName).ToDictionary(g => g.Key, g => g.ToList());

            SetClosedVolume(specification.FilterConditions, zonesByName);
            SetClosedVolume(specification.CheckConditions, zonesByName);
        }

        [Description("Update the given IsInZone condition, by adding the input Zones' ClosedVolumes (matched by ZoneName).")]
        private static void SetClosedVolume(List<ICondition> conditions, Dictionary<string, List<Zone>> zonesByName)
        {
            for (int i = 0; i < conditions.Count(); i++)
            {
                IsInZone isInZoneCondition = conditions[i] as IsInZone;
                if (isInZoneCondition == null)
                    continue;

                // Select only zones with the same name of the current Condition ("pertainingZones")
                List<Zone> pertainingZones = new List<Zone>();
                if (!zonesByName.TryGetValue(isInZoneCondition.ZoneName, out pertainingZones))
                    continue;

                // Add the closed volumes of the pertainingZones to the current Condition.
                isInZoneCondition.ClosedVolumes.AddRange(pertainingZones.Select(z => z.ClosedVolume));

                // Update the input list.
                conditions[i] = isInZoneCondition;
            }
        }
    }
}
