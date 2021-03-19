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
using BH.oM.CIH.Conditions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.Data.Specifications
{
    public class Specification : ISpecification
    {
        [Description("Unique identifier to reference the Condition within a set.")]
        public virtual string Clause { get; set; }

        [Description("Specification Name.")]
        public virtual string SpecName { get; set; }
        public virtual string Description { get; set; }

        public virtual List<ICondition> FilterConditions { get; set; } // TODO: switch to single condition instead of list. Multiple conditions can be done in one logical condition.
        public virtual List<ICondition> CheckConditions { get; set; } // TODO: switch to single condition instead of list. Multiple conditions can be done in one logical condition.

        public override string ToString()
        {
            return $"{(string.IsNullOrWhiteSpace(SpecName) ? "This Specification" : $"`{SpecName}`")} requires objects that respect the following conditions:\n\t - {string.Join(",\n\t - ", FilterConditions.Select(c => c?.ToString()))}\n" +
                $"to comply with the following conditions:\n\t{string.Join(",\n\t - ", CheckConditions.Select(c => c?.ToString()))}";
        }
    }
}


