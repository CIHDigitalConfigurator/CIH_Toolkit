using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.Engine.Reflection;
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
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static object ValueFromSource(this object obj, string sourceName)
        {
            if (obj == null || sourceName == null)
                return null;

            if (sourceName.Contains("."))
            {
                string[] props = sourceName.Split('.');
                foreach (string innerProp in props)
                {
                    obj = obj.ValueFromSource(innerProp);
                    if (obj == null)
                        break;
                }
                return obj;
            }

            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(sourceName);

            if (prop != null)
                return prop.GetValue(obj);
            else
                return GetValue(obj as dynamic, sourceName);

        }

        private static object GetValue(this IBHoMObject obj, string sourceName)
        {
            IBHoMObject bhomObj = obj as IBHoMObject;
            object value;
            if (bhomObj.CustomData.ContainsKey(sourceName))
            {
                value = bhomObj.CustomData.TryGetValue(sourceName, out value);
                return value;
            }
            else if (sourceName.ToLower().Contains("customdata["))
            {
                string keyName = sourceName.Substring(sourceName.IndexOf('[')+1, sourceName.IndexOf(']') - sourceName.IndexOf('[') -1);
                bhomObj.CustomData.TryGetValue(keyName, out value);
                return value;
            }
            else
            {
                if (sourceName.Contains("."))
                {
                    string[] props = sourceName.Split('.');

                    Type fragmentType = BH.Engine.Reflection.Create.Type(sourceName, true);

                    if (fragmentType != null)
                    {
                        List<IFragment> allFragmentsOfType = bhomObj.Fragments.Where(fr => fragmentType.IsAssignableFrom(fr.GetType())).ToList();
                        List<object> values = allFragmentsOfType.Select(f => ValueFromSource(f, string.Join(".", props.Skip(1)))).ToList();
                        return values.Count == 1 ? values.First() : values;
                    }

                }
            }

            return null;
        }

        /***************************************************/

        private static object GetValue<T>(this Dictionary<string, T> dic, string propName)
        {
            if (dic.ContainsKey(propName))
            {
                return dic[propName];
            }
            else
            {
                return null;
            }
        }

        /***************************************************/

        private static object GetValue<T>(this IEnumerable<T> obj, string propName)
        {
            return obj.Select(x => x.ValueFromSource(propName)).ToList();
        }


        /***************************************************/
        /**** Fallback Methods                           ****/
        /***************************************************/

        private static object GetValue(this object obj, string propName)
        {
            return null;
        }

    }
}
