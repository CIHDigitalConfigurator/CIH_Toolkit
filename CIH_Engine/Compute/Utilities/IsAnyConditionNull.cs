using BH.oM.CIH.Conditions;
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
        private static bool IsAnyConditionNull(ICondition condition, string conditionName = "")
        {
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError($"The condition is null. Please specify a valid condition.");
                return true;
            }

            // Collect all sub-properties of type Condition from the condition and check if any is null
            bool isAnyNull = false;
            var subConditions = condition.GetType().GetProperties().Where(p => p.PropertyType is ICondition);
            foreach (var subCond in subConditions)
            {
                if (subCond.GetValue(condition) == null)
                {
                    BH.Engine.Base.Compute.RecordError($"The condition `{condition.GetType().Name}.{subCond.Name}` is null. Please specify a valid condition.");
                    isAnyNull = true;
                }

            }

            return isAnyNull;
        }
    }
}
