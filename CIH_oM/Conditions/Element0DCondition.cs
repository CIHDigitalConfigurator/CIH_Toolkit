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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Dimensional;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Data.Conditions
{
    public class Element0DCondition : BaseCondition, ISpatialCondition
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Point ReferencePoint { get; set; }

        public double LocalXDimension { get; set; }
        public double LocalYDimension { get; set; }
        public double LocalZDimension { get; set; }

        [Description("Describes what kind of rule should be applied to evaluate whether the BoundingBox actually contains a BHoMObject." +
            "By default, it checks the inclusion of the BHoMObject's `Geometry`.")]
        public ContainmentRules ContainmentRule { get; set; } = ContainmentRules.ContainsGeometry;

        /***************************************************/

        public override string ToString()
        {
            return $"Must be in a Box of dimensions {LocalXDimension}x{LocalYDimension}x{LocalZDimension} centered in ({ReferencePoint.X},{ReferencePoint.Y},{ReferencePoint.Z}).";
        }
    }
}


