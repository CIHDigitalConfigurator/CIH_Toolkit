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
        private static ConditionResult ApplyCondition(List<object> objects, PropertyCondition propertyCondition)
        {
            ConditionResult result = new ConditionResult();

            foreach (var obj in objects)
            {
                bool passed = false;

                ConditionResult subConditionResult = new ConditionResult();
                subConditionResult = IApplyCondition(new List<object>() { BH.Engine.Reflection.Query.PropertyValue(obj, propertyCondition.PropertyName) }, propertyCondition.Condition);

                if (subConditionResult != null && subConditionResult.Pattern.Count != 0 && subConditionResult.Pattern.TrueForAll(v => v == true))
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

            return result;
        }

      
    }
}
