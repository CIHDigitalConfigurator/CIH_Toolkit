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
        private static ConditionResult ApplyCondition(List<object> objects, IsInSet setCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = setCondition };
            List<string> info = new List<string>();

            foreach (var obj in objects)
            {
                bool passed = false;

                object value = obj.ValueFromSource(setCondition.PropertyName);

                if (setCondition.ComparisonConfig != null) // use hashComparer
                    passed = setCondition.Set.Contains(value, new HashComparer<object>(setCondition.ComparisonConfig));
                else if (setCondition.Set.Contains(value)) // use default comparer
                    passed = true;

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"Value of {setCondition.PropertyName} was {value}, which is not among: {string.Join(" | ", setCondition.Set.Select(v => v.ToString()))}.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }


    }
}
