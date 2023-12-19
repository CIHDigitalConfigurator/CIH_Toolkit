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
        private static ConditionResult VerifyCondition(List<object> objects, IsOfType typeCondition)
        {
            ConditionResult result = new ConditionResult() { Condition = typeCondition };
            List<string> info = new List<string>();

            Type type = typeCondition.Type is string ? BH.Engine.Base.Create.Type(typeCondition.Type.ToString()) : typeCondition.Type as Type;

            if (type == null)
            {
                string error = $"Invalid {nameof(IsOfType.Type)} input in the given {nameof(IsOfType)}.";
                BH.Engine.Base.Compute.RecordError(error);
                result.FailedObjects = objects;
                result.FailInfo = Enumerable.Repeat(error, objects.Count).ToList();
                return result;
            }

            foreach (var obj in objects)
            {
                if (obj.GetType() == type)
                {
                    result.PassedObjects.Add(obj);
                    result.Pattern.Add(true);
                }
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"{(string.IsNullOrWhiteSpace(typeCondition.Clause) ? $"{nameof(IsOfType)}" : typeCondition.Clause) + " failed: "}" +
                        $"Type was `{obj.GetType().Name}` instead of `{type.Name}`.");
                    result.Pattern.Add(false);
                }
            }

            result.FailInfo = info;
            return result;
        }
    }
}
