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
        private static ConditionResult ApplyCondition(List<object> objects, CustomDataCondition customDataCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = customDataCondition };
            foreach (var obj in objects)
            {
                IBHoMObject bhomObj = obj as IBHoMObject;

                if (bhomObj != null && !IsAnyConditionNull(customDataCondition.Comparison, nameof(CustomDataCondition) + "." + nameof(CustomDataCondition.Comparison)))
                {
                    object customDataValue = null;
                    bhomObj.CustomData.TryGetValue(customDataCondition.CustomDataKey, out customDataValue);

                    ConditionResult subConditionResult = new ConditionResult();
                    subConditionResult = IApplyCondition(new List<object>() { customDataValue }, customDataCondition.Comparison);

                    if (subConditionResult.Pattern.TrueForAll(v => v == true))
                    {
                        result.PassedObjects.Add(obj);
                        result.Pattern.Add(true);

                    }
                    else
                    {
                        result.FailedObjects.Add(obj);
                        result.Pattern.Add(false);
                    }
                }
                else
                {
                    // If the object is not a BHoMObject, the condition can't apply to it.
                    result.FailedObjects.Add(obj);
                    result.Pattern.Add(false);
                }
            }
            return result;
        }
    }
}
