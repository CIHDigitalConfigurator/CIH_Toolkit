﻿using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.CIH;
using BH.Engine.Diffing;
using BH.Engine.Base.Objects;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult VerifyCondition(List<object> objects, ValueCondition valueCondition)
        {
            ConditionResult conditionResult = new ConditionResult() { Condition = valueCondition };

            var refValue = valueCondition.ReferenceValue;
            if (refValue == null)
                BH.Engine.Base.Compute.RecordNote($"A {nameof(ValueCondition)}'s {nameof(valueCondition.ReferenceValue)} was null. Make sure this is intended.\nTo check for null/not null, consider using a {nameof(IsNull)} instead.");

            foreach (var obj in objects)
            {
                bool passed = true;

                // Value to be compared to the referenceValue.
                object value = obj.ValueFromSource(valueCondition.PropertyName);

                // If no propertyName was assigned, assume we want to compare the entire object.
                if (string.IsNullOrWhiteSpace(valueCondition.PropertyName))
                    value = obj;

                // Basic cases (check for nullity)
                if (valueCondition.ReferenceValue == null && value == null)
                {
                    passed = true;
                    PopulateConditionResults(obj, value, valueCondition, conditionResult, passed);
                    continue;
                }
                if (valueCondition.ReferenceValue == null && value != null)
                {
                    passed = false;
                    PopulateConditionResults(obj, value, valueCondition, conditionResult, passed);
                    continue;

                }
                else if (valueCondition.ReferenceValue != null && value == null)
                {
                    if (valueCondition.PropertyName == "Type" && valueCondition.ReferenceValue is Type)
                        passed = obj.GetType() == (Type)valueCondition.ReferenceValue;
                    else
                        passed = false;


                    PopulateConditionResults(obj, value, valueCondition, conditionResult, passed);
                    continue;

                }

                // Try a numerical comparison.
                double numericalValue;
                if (double.TryParse(value?.ToString(), out numericalValue))
                {
                    double referenceNumValue;
                    double.TryParse(valueCondition.ReferenceValue?.ToString(), out referenceNumValue);

                    double numTolerance;
                    if (!double.TryParse(valueCondition.Tolerance?.ToString(), out numTolerance))
                        numTolerance = 1e-03;

                    passed = NumericalComparison(numericalValue, referenceNumValue, numTolerance, valueCondition.Comparison);
                    PopulateConditionResults(obj, value, valueCondition, conditionResult, passed);
                    continue;
                }

                // Consider some other way to compare objects.
                if (valueCondition.Comparison == ValueComparisons.EqualTo)
                {
                    // If the referenceValue is a Type, convert this ValueCondition to a IsOfType condition.
                    if (valueCondition.ReferenceValue is Type)
                    {
                        IsOfType typeCondition = new IsOfType() { Type = valueCondition.ReferenceValue as Type };
                        var TypeCondResult = VerifyCondition(new List<object>() { value }, typeCondition);
                        conditionResult.PassedObjects.Add(TypeCondResult.PassedObjects);
                        conditionResult.FailedObjects.Add(TypeCondResult.FailedObjects);
                        conditionResult.FailInfo.Add(TypeCondResult.FailInfo.FirstOrDefault());
                        conditionResult.Pattern.Add(TypeCondResult.Pattern.FirstOrDefault());
                        conditionResult.Condition = valueCondition;
                        continue;
                    }

                    passed = CompareEquality(value, valueCondition.ReferenceValue, valueCondition.Tolerance);
                    PopulateConditionResults(obj, value, valueCondition, conditionResult, passed);
                }
            }

            return conditionResult;
        }

        private static void PopulateConditionResults(object obj, object value, ValueCondition valueCondition, ConditionResult result, bool status)
        {
            result.Condition = valueCondition;
            result.Pattern.Add(status);

            if (status) // if it's passed.
            {
                result.PassedObjects.Add(obj);
                return;
            }

            result.FailedObjects.Add(obj);
            string valueString = value == null ? "null" : value.ToString();

            if (valueString.Contains("BH.oM") && !(valueCondition.ReferenceValue is Type))
                valueString = BH.Engine.Base.Query.PropertyValue(value, "Name") as string;

            string conditionText = valueCondition.ToString();
            conditionText = conditionText.Replace(valueCondition.PropertyName + " ", "");

            result.FailInfo.Add($"{(string.IsNullOrWhiteSpace(valueCondition.Clause) ? $"{nameof(ValueCondition)}" : valueCondition.Clause) + " failed: "}" +
                $"{valueCondition.PropertyName} must be {conditionText}, but is {valueString ?? "empty"}.");
        }

        private static bool CompareEquality(object value, object refValue, object tolerance)
        {
            var cc = tolerance as ComparisonConfig;
            if (cc != null)
            {
                //Compare by hash
                HashComparer<object> hc = new HashComparer<object>(cc);
                return hc.Equals(value, refValue);
            }

            // Try checking name compatibility. Useful for materials.
            string valueString = BH.Engine.CIH.Query.ValueFromSource(value, "Name") as string;
            string referenceValue = BH.Engine.CIH.Query.ValueFromSource(refValue, "Name") as string;
            if (string.IsNullOrWhiteSpace(referenceValue))
                referenceValue = refValue as string;

            if (valueString == referenceValue)
                return true;

            if (value.ToString().Contains("BH.oM") && (refValue is Type))
                return value.ToString() == refValue.ToString();

            if (value is string && refValue is string)
                return value.ToString() == refValue.ToString(); // this workaround is required. Not even Convert.ChangeType and dynamic type worked.

            return value == refValue;
        }

        private static bool NumericalComparison(double numericalValue, double referenceNumValue, double numTolerance, ValueComparisons comparison)
        {
            bool passed;
            switch (comparison)
            {
                case (ValueComparisons.EqualTo):
                    passed = referenceNumValue - numTolerance <= numericalValue && numericalValue <= referenceNumValue + numTolerance;
                    break;
                case (ValueComparisons.LessThan):
                    passed = numericalValue < referenceNumValue + numTolerance;
                    break;
                case (ValueComparisons.LessThanOrEqualTo):
                    passed = numericalValue <= referenceNumValue + numTolerance;
                    break;
                case (ValueComparisons.GreaterThanOrEqualTo):
                    passed = numericalValue >= referenceNumValue - numTolerance;
                    break;
                case (ValueComparisons.GreaterThan):
                    passed = numericalValue > referenceNumValue - numTolerance;
                    break;
                default:
                    passed = false;
                    break;
            }

            return passed;
        }
    }
}
