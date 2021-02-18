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
        private static ConditionResult ApplyCondition(List<object> objects, FragmentCondition fragmentCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = fragmentCondition };
            foreach (var obj in objects)
            {
                IBHoMObject bhomObj = obj as IBHoMObject;

                if (bhomObj != null && !IsAnyConditionNull(fragmentCondition.Condition, nameof(FragmentCondition) + "." + nameof(FragmentCondition.Condition)))
                {
                    // If there is only one fragment of the specified type, no problem, the check will be done on it.
                    // If the specified type is a parent type, all fragments of that type will be retrieved:
                    // the condition will have to be satisfied for all fragments (forced AND).
                    // To have an OR condition on the child fragments, 
                    // multiple conditions that target the individual fragment child type combined with a logical OR condition is needed.
                    List<IFragment> fragments = bhomObj.GetAllFragments(fragmentCondition.FragmentType);

                    ConditionResult subConditionResult = new ConditionResult();
                    subConditionResult = IApplyCondition(fragments.OfType<object>().ToList(), fragmentCondition.Condition);

                    // Forced AND on all child fragments. See comment above.
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
