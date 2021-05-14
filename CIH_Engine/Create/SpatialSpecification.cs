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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.CIH
{
    public static partial class Create
    {
        [Description("Returns an 'Applied' Spatial Specification. This is a set of Zone Specifications applied to precise locations.")]
        [Input("referenceElements", "Objects indicating where the ZoneSpecifications should be applied. They must have a `zoneName` property that specifies in what Zone they're in.")]
        [Input("zoneSpecification", "Zone specification to be applied in the `locations`. It will be matched to the `locations` through the `ZoneName` property.")]
        [Input("prop", "The name of the property where the Zone Name is stored for the 'locations' objects. Defaults to `ZoneName`.")]
        public static SpatialSpecification SpatialSpecification(List<IObject> referenceElements, ZoneSpecification zoneSpecification, string prop = "ZoneName")
        {
            SpatialSpecification result = new SpatialSpecification();

            foreach (var loc in referenceElements)
            {
                string zoneName = Reflection.Query.PropertyValue(loc, "ZoneName") as string;

                if (zoneName == zoneSpecification.ZoneName)
                    result.Locations.Add(loc);
            }

            result.ZoneSpecification = zoneSpecification;

            return result;
        }

        [Description("Returns an 'Applied' Spatial Specification. This is a set of Zone Specifications applied to precise locations.")]
        [Input("referenceElements", "Objects indicating where the ZoneSpecifications should be applied. They must have a `zoneName` property that specifies in what Zone they're in.")]
        [Input("zoneSpecification", "Zone specification to be applied in the `locations`. It will be matched to the `locations` through the `ZoneName` property.")]
        [Input("prop", "The name of the property where the Zone Name is stored for the 'referenceElements' objects. Defaults to `ZoneName`.")]
        public static List<SpatialSpecification> SpatialSpecifications(List<IObject> referenceElements, List<ZoneSpecification> zoneSpecifications, string prop = "ZoneName")
        {
            List<SpatialSpecification> result = new List<SpatialSpecification>();

            var zoneLocations = new Dictionary<string, List<IObject>>();

            bool missingProp = false;
            foreach (var refElem in referenceElements)
            {
                string zoneName = refElem.ValueFromSource("ZoneName") as string;

                if (string.IsNullOrWhiteSpace(zoneName))
                {
                    missingProp = true;
                    continue;
                }

                if (!zoneLocations.ContainsKey(zoneName))
                    zoneLocations[zoneName] = new List<IObject>();

                zoneLocations[zoneName].Add(refElem);
            }

            if (missingProp)
                BH.Engine.Reflection.Compute.RecordError($"Some {nameof(referenceElements)} object did not have the required `{prop}` property in its CustomData.");

            Dictionary<string, ZoneSpecification> zoneDic = zoneSpecifications.GroupBy(z => z.ZoneName).ToDictionary(g => g.Key, g => g.FirstOrDefault());
            HashSet<string> zoneSpecsNotFound = new HashSet<string>();
            foreach (string zoneName in zoneLocations.Keys)
            {
                SpatialSpecification sp = new SpatialSpecification();
                sp.Locations = zoneLocations[zoneName];
                if (!zoneDic.ContainsKey(zoneName))
                {
                    zoneSpecsNotFound.Add(zoneName);
                    continue;
                }

                sp.ZoneSpecification = zoneDic[zoneName];

                result.Add(sp);
            }

            if (zoneSpecsNotFound.Count > 0)
            {
                foreach (var notFoundZoneName in zoneSpecsNotFound)
                {
                    BH.Engine.Reflection.Compute.RecordWarning($"No Zone specification was found for the Zone named `{notFoundZoneName}` that was required by some of the objects provided.");
                }
            }

            return result;
        }
    }
}
