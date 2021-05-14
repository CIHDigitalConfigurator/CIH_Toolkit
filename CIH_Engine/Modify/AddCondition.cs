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
using System.ComponentModel;
using BH.oM.Data.Specifications;

namespace BH.Engine.CIH
{
    public static partial class Modify
    {
        [Description("Adds the given condition to the input specification's FilterConditions.")]
        public static Specification AddConditions(Specification specification, List<ICondition> filterConditions, List<ICondition> checkConditions, string nameToAppend = "", string clauseToAppend = "")
        {
            Specification spec = specification.DeepClone();

            spec.FilterConditions.AddRange(filterConditions);
            spec.CheckConditions.AddRange(checkConditions);

            spec.SpecName += nameToAppend;
            spec.Clause += clauseToAppend;

            return spec;
        }

        [Description("Adds the given condition to the input specification's FilterConditions.")]
        public static Specification AddFilterCondition(Specification specification, ICondition condition)
        {
            Specification spec = specification.DeepClone();

            spec.FilterConditions.Add(condition);

            return spec;
        }

        [Description("Adds the given condition to the input specification's CheckConditions.")]
        public static Specification AddCheckCondition(Specification specification, ICondition condition)
        {
            Specification spec = specification.DeepClone();

            spec.CheckConditions.Add(condition);

            return spec;
        }
    }
}
