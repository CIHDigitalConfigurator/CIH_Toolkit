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
        public static ConditionResult IApplyCondition(List<object> objects, ICondition condition)
        {
            return ApplyCondition(objects, condition as dynamic);
        }

        public static ConditionResult ApplyConditions(List<object> objects, List<ICondition> conditions, BooleanOperator booleanOperator = BooleanOperator.AND)
        {
            return ApplyCondition(objects, new LogicalCondition() { Conditions = conditions, BooleanOperator = booleanOperator });
        }

        //Fallback
        private static ConditionResult ApplyCondition(List<object> objects, ICondition condition)
        {
            BH.Engine.Reflection.Compute.RecordError($"Could not apply {condition.GetType().Name}. No method found.");
            return null;
        }
    }
}
