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
        private static ConditionResult ApplyCondition(List<object> objects, PropertyValueCondition propertyValueCondition)
        {
            if (propertyValueCondition == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Specified condition is null.");
                return null;
            }

            ConditionResult result = new ConditionResult();

            foreach (var obj in objects)
            {
                bool passed = true;

                double actualValue;
                if (double.TryParse(propertyValueCondition.Value.ToString(), out actualValue))
                {
                    double requestedValue = Convert.ToDouble(propertyValueCondition.Value);
                    double tolerance = propertyValueCondition.Tolerance == null ? 1e-03 : Convert.ToDouble(propertyValueCondition.Tolerance);

                    switch (propertyValueCondition.ValueComparison)
                    {
                        case (ValueComparison.Equal):
                            passed = requestedValue - tolerance <= actualValue && actualValue <= requestedValue + tolerance;
                            break;
                        case (ValueComparison.SmallerThan):
                            passed = actualValue < requestedValue + tolerance;
                            break;
                        case (ValueComparison.SmallerThanOrEqual):
                            passed = actualValue <= requestedValue + tolerance;
                            break;
                        case (ValueComparison.LargerThanOrEqual):
                            passed = actualValue >= requestedValue + tolerance;
                            break;
                        case (ValueComparison.LargerThan):
                            passed = actualValue > requestedValue + tolerance;
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
