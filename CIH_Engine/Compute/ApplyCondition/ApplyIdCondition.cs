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
        private static ConditionResult ApplyCondition(List<object> objects, IdCondition idCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = idCondition };
            foreach (var obj in objects)
            {
                IBHoMObject bhomObj = obj as IBHoMObject;

                if (bhomObj == null || idCondition.Ids.Contains(bhomObj.FindFragment<IAdapterId>()?.Id))
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
