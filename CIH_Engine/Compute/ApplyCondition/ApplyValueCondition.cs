using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.Engine.Diffing;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, ValueCondition valueCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = valueCondition };

            var refValue = valueCondition.ReferenceValue;
            if (refValue == null)
                BH.Engine.Reflection.Compute.RecordNote($"A {nameof(ValueCondition)}'s {nameof(valueCondition.ReferenceValue)} was null. Make sure this is intended.\nTo check for null/not null, consider using a {nameof(ValueNullCondition)} instead.");


            foreach (var obj in objects)
            {
                bool passed = true;

                object value = obj.ValueFromSource(valueCondition.PropertyName);

                if (valueCondition.ReferenceValue == null && value == null)
                    passed = true;
                else if (valueCondition.ReferenceValue == null && value != null)
                    passed = false;
                else if (valueCondition.ReferenceValue != null && value == null)
                    if (valueCondition.Comparison == ValueComparisons.EqualTo && valueCondition.ReferenceValue is Type && valueCondition.PropertyName == null)
                    {
                        TypeCondition typeCondition = new TypeCondition() { Type = valueCondition.ReferenceValue as Type };
                        var TypeCondResult = ApplyCondition(new List<object>() { obj }, typeCondition);
                        result.PassedObjects.Add(TypeCondResult.PassedObjects);
                        result.FailedObjects.Add(TypeCondResult.FailedObjects);
                        result.FailInfo.Add(TypeCondResult.FailInfo.FirstOrDefault());
                        result.Pattern.Add(TypeCondResult.Pattern.FirstOrDefault());
                        result.Condition = valueCondition;
                    }
                    else
                        passed = false;
                else if (valueCondition.ReferenceValue != null && value != null)
                {
                    double numericalValue;

                    if (double.TryParse(value?.ToString(), out numericalValue))
                    {
                        double referenceNumValue;
                        double.TryParse(valueCondition.ReferenceValue?.ToString(), out referenceNumValue);

                        double numTolerance;
                        if (!double.TryParse(valueCondition.Tolerance?.ToString(), out numTolerance))
                            numTolerance = 1e-03;

                        switch (valueCondition.Comparison)
                        {
                            case (ValueComparisons.EqualTo):
                                passed = referenceNumValue - numTolerance <= numericalValue && numericalValue <= referenceNumValue + numTolerance;
                                break;
                            case (ValueComparisons.SmallerThan):
                                passed = numericalValue < referenceNumValue + numTolerance;
                                break;
                            case (ValueComparisons.SmallerThanOrEqualTo):
                                passed = numericalValue <= referenceNumValue + numTolerance;
                                break;
                            case (ValueComparisons.LargerThanOrEqualTo):
                                passed = numericalValue >= referenceNumValue - numTolerance;
                                break;
                            case (ValueComparisons.LargerThan):
                                passed = numericalValue > referenceNumValue - numTolerance;
                                break;
                            default:
                                passed = false;
                                break;
                        }
                    }
                    else
                    {
                        // Consider some other way to compare objects
                        if (valueCondition.Comparison == ValueComparisons.EqualTo)
                        {

                            var cc = valueCondition.Tolerance as ComparisonConfig;
                            if (cc != null)
                            {
                                //Compare by hash
                                HashComparer<object> hc = new HashComparer<object>(cc);
                                passed = hc.Equals(value, refValue);
                            }
                            else
                            {
                                // Try checking name compatibility. Useful for materials.
                                string valueString = BH.Engine.CIH.Query.ValueFromSource(value, "Name") as string;
                                string referenceValue = BH.Engine.CIH.Query.ValueFromSource(valueCondition.ReferenceValue, "Name") as string;
                                if (string.IsNullOrWhiteSpace(referenceValue))
                                    referenceValue = valueCondition.ReferenceValue as string;

                                if (valueString == referenceValue)
                                {
                                    passed = true;
                                }
                                else if (value.ToString().Contains("BH.oM") && (valueCondition.ReferenceValue is Type))
                                {
                                    passed = value.ToString() == valueCondition.ReferenceValue.ToString();
                                }
                                else if (value is string && refValue is string)
                                    passed = value.ToString() == refValue.ToString(); // this workaround is required. Not even Convert.ChangeType and dynamic type worked.
                                else
                                {
                                    passed = value == refValue;
                                }

                            }
                        }
                    }
                }

                PopulateConditionResults(obj, value, valueCondition, result, passed);
            }

            return result;
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
                valueString = BH.Engine.Reflection.Query.PropertyValue(value, "Name") as string;

            string conditionText = valueCondition.ToString();
            conditionText = conditionText.Replace(valueCondition.PropertyName + " ", "");

            result.FailInfo.Add($"{valueCondition.PropertyName} was {valueString ?? "empty"}, which does not respect «{conditionText}».");
        }
    }
}
