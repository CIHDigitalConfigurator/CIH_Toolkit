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
    /*
     * The Zone object pairs a Closed Volume in space with the name of a Zone.
     * This is used to define ZoneConditions and Zone Specifications.
     * In the reference implementation, scripts that reference to this object this include:
     *  - The specification definition spreadsheet `SA05-StructuralFrameSpecifications` defines several Zone Conditions. 
     *  - Module 4 '04-SA05_ZoneGeneration-Structure' takes the Reference Elements, 
     *    the Zone Dimensions and the (partially-defined) Zone Specifications and returns "applied" Zone Specifications, 
     *    where the ClosedVolume is calculated based on Reference Elements and Zone Dimensions.
    */

    [Description("Defines a Zone, that is a closed volume in space, tagged with some ZoneName.")]
    public class Zone : IObject
    {
        [Description("Identifier of the Zone.")]
        public virtual string ZoneName { get; set; }

        [Description("A BoundingBox or Cuboid (this will later support any kind of closed volume object).")]
        public virtual IGeometry ClosedVolume { get; set; }

        public override string ToString()
        {
            return $"Zone of name {ZoneName} and closed volume identified by a {ClosedVolume.GetType().Name}.";
        }
    }
}
