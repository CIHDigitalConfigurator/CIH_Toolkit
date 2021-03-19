using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult VerifyCondition(List<object> objects, IsInZone isInZone)
        {
            ConditionResult result = new ConditionResult() { Condition = isInZone };
            List<string> failInfo = new List<string>();

            foreach (var obj in objects)
            {
                bool passed = false;

                IObject iObj = obj as IObject;
                if (iObj != null)
                {
                    //passed = BH.Engine.CIH.Query.IsContaining(bbc.BoundingBox, iObj);
                    if (passed)
                        result.PassedObjects.Add(obj);
                    else
                    {
                        result.FailedObjects.Add(obj);
                        failInfo.Add($"Object not in the specified Bounding Box.");
                    }
                }
                else
                {
                    result.FailedObjects.Add(obj);
                    failInfo.Add($"Could not evaluate containment for an object of type `{obj.GetType().Name}`.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = failInfo;
            return result;
        }
    }
}
