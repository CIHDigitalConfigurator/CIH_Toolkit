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
using System.ComponentModel;
using BH.oM.CIH;

namespace BH.oM.Data.Conditions
{
    [Description("Checks if a object is in a Zone.")]
    public class IsInZone : BaseCondition, ISpatialCondition
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ZoneName { get; set; }

        [Description("Closed volumes associated with the ZoneName and its Reference Elements.")]
        public List<IGeometry> ClosedVolumes { get; set; }

        [Description("Describes what kind of rule should be applied to evaluate whether the Zone actually contains the object." +
            "By default, it checks the inclusion of the BHoM's `Geometry`.")]
        public ContainmentRules ContainmentRule { get; set; } = ContainmentRules.Geometry;

        /***************************************************/

        public override string ToString()
        {
            return $"{ContainmentRule} of the object must be within Zone of name `{ZoneName}`";
        }
    }
}


