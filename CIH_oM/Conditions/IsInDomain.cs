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
using BH.oM.Data.Collections;
using BH.oM.CIH.Conditions;
using BH.oM.Data.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.CIH.Conditions
{
    [Description("Identifies a Condition that verifies if a Property of the object is within a certain domain (range).")]
    public class IsInDomain : BaseCondition, IPropertyCondition
    {
        [Description("Source of the value to be extracted from the objects that will be subject to the condition.")]
        public string PropertyName { get; set; }

        [Description("Reference Value that the property value should be compared to.")]
        public virtual Domain Domain { get; set; }

        [Description("If applicable, tolerance to be considered in the comparison.")]
        public virtual double Tolerance { get; set; }

        public override string ToString()
        {
            return Domain == null ? "" : $"{PropertyName} included between {Domain.Min} and {Domain.Max}";
        }
    }
}


