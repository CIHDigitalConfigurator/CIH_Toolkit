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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.Data.Specifications
{
    [Description("A specification that asks a certain condition to be satisfied only by objects placed in certain location (Zone) in space." +
        "The Zone acts both as a Filter condition (spec applies only to elements whose *centreline* in the Zone) and as a CheckCondition (verifies that the element's boundingbox is completely included in the zone).")]
    public class ZoneSpecification : Specification
    {
        [Description("Identifying Name of the Zone this specification is defined for.")]
        public string ZoneName { get; set; }

        [Description("Dimension of the Zone in the localY direction. For a horizontal beam, this is horizontally its section plane.")]
        public double Width { get; set; }
        [Description("Dimension of the Zone in the localZ direction. For a horizontal beam, this is vertically its section plane.")]
        public double Height { get; set; }
        [Description("Dimension of the Zone in the localX direction, that is, parallel to its axis.")]
        public double Depth { get; set; }

        // Prints only the CheckConditions.
        public override string ToString()
        {
            List<string> conditionsText = new List<string>();

            conditionsText.Add($"geometry3D of the object must be contained within the Zone aligned to the reference element and of size " +
                $"{(Width != 0 ? Width.ToString() : "")}" +
                $"{(Height != 0 ? "x"+ Height.ToString() : "")}" +
                $"{(Depth != 0 ? "x" + Depth.ToString() : "")}.");
            conditionsText.AddRange(CheckConditions.Select(c => c?.ToString()));

            return $"{(string.IsNullOrWhiteSpace(SpecName) ? "This Specification" : $"`{SpecName}`")} is defined for the zone `{ZoneName}`. \n" +
                $"It requires objects that respect the following conditions:\n\t - {string.Join(",\n\t - ", FilterConditions.Select(c => c?.ToString()))}\n" +
                $"to comply with the following conditions:\n\t{string.Join(",\n\t - ", conditionsText)}";
        }
    }
}


