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
using BH.oM.Data;

namespace BH.oM.CIH.Conditions
{
    [Description("Condition that verifies if an Property of an object is of a certain Value (e.g. smaller, equal, or greater than).")]
    public class ValueCondition : BaseCondition, IValueCondition
    {
        [Description("Source of the value to be extracted from the objects that will be subject to the condition." +
            "\nValid inputs: " +
            "\n\t- Name of the property (e.g. BHoM_Guid)" +
            "\n\t- Name of any sub-property, using dot separators (e.g. Bar.SectionProperty.Area)" +
            "\n\t- Name of any queriable derived property (e.g. Length)" +
            "\n\t- For a value stored in a BHoMObject's CustomData, enter `CustomData[keyName]`" +
            "\n\t- For a value stored in a Fragment's property, enter `FragmentTypeName.PropertyName`." +
            "\n\t- No input (null): if the ReferenceValue is a System.Type, then check object's type equality.")]
        public virtual string PropertyName { get; set; }

        [Description("Reference Value that the property value should be compared to." +
            "\nIt can be a number, or a DateTime (e.g. � 1 day), or anything comparable.")]
        public virtual object ReferenceValue { get; set; }

        [Description("Whether the property value should be smaller, greater, etc. than the ReferenceValue.")]
        public virtual ValueComparisons Comparison { get; set; } = ValueComparisons.EqualTo;

        [Description("If applicable, tolerance to be considered in the comparison." +
            "\nIt can be a number, or a DateTime (e.g. � 1 day), or anything comparable with the property value.")]
        public virtual object Tolerance { get; set; } = null;

        public override string ToString()
        {
            string valueString = "";

            if (ReferenceValue == null)
                valueString = "null";
            else if (ReferenceValue.ToString().Contains("BH.oM") && !(ReferenceValue is Type))
                valueString = ReferenceValue.GetType().GetProperty("Name").GetValue(ReferenceValue) as string;

            valueString = string.IsNullOrWhiteSpace(valueString) ? ReferenceValue.ToString() : valueString;

            // Make the text in the ValueComparisons more readable (remove camelCase and add spaces)
            string comparisonText = Comparison.ToString();
            var builder = new System.Text.StringBuilder();
            foreach (char c in comparisonText)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(Char.ToLower(c));
            }
            comparisonText = builder.ToString();

            if(string.IsNullOrWhiteSpace(PropertyName) && ReferenceValue is System.Type)
                return $"must be of type `{(ReferenceValue as Type).Name}`";

            return $"{PropertyName} {comparisonText} {valueString}";
        }
    }
}


