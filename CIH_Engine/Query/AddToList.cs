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
        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2)
        {
            return new List<object>() { obj1, obj2 };
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3)
        {
            return new List<object>() { obj1, obj2, obj3 };
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4)
        {
            return new List<object>() { obj1, obj2, obj3, obj4 };
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5 };
        }

        [Description("Function to combine multiple objects into one List. Useful in Excel.")]
        public static List<object> AddToList(object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
        {
            return new List<object>() { obj1, obj2, obj3, obj4, obj5, obj6 };
        }
    }
}
