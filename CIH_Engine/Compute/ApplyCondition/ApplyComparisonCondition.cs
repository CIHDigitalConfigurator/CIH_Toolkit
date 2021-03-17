using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.Engine.Diffing;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, IComparisonCondition comparisonCondition)
        {
            ValueCondition vc = comparisonCondition as ValueCondition;
            if (vc != null)
                return ApplyCondition(objects, vc);

            BH.Engine.Reflection.Compute.RecordError($"Failed to verify Condition of type {comparisonCondition.GetType().Name}");
            return new ConditionResult();
        }
    }
}
