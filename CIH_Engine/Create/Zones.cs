/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.oM.Data.Specifications;

using System.ComponentModel;
using BH.oM.Data.Collections;
using BH.oM.Data.Library;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Create
    {
        public static List<Zone> Zones(List<IObject> referenceElements, List<ZoneDimensions> zoneDimensions)
        {
            List<Zone> zones = new List<Zone>();

            var zone_refElements = referenceElements
                .Select(refEl => refEl.ToZoneReferenceElement())
                .Where(zRefEl => zRefEl != null)
                .GroupBy(zRefEl => zRefEl.ZoneName)
                .ToDictionary(g => g.Key, g => g.ToList());

            var zone_zoneDims = zoneDimensions
                .GroupBy(zDim => zDim.ZoneName)
                .ToDictionary(g => g.Key, g => g.ToList());

            var commonZones = zone_refElements.Keys.Intersect(zone_zoneDims.Keys);

            foreach (string zoneName in commonZones)
            {
                if(zone_zoneDims[zoneName].Count() > 1)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Cannot create {zoneName}: more than 1 `{nameof(ZoneDimensions)}` objects specified for it.");
                    continue;
                }

                ZoneDimensions zoneDims = zone_zoneDims[zoneName].FirstOrDefault();
                if (zoneDims == null)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Cannot create {zoneName}: no valid `{nameof(ZoneDimensions)}` objects found for it.");
                    continue;
                }

                List<IObject> refElems = zone_refElements[zoneName].OfType<IObject>().ToList();

                zones.AddRange(Create.Zones(refElems, zoneDims));
            }

            return zones;
        }

        [Description("Create a list of zones from the given ReferenceElements and Dimensions.")]
        public static List<Zone> Zones(List<IObject> referenceElements, ZoneDimensions zoneDimensions)
        {
            List<Zone> zones = new List<Zone>();

            foreach (var refElement in referenceElements)
            {
                ZoneReferenceElement zoneRefEl = refElement.ToZoneReferenceElement();

                if (zoneRefEl.ZoneName != zoneDimensions.ZoneName)
                    continue;

                BoundingBox bb = Query.IElementBoundingBox(zoneRefEl.ReferenceGeometry, zoneDimensions.Width, zoneDimensions.Height, zoneDimensions.Depth);

                if (bb != null)
                {
                    Zone zone = new Zone() { ZoneName = zoneRefEl.ZoneName, ClosedVolume = bb };
                    zones.Add(zone);
                }
            }

            return zones;
        }

    }
}
