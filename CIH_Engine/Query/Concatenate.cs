using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using System.ComponentModel;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        [Description("Concatenate the elements of two lists into one list.")]
        public static List<object> Concatenate(List<object> list1, List<object> list2)
        {
            return Concat(list1, list2).ToList();
        }

        [Description("Concatenate the elements of multiple lists into one list.")]
        public static List<object> Concatenate(List<object> list1, List<object> list2, List<object> list3)
        {
            return Concat(list1, list2, list3).ToList();
        }

        [Description("Concatenate the elements of multiple lists into one list.")]
        public static List<object> Concatenate(List<object> list1, List<object> list2, List<object> list3, List<object> list4)
        {
            return Concat(list1, list2, list3, list4).ToList();
        }

        [Description("Concatenate the elements of multiple lists into one list.")]
        public static List<object> Concatenate(List<object> list1, List<object> list2, List<object> list3, List<object> list4, List<object> list5)
        {
            return Concat(list1, list2, list3, list4, list5).ToList();
        }

        [Description("Concatenate the elements of multiple lists into one list.")]
        public static List<object> Concatenate(List<object> list1, List<object> list2, List<object> list3, List<object> list4, List<object> list5, List<object> list6)
        {
            return Concat(list1, list2, list3, list4, list5, list6).ToList();
        }

        // It would be nice to be able to expose this one to the UI instead. `params` not exposable currently.
        private static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sequences)
        {
            return sequences.SelectMany(x => x);
        }
    }
}
