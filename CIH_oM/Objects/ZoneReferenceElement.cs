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
using System.Threading.Tasks;

namespace BH.oM.CIH
{
    /*
     * The ZoneDimensions object pairs Reference Geometry (e.g. lines) with the name of a Zone.
     * This is used to define ZoneConditions and Zone Specifications.
     * In the reference implementation, scripts that reference to this object this include:
     *  - Module 3 '03-SA05_ReferenceElements-Structure' outputs Zone Reference Elements for the Structural parts of the building. 
     *  - Module 4 '04-SA05_ZoneGeneration-Structure' takes the Reference Elements, 
     *    the Zone Dimensions and the (partially-defined) Zone Specifications and returns "applied" Zone Specifications, 
     *    where the ClosedVolume is calculated based on Reference Elements and Zone Dimensions.
    */

    [Description("Defines geometrical objects tagged with the name of a Zone. " +
        "\nThese reference elements can be used with partially-defined Zones. Zone conditions and Zone Specifications using partially defined Zones" +
        "can become fully defined ('applied') by using the method `Modify.ApplyToZone`.")]
    public class ZoneReferenceElement : IObject
    {
        public virtual IGeometry ReferenceGeometry { get; set; }

        public string ZoneName { get; set; }
    }
}
