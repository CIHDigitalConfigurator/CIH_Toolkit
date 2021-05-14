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
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Data
{
    public class ConditionResult : IObject
    {
        [Description("The condition that was evaluated.")]
        public virtual ICondition Condition { get; set; }
        [Description("The objects that passed the condition.")]
        public virtual List<object> PassedObjects { get; set; } = new List<object>();
        [Description("The objects that failed the condition.")]
        public virtual List<object> FailedObjects { get; set; } = new List<object>();
        [Description("One info text per each failed object, describing how it failed.")]
        public virtual List<string> FailInfo { get; set; } = new List<string>();
        [Description("A pattern of booleans (true/false) that describes if each object in the original list passed or failed.")]
        public virtual List<bool> Pattern { get; set; } = new List<bool>();
    }
}