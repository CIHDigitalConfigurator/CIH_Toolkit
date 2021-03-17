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

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConditionResult ApplyConditions(List<object> objects, List<ICondition> conditions, BooleanOperator booleanOperator = BooleanOperator.AND)
        {
            if (conditions.Count == 1)
                return IApplyCondition(objects, conditions[0]);

            return IApplyCondition(objects, new LogicalCondition() { Conditions = conditions, BooleanOperator = booleanOperator });
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static ConditionResult IApplyCondition(List<object> objects, ICondition condition)
        {
            if (IsAnyConditionNull(condition))
                return null;

            if (objects == null || objects.Count == 0)
                return new ConditionResult() { Condition = condition };

            return ApplyCondition(objects, condition as dynamic);
        }

        /***************************************************/

        //Fallback
        private static ConditionResult ApplyCondition(List<object> objects, ICondition condition)
        {
            BH.Engine.Reflection.Compute.RecordError($"Could not apply {condition.GetType().Name}. No method found.");
            return null;
        }
    }
}
