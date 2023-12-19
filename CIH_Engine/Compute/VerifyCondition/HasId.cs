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
        private static ConditionResult VerifyCondition(List<object> objects, HasId idCondition)
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
                    result.FailInfo.Add($"{(string.IsNullOrWhiteSpace(idCondition.Clause) ? $"{nameof(HasId)}" : idCondition.Clause) + " failed: "}" +
                        $"object does not have the requested id.");
                    result.Pattern.Add(false);
                }
            }
            return result;
        }
    }
}
