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

using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Data.Specifications;
using System.Collections;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IGeometry IGeometry(object obj)
        {
            if (obj is IGeometry)
                return obj as IGeometry;
            else if (obj is IBHoMObject)
                return ((IBHoMObject)obj).IGeometry();
            else if (obj is IEnumerable)
            {
                List<IGeometry> geometries = new List<IGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IGeometry geometry = IGeometry(item);
                    if (geometry != null)
                        geometries.Add(geometry);
                }
                if (geometries.Count() > 0)
                    return new CompositeGeometry { Elements = geometries };
                else
                    return null;
            }
            else
                return null;
        }
    }
}
