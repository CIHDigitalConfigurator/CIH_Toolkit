using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, ValueComparison valueCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = valueCondition };

            foreach (var obj in objects)
            {
                bool passed = true;
                double numericalValue;

                if (valueCondition.ReferenceValue != null && obj == null)
                    passed = false;
                else if (valueCondition.ReferenceValue != null && double.TryParse(obj.ToString(), out numericalValue))
                {
                    double requestedNumbericalValue = Convert.ToDouble(valueCondition.ReferenceValue);
                    double tolerance = valueCondition.Tolerance == null ? 1e-03 : Convert.ToDouble(valueCondition.Tolerance);

                    switch (valueCondition.Comparison)
                    {
                        case (ValueComparisons.EqualTo):
                            passed = requestedNumbericalValue - tolerance <= numericalValue && numericalValue <= requestedNumbericalValue + tolerance;
                            break;
                        case (ValueComparisons.SmallerThan):
                            passed = numericalValue < requestedNumbericalValue + tolerance;
                            break;
                        case (ValueComparisons.SmallerThanOrEqualTo):
                            passed = numericalValue <= requestedNumbericalValue + tolerance;
                            break;
                        case (ValueComparisons.LargerThanOrEqualTo):
                            passed = numericalValue >= requestedNumbericalValue + tolerance;
                            break;
                        case (ValueComparisons.LargerThan):
                            passed = numericalValue > requestedNumbericalValue + tolerance;
                            break;
                        default:
                            passed = false;
                            break;
                    }

                }
                else
                {
                    // Consider some way to compare objects 

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

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                    result.FailedObjects.Add(obj);

                result.Pattern.Add(passed);
            }


            return result;
        }


    }
}
