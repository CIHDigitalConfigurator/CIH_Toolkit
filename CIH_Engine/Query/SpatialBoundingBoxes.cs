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

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract the bounding boxes associated to a SpatialSpecification.")]
        public static List<BoundingBox> SpatialBoundingBoxes(this SpatialSpecification spatialSpec)
        {
            List<BoundingBox> result = new List<BoundingBox>();

            List<IGeometry> locations = spatialSpec.Locations.Select(l => BH.Engine.CIH.Query.IGeometry(l)).ToList();
            ZoneSpecification zoneSpec = spatialSpec.ZoneSpecification;

            Dictionary<BHoMObject, Tuple<IElement, IGeometry>> objsReferenceElement = new Dictionary<BHoMObject, Tuple<IElement, IGeometry>>();

            foreach (var obj in locations)
            {
                BoundingBox bb = Query.IElementBoundingBox(obj, zoneSpec.Width, zoneSpec.Height, zoneSpec.Depth);

                if (bb != null)
                    result.Add(bb);
            }

            return result;
        }
    }
}
