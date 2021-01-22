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
            List<object> info = new List<object>();

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
                        double referenceNumValue = Convert.ToDouble(valueCondition.ReferenceValue);

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
                                passed = numericalValue >= referenceNumValue + numTolerance;
                                break;
                            case (ValueComparisons.LargerThan):
                                passed = numericalValue > referenceNumValue + numTolerance;
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
                                passed = hc.Equals(obj, valueCondition.ReferenceValue);
                            }
                            else
                                passed = obj == valueCondition.ReferenceValue;
                        }

                        //result.Passed = propertyValueCondition.Value == propertyValue;

                        //if (valueCheck.ValueComparison == ValueComparison.Equal)
                        //    result.Passed = requestedValue - tolerance <= actualValue && actualValue <= requestedValue + tolerance;
                        //else if (valueCheck.ValueComparison == ValueComparison.SmallerThan)
                        //    result.Passed = actualValue < requestedValue + tolerance;
                        //else if (valueCheck.ValueComparison == ValueComparison.SmallerThanOrEqual)
                        //    result.Passed = actualValue <= requestedValue + tolerance;
                        //else if (valueCheck.ValueComparison == ValueComparison.LargerThan)
                        //    result.Passed = actualValue > requestedValue + tolerance;
                        //else if (valueCheck.ValueComparison == ValueComparison.LargerThanOrEqual)
                        //    result.Passed = actualValue >= requestedValue + tolerance;
                    }
                }

                // 

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"Value of {valueCondition.PropertyName} was {value}, which does not respect '{valueCondition.ToString()}'.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }


    }
}
