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
        private static ConditionResult VerifyCondition(List<object> objects, IsEqualTo cond)
        {
            return ConvertToValueConditionAndVerify(objects, cond);
        }

        private static ConditionResult VerifyCondition(List<object> objects, IsLessThan cond)
        {
            return ConvertToValueConditionAndVerify(objects, cond);
        }

        private static ConditionResult VerifyCondition(List<object> objects, IsGreaterThan cond)
        {
            return ConvertToValueConditionAndVerify(objects, cond);
        }

        private static ConditionResult VerifyCondition(List<object> objects, IsLessThanOrEqualTo cond)
        {
            return ConvertToValueConditionAndVerify(objects, cond);
        }

        private static ConditionResult VerifyCondition(List<object> objects, IsGreaterThanOrEqualTo cond)
        {
            return ConvertToValueConditionAndVerify(objects, cond);
        }

        private static ConditionResult ConvertToValueConditionAndVerify(List<object> objects, IValueCondition cond)
        {
            ConditionResult result = new ConditionResult() { Condition = cond };

            var refValue = cond.ReferenceValue;
            if (refValue == null)
                BH.Engine.Base.Compute.RecordNote($"A {cond.GetType().Name}'s {nameof(cond.ReferenceValue)} was null. Make sure this is intended.\nTo check for null/not null, consider using a {nameof(IsNull)} instead.");


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
