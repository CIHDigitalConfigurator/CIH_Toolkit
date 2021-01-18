using BH.oM.Data.Checks;
using BH.oM.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        public static List<object> ApplyChecks(List<object> objects, List<ICheck> check)
        {
            return null;
        }

        private static CheckResult ApplyPropertyCheck(object obj, IPropertyCheck check)
        {
            object propertyValue = BH.Engine.Reflection.Query.PropertyValue(obj, check.PropertyName);

            bool passed = false;
            CheckResult result = new CheckResult() { Object = obj, Check = check, Passed = passed };

            PropertyInDomain domainCheck = check as PropertyInDomain;
            if (domainCheck != null)
            {
                double number = Convert.ToDouble(propertyValue);

                result.Passed = domainCheck.Domain.Min <= number + domainCheck.Tolerance && number - domainCheck.Tolerance <= domainCheck.Domain.Max;
                return result;
            }

            PropertyValueCheck valueCheck = check as PropertyValueCheck;
            if (valueCheck != null)
            {
                double actualValue;

                if (double.TryParse(propertyValue.ToString(), out actualValue))
                {
                    double requestedValue = Convert.ToDouble(valueCheck.Value);
                    double tolerance = valueCheck.Tolerance == null ? 1e-03 : Convert.ToDouble(valueCheck.Tolerance);

                    if (valueCheck.ValueComparison == ValueComparison.Equal)
                        result.Passed = requestedValue - tolerance <= actualValue && actualValue <= requestedValue + tolerance;
                    else if (valueCheck.ValueComparison == ValueComparison.SmallerThan)
                        result.Passed = actualValue < requestedValue + tolerance;
                    else if (valueCheck.ValueComparison == ValueComparison.SmallerThanOrEqual)
                        result.Passed = actualValue <= requestedValue + tolerance;
                    else if (valueCheck.ValueComparison == ValueComparison.LargerThan)
                        result.Passed = actualValue > requestedValue + tolerance;
                    else if (valueCheck.ValueComparison == ValueComparison.LargerThanOrEqual)
                        result.Passed = actualValue >= requestedValue + tolerance;
                }
                else
                {
                    // Consider some way to compare objects 

                    result.Passed = valueCheck.Value == propertyValue;

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
                
                return result;
            }

            return null;
        }
    }
}
