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
        private static ConditionResult ApplyCondition(List<object> objects, IsNull valueNullCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = valueNullCondition };
            List<string> info = new List<string>();

            var requiredState = valueNullCondition.NullCondition;

            foreach (var obj in objects)
            {
                bool passed = false;
                object value = obj.ValueFromSource(valueNullCondition.PropertyName);

                if (requiredState == ValueNullConditions.MustBeNull)
                    passed = value == null;
                else if (requiredState == ValueNullConditions.MustBeNotNull)
                    passed = value != null;

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"{valueNullCondition.PropertyName} was {value ?? "empty"}, which does not respect '{valueNullCondition.ToString()}'.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }
    }
}
