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
    public static partial class Query
    {
        public static string Description(ICondition condition)
        {
            return condition.ToString();
        }

        public static string Description(ConditionResult condRes)
        {
            return condRes.Condition.ToString();
        }
    }
}
