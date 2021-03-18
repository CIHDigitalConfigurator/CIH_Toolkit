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
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BH.oM.CIH
{
    [Description("Defines the dimensions of a Zone. These dimensions will be used together with a Reference Element that targets the same ZoneName to compute a Closed Volume for the Zone." +
        "The dimensions (width/height/depth) here defined are currently assuming that the closed volume will be located below the Reference Element." +
        "E.g. a ZoneDimensions for a Beam Zone can specify Width and Height only; Width and Height will be used, together with a 1D reference element (a line tagged with the same ZoneName) to compute a cuboid geometry placed below the reference Line.")]
    public class ZoneDimensions : IObject
    {
        [Description("Name of the Zone. This will be used to associate this ZoneDimensions to ZoneReferenceElements with the same ZoneName.")]
        public virtual string ZoneName { get; set; }

        [Description("Dimension of the Zone in the localY direction. E.g. if the reference Element is a horizontal Element1D, this is the horizontal direction of its section plane.")]
        public double Width { get; set; }
        [Description("Dimension of the Zone in the localZ direction. E.g. for a horizontal Element1D, this is the vertical direction of its section plane.")]
        public double Height { get; set; } = 0;
        [Description("Dimension of the Zone in the localX direction. For an Element1D, this is parallel to its axis.")]
        public double Depth { get; set; } = 0;

        public override string ToString()
        {
            string length = Height == 0 ? "" : "x" + Height.ToString();
            string depth = Depth == 0 ? "" : "x" + Depth.ToString();

            return $"ZoneDimensions for Zone named `{ZoneName}`: {Width}{length}{depth}";
        }
    }
}
