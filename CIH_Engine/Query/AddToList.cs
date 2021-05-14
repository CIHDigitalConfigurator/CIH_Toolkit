using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using System.ComponentModel;
using System.Collections;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2)
        {
            return new List<object>() { obj1, obj2 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3)
        {
            return new List<object>() { obj1, obj2, obj3 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4)
        {
            return new List<object>() { obj1, obj2, obj3, obj4 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6, obj7 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9 }.Flatten();
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9, object obj10)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9, obj10 }.Flatten();
        }

        // It would be nice to be able to expose this one to the UI instead. `params` not exposable currently.
        private static List<T> AddToList<T>(params T[] objects)
        {
            return new List<T>(objects).OfType<object>().Flatten().OfType<T>().ToList();
        }

        public static List<object> Flatten(this IEnumerable<object> list)
        {
            List<object> result = new List<object>();

            foreach (var item in list)
            {
                IEnumerable li = item as IEnumerable;
                if (item.GetType() != typeof(string) && li != null)
                    foreach (var subItem in li)
                    {
                        result.Add(subItem);
                    }
                else
                    result.Add(item);

            }

            return result;
        }
    }
}
