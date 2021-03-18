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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.Data.Conditions
{
    public class IsSmallerThanOrEqualTo : BaseCondition, IPropertyCondition, IComparisonCondition
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Source of the value to be extracted from the objects that will be subject to the condition." +
            "\nValid inputs: " +
            "\n\t- Name of the property (e.g. BHoM_Guid)" +
            "\n\t- Name of any sub-property, using dot separators (e.g. Bar.SectionProperty.Area)" +
            "\n\t- Name of any queriable derived property (e.g. Length)" +
            "\n\t- For a value stored in a BHoMObject's CustomData, enter `CustomData[keyName]`" +
            "\n\t- For a value stored in a Fragment's property, enter `FragmentTypeName.PropertyName`.")]
        public virtual string PropertyName { get; set; }

        [Description("Reference Value that the property value should be compared to." +
            "\nIt can be a number, or a DateTime (e.g. ± 1 day), or anything comparable.")]
        public virtual object ReferenceValue { get; set; }

        [Description("If applicable, tolerance to be considered in the comparison." +
            "\nIt can be a number, or a DateTime (e.g. ± 1 day), or anything comparable with the property value.")]
        public virtual object Tolerance { get; set; } = null;


        /***************************************************/
        /**** Implicit Casting                          ****/
        /***************************************************/

        public static implicit operator ValueCondition(IsSmallerThanOrEqualTo condition)
        {
            return new ValueCondition()
            {
                Comparison = ValueComparisons.LessThanOrEqualTo,
                PropertyName = condition.PropertyName,
                Clause = condition.Clause,
                Comment = condition.Comment,
                Name = condition.Name,
                ReferenceValue = condition.ReferenceValue,
                Source = condition.Source,
                Tolerance = condition.Tolerance
            };
        }

        /***************************************************/
        /**** ToString                                  ****/
        /***************************************************/

        public override string ToString()
        {
            return ((ValueCondition)this).ToString();
        }
    }
}


