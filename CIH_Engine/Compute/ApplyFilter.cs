using BH.oM.Data.Filters;
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
        public static FilterResult ApplyFilters(List<object> objects, List<IFilter> filters, BooleanOperator booleanOperator = BooleanOperator.AND)
        {
            return ApplyFilter(objects, new LogicalFilter() { Filters = filters, BooleanOperator = booleanOperator });
        }

        public static FilterResult IApplyFilter(List<object> objects, IFilter filter)
        {
            return ApplyFilter(objects, filter as dynamic);
        }

        private static FilterResult ApplyFilter(List<object> objects, TypeFilter filter)
        {
            FilterResult result = new FilterResult();
            foreach (var obj in objects)
            {
                if (obj.GetType() == filter.Type)
                {
                    result.PassedObject.Add(obj);
                    result.Pattern.Add(true);
                }
                else
                {
                    result.BlockedObjects.Add(obj);
                    result.Pattern.Add(false);
                }
            }
            return result;
        }

        private static FilterResult ApplyFilter(List<object> objects, IdFilter filter)
        {
            FilterResult result = new FilterResult();
            foreach (var obj in objects)
            {
                IBHoMObject bhomObj = obj as IBHoMObject;

                if (bhomObj == null || filter.Ids.Contains(bhomObj.FindFragment<IAdapterId>()?.Id))
                {
                    result.PassedObject.Add(obj);
                    result.Pattern.Add(true);
                }
                else
                {
                    result.BlockedObjects.Add(obj);
                    result.Pattern.Add(false);
                }
            }
            return result;
        }

        private static FilterResult ApplyFilter(List<object> objects, LogicalFilter filter)
        {
            if (filter.BooleanOperator == BooleanOperator.NOT)
            {
                BH.Engine.Reflection.Compute.RecordError($"Boolean operator `{BooleanOperator.NOT}` is not applicable when combining filters.");
                return null;
            }

            if (filter.Filters.Count > 1)
                BH.Engine.Reflection.Compute.RecordNote($"A total of {filter.Filters.Count} filters were specified. The filters will be applied in sequential order: the result of the first filtering will be filtered by the second filter, and so on.");

            List<bool> passes = new List<bool>();
            Enumerable.Repeat(true, objects.Count);

            List<FilterResult> results = new List<FilterResult>();
            
            foreach (var f in filter.Filters)
            {
                results.Add(IApplyFilter(objects, f));
            }

            IEnumerable<IEnumerable<bool>> patterns = results.Select(r => r.Pattern);
            List<bool> bools = AggreateBooleanSequences(patterns, filter.BooleanOperator);

            FilterResult result = new FilterResult();

            if (bools.Count != objects.Count)
            {
                BH.Engine.Reflection.Compute.RecordError("Error in combining filters.");
                return new FilterResult();
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if(bools[i])
                    result.PassedObject.Add(objects[i]);
                else
                    result.BlockedObjects.Add(objects[i]);
            }

            result.Pattern = bools;

            return result;
        }

        private static List<bool> AggreateBooleanSequences(IEnumerable<IEnumerable<bool>> lists, BooleanOperator booleanOperator = BooleanOperator.AND)
        {
            return lists.Aggregate((a, b) => a.Zip(b, (aElement, bElement) => BooleanOperation(aElement, bElement, booleanOperator))).ToList();
        }

        private static bool BooleanOperation(bool A, bool B, BooleanOperator booleanOperator)
        {
            switch (booleanOperator)
            {
                case (BooleanOperator.AND):
                    return A && B;
                case (BooleanOperator.OR):
                    return A || B;
                case (BooleanOperator.XOR):
                    return A ^ B;
                case (BooleanOperator.IMPLIES):
                    return !A | B;
                default:
                    return false;
            }
        }
    }
}
