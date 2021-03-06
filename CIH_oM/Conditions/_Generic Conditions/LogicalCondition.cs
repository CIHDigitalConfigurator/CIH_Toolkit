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

using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.CIH.Conditions
{
    [Description("Condition that is made by the combination of several conditions. Each condition is combined with a Boolean Operator to the others." +
        "E.g. If AND is used, then the LogicalCondition will be considering a pass only if the object satisfies all the given Conditions.")]
    public class LogicalCondition : BaseCondition
    {
        /***************************************************/
        /****                Properties                 ****/
        /***************************************************/

        public virtual List<ICondition> Conditions { get; set; } = new List<ICondition>();

        public virtual BooleanOperator BooleanOperator { get; set; }

        /***************************************************/

        public override string ToString()
        {
            return $"{string.Join($",\n\t - {BooleanOperator} ", Conditions.Select(c => c?.ToString()))}".Replace("-  -", "-");
        }
    }
}

