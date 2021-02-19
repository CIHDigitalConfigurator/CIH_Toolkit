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
            List<string> info = new List<string>();

            var refValue = valueCondition.ReferenceValue;
            if (refValue == null)
                BH.Engine.Reflection.Compute.RecordNote($"A {nameof(ValueCondition)}'s {nameof(valueCondition.ReferenceValue)} was null. Make sure this is intended.\nTo check for null/not null, consider using a {nameof(ValueNullCondition)} instead.");


            foreach (var obj in objects)
            {
                bool passed = true;

                object value = obj.ValueFromSource(valueCondition.PropertyName);

                if (valueCondition.ReferenceValue == null && obj == null)
                    passed = true;

                if (valueCondition.ReferenceValue == null && obj != null)
                    passed = false;

                if (valueCondition.ReferenceValue != null && obj == null)
                    passed = false;

                if (valueCondition.ReferenceValue != null && obj != null)
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
                                passed = hc.Equals(obj, refValue);
                            }
                            else
                            {
                                if (value.ToString().Contains("BH.oM") && !(valueCondition.ReferenceValue is Type))
                                {
                                    // If the value is a Type and the referenceValue is not, try comparing the `Name` property extracted from both.
                                    // In some cases (e.g. Materials) this is useful.
                                    string valueString = BH.Engine.Reflection.Query.PropertyValue(value, "Name") as string;
                                    string referenceValue = BH.Engine.Reflection.Query.PropertyValue(valueCondition.ReferenceValue, "Name") as string;
                                    passed = valueString == referenceValue;

                                    if (!passed)
                                    {
                                        // If the previous failed, try comparing the Name extracted from the Value with the ToString() of the reference value.
                                        // This is useful when an enum is provided as ReferenceValue.
                                        passed = valueString == valueCondition.ReferenceValue.ToString();
                                    }
                                }
                                else if (value.ToString().Contains("BH.oM") && (valueCondition.ReferenceValue is Type))
                                {
                                    passed = value.ToString() == valueCondition.ReferenceValue.ToString();
                                }
                                else if (value is string && refValue is string)
                                    passed = value.ToString() == refValue.ToString(); // workaround needed. Not even Convert.ChangeType and dynamic type worked.
                                else
                                    passed = value == refValue;

                            }

                        }
                        //else if (valueCondition.Comparison == ValueComparisons.SmallerThan)
                        //    passed = value < refValue;
                        //else if (valueCondition.Comparison == ValueComparisons.SmallerThanOrEqualTo)
                        //    passed = actualValue <= requestedValue + tolerance;
                        //else if (valueCondition.Comparison == ValueComparisons.LargerThan)
                        //    passed = actualValue > requestedValue + tolerance;
                        //else if (valueCondition.Comparison == ValueComparisons.LargerThanOrEqualTo)
                        //    passed = actualValue >= requestedValue + tolerance;
                    }
                }

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    string valueString = value.ToString();

                    if (valueString.Contains("BH.oM") && !(valueCondition.ReferenceValue is Type))
                        valueString = BH.Engine.Reflection.Query.PropertyValue(value, "Name") as string;

                    info.Add($"{valueCondition.PropertyName} was {valueString ?? "empty"}, which does not respect '{valueCondition.ToString()}'.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }


    }
}
