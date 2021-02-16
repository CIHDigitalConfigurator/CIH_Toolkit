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
        private static ConditionResult ApplyCondition(List<object> objects, LogicalCondition logicalCondition)
        {
            ConditionResult combinedResult = new ConditionResult() { Condition = logicalCondition };

            if (logicalCondition.Conditions.Count == 0)
                return combinedResult;

            if (logicalCondition.BooleanOperator == BooleanOperator.NOT)
            {
                BH.Engine.Reflection.Compute.RecordError($"Boolean operator `{BooleanOperator.NOT}` is not applicable when combining filters.");
                return null;
            }

            if (logicalCondition.Conditions.Count > 1)
                BH.Engine.Reflection.Compute.RecordNote($"A total of {logicalCondition.Conditions.Count} filters were specified. The filters will be applied in sequential order: the result of the first filtering will be filtered by the second filter, and so on.");

            List<bool> passes = new List<bool>();
            Enumerable.Repeat(true, objects.Count);

            List<ConditionResult> results = new List<ConditionResult>();

            // FailInfos matrix: contains all failInfo, for each condition, for each object.
            var failInfos = new List<List<string>>();
            // ParentList = one subList of infos per each failed condition
            // SubList = one FailInfo per each failed object.

            List<string> allFailuresPerObject = new List<string>();

            foreach (var f in logicalCondition.Conditions)
            {
                ConditionResult r = IApplyCondition(objects, f);
                if (r != null)
                    results.Add(r);

                failInfos.Add(r.FailInfo);
            }

            Dictionary<object, string> failInfoPerObject = new Dictionary<object, string>();
            foreach (var r in results)
            {
                for (int i = 0; i < r.FailedObjects.Count; i++)
                {
                    object failedObject = r.FailedObjects[i];
                    if (!failInfoPerObject.ContainsKey(failedObject))
                        failInfoPerObject[failedObject] = r.FailInfo[i];
                    else
                        failInfoPerObject[failedObject] += "\n\t" + r.FailInfo[i];
                }
            }

            IEnumerable<IEnumerable<bool>> patterns = results.Select(r => r.Pattern);
            List<bool> bools = AggreateBooleanSequences(patterns, logicalCondition.BooleanOperator);


            if (bools.Count != objects.Count)
            {
                BH.Engine.Reflection.Compute.RecordError("Error in combining filters.");
                return new ConditionResult();
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (bools[i])
                    combinedResult.PassedObjects.Add(objects[i]);
                else
                    combinedResult.FailedObjects.Add(objects[i]);
            }

            // Add Failinfo in order
            foreach (var failedObj in combinedResult.FailedObjects)
            {
                string str;
                if (failInfoPerObject.TryGetValue(failedObj, out str))
                    combinedResult.FailInfo.Add(str);
            }

            combinedResult.Pattern = bools;

            return combinedResult;
        }

        private static List<bool> AggreateBooleanSequences(IEnumerable<IEnumerable<bool>> lists, BooleanOperator booleanOperator = BooleanOperator.AND)
        {
            List<bool> result = new List<bool>();

            if (lists != null && lists.Count() != 0)
                result = lists.Aggregate((a, b) => a.Zip(b, (aElement, bElement) => BooleanOperation(aElement, bElement, booleanOperator))).ToList();

            return result;
        }

        private static bool BooleanOperation(bool A, bool B, BooleanOperator booleanOperator)
        {
            switch (booleanOperator)
            {
                case BooleanOperator.AND:
                    return A && B;
                case BooleanOperator.OR:
                    return A || B;
                case BooleanOperator.XOR:
                    return A ^ B;
                case BooleanOperator.IMPLIES:
                    return !A | B;
                default:
                    return false;
            }
        }
    }
}
