/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.CIH.Conditions;
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
                if (bhomObj.CustomData.TryGetValue(sourceName, out value))
                    return value;
            }
            else if (sourceName.ToLower().Contains("customdata["))
            {
                string keyName = sourceName.Substring(sourceName.IndexOf('[') + 1, sourceName.IndexOf(']') - sourceName.IndexOf('[') - 1);
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
                else
                {
                    // Try extracting the property using an Extension method.
                    return BH.Engine.Reflection.Compute.RunExtensionMethod(obj, sourceName);
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
