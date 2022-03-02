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
using BH.oM.Base.Attributes;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Whether an object is predominantly horizontal.\n" +
            "Works by evaluating the vertical VS horizontal extent of its Geometry bounding box.")]
        [Output("True if the object is predominatly horizontal, false otherwise.")]
        public static bool IsHorizontalApprox(this IObject iObj)
        {
            var bb = BH.Engine.CIH.Query.IGeometry(iObj).IBounds();

            if (bb == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot evaluate the bounding box of the object of type {iObj.GetType()}");
                return false;
            }
            double maxVert = Math.Abs(bb.Max.Z - bb.Min.Z);

            double maxHoriz = Math.Max(Math.Abs(bb.Max.Y - bb.Min.Y), Math.Abs(bb.Max.X - bb.Min.X));

            if (maxVert < maxHoriz)
                return true;

            return false;
        }

        [Description("Dispatch objects depending on whether they are predominantly horizontal or vertical.\n" +
            "Works by evaluating the vertical VS horizontal extent of its Geometry bounding box.")]
        [MultiOutput(0,"horiz", "Objects that are predominantly horizontal.")]
        [MultiOutput(1, "vert", "Objects that are predominantly vertical.")]
        public static Output<List<IObject>, List<IObject>> HorizontalVertical(List<IObject> iObjects)
        {
            List<IObject> horizontals = new List<IObject>();
            List<IObject> verticals = new List<IObject>();

            foreach (var iObj in iObjects)
            {
                if (iObj.IsHorizontalApprox())
                    horizontals.Add(iObj);
                else
                    verticals.Add(iObj);
            }

            return new Output<List<IObject>, List<IObject>> { Item1 = horizontals, Item2 = verticals };
        }
    }
}
