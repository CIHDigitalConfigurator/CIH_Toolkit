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

using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, DomainCondition domainCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = domainCondition };
            List<string> failInfo = new List<string>();

            foreach (var obj in objects)
            {
                bool passed = false;

                object value = obj.ValueFromSource(domainCondition.PropertyName);
                double numericalValue;
                double tolerance;
                double.TryParse(domainCondition.Tolerance.ToString(), out tolerance);

                if (double.TryParse(value?.ToString(), out numericalValue))
                    passed = NumberInDomain(numericalValue, domainCondition.Domain, tolerance);
                else if (obj is DateTime)
                {
                    DateTime? dt = obj as DateTime?;
                    passed = NumberInDomain(dt.Value.Ticks, domainCondition.Domain, tolerance);
                }

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    failInfo.Add($"\"{domainCondition.ToString()}\"\nnot respected: value is {value}.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = failInfo;
            return result;
        }

        public static bool NumberInDomain(double number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            if (true) // TODO: implement extremes.
                return domain.Min <= number + tolerance && number - tolerance <= domain.Max;
        }

        public static bool NumberInDomain(long number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            if (true) // TODO: implement domain extremes.
                return domain.Min <= number + tolerance && number - tolerance <= domain.Max;
        }
    }
}
