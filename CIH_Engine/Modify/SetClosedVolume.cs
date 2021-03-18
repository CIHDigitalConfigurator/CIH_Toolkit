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

namespace BH.Engine.CIH
{
    public static partial class Modify
    {
        [Description("Modify the IsInZone condition to update its `ClosedVolumes` property: add in all the input zones' ClosedVolumes with the same ZoneName.")]
        public static void SetClosedVolume(IsInZone isInZoneCondition, List<Zone> zones)
        {
            isInZoneCondition.ClosedVolumes.AddRange(
                zones
                    .Where(z => z.ZoneName == isInZoneCondition.ZoneName) // Make sure only Zones with the same ZoneName as the condition are used.
                    .Select(z => z.ClosedVolume)
                );
        }
    }
}
