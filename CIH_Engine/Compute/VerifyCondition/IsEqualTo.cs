using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.CIH;
using BH.Engine.Diffing;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult VerifyCondition(List<object> objects, IValueCondition cond)
        {
            ConditionResult result = new ConditionResult() { Condition = cond };

            var refValue = cond.ReferenceValue;
            if (refValue == null)
                BH.Engine.Reflection.Compute.RecordNote($"A {nameof(ValueCondition)}'s {nameof(cond.ReferenceValue)} was null. Make sure this is intended.\nTo check for null/not null, consider using a {nameof(IsNull)} instead.");


            ValueCondition valueCondition = cond.ToValueCondition();

            ConditionResult valueCondRes = VerifyCondition(objects, valueCondition);
            result.FailedObjects = valueCondRes.FailedObjects;
            result.PassedObjects = valueCondRes.PassedObjects;
            result.Pattern = valueCondRes.Pattern;
            result.FailInfo = valueCondRes.FailInfo;

            return result;
        }
    }
}
