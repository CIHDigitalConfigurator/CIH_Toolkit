﻿/*
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

using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.Engine.Diffing;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, ValueNullCondition valueNullCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = valueNullCondition };
            List<string> info = new List<string>();

            var requiredState = valueNullCondition.NullCondition;

            foreach (var obj in objects)
            {
                bool passed = false;
                object value = obj.ValueFromSource(valueNullCondition.PropertyName);

                if (requiredState == ValueNullConditions.MustBeNull)
                    passed = value == null;
                else if (requiredState == ValueNullConditions.MustBeNotNull)
                    passed = value != null;

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"{valueNullCondition.PropertyName} was {value ?? "empty"}, which does not respect '{valueNullCondition.ToString()}'.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }
    }
}
