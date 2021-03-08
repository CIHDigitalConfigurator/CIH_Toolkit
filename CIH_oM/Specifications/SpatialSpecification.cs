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

using BH.oM.Base;
using BH.oM.Data.Conditions;
using BH.oM.Data.Library;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.Data.Specifications
{
    [Description("Zone specification 'applied' to a certain location.")]
    public class SpatialSpecification : ISpecification
    {
        public virtual string SpecName { get; set; }
        public virtual string Description { get; set; }

        [Description("Objects owning a geometrical property that indicates the location where the Zone Specifications should be applied.")]
        public List<IObject> Locations { get; set; } = new List<IObject>(); // this could be a list of boxes.

        public ZoneSpecification ZoneSpecification { get; set; } // this could be a "normal specification"

        public override string ToString()
        {
            List<string> filterConditionsText = new List<string>();

            string zoneBoxDescription = $"{(ZoneSpecification.Width != 0 ? ZoneSpecification.Width.ToString() : "")}" +
                $"{(ZoneSpecification.Height != 0 ? "x" + ZoneSpecification.Height.ToString() : "")}" +
                $"{(ZoneSpecification.Depth != 0 ? "x" + ZoneSpecification.Depth.ToString() : "")}";

            filterConditionsText.Add($"the defining geometry of the object must be contained within the Zone");
            filterConditionsText.AddRange(ZoneSpecification.FilterConditions.Select(c => c?.ToString()));

            List<string> conditionsText = new List<string>();

            conditionsText.Add($"geometry3D of the object must be contained within the Zone");
            if (ZoneSpecification.CheckConditions != null)
                conditionsText.AddRange(ZoneSpecification.CheckConditions.Select(c => c?.ToString()));

            return $"{(string.IsNullOrWhiteSpace(SpecName) ? "This Spatial Specification" : $"`{SpecName}`")} defines a Zone `{ZoneSpecification.ZoneName}` made of box solids aligned to the reference elements and of size {zoneBoxDescription}. " +
                $"\nIt requires objects that respect the following conditions:" +
                $"\n\t - {string.Join(";\n\t - ", filterConditionsText)}" +
                $"\nto comply with the following conditions:\n\t - {string.Join(";\n\t - ", conditionsText)}";
        }
    }
}


