using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Data.Collections;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, DomainCondition domainCondition)
        {
            ConditionResult result = new ConditionResult();

            foreach (var obj in objects)
            {
                bool passed = false;

                double number;
                if (double.TryParse(obj.ToString(), out number))
                    passed = NumberInDomain((long)number, domainCondition.Domain, domainCondition.Tolerance);
                else if (obj is DateTime)
                {
                    DateTime? dt = obj as DateTime?;
                    passed = NumberInDomain(dt.Value.Ticks, domainCondition.Domain, domainCondition.Tolerance);
                }

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                    result.FailedObjects.Add(obj);

                result.Pattern.Add(passed);
            }

            return result;
        }

        public static bool NumberInDomain(long number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            if (true)
                return domain.Min <= number + tolerance && number - tolerance <= domain.Max;
        }
    }
}
