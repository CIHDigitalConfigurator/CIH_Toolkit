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
using BH.Engine.Diffing;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, ISpatialCondition cond)
        {
            // First apply filter to get relevant objects
            BoundingBox containingBox = null;

            Element0DCondition spec0D = cond as Element0DCondition;
            if (spec0D != null)
                containingBox = Query.ElementBoundingBox(spec0D.ReferencePoint, spec0D.LocalXDimension, spec0D.LocalYDimension, spec0D.LocalZDimension);

            Element1DCondition spec1D = cond as Element1DCondition;
            if (spec1D != null)
                containingBox = Query.ElementBoundingBox(spec1D.ReferenceLine, spec1D.LocalYDimension, spec1D.LocalZDimension);

            Element2DCondition spec2D = cond as Element2DCondition;
            if (spec2D != null)
                containingBox = Query.ElementBoundingBox(spec2D.ReferenceElement, spec2D.LocalZDimension);

            IsInBoundingBox bbc = cond as IsInBoundingBox;
            if (bbc != null)
                containingBox = bbc.BoundingBox;

            ConditionResult result = new ConditionResult() { Condition = cond };
            List<string> info = new List<string>();

            foreach (var obj in objects)
            {
                bool passed = false;

                IObject iObj = obj as IObject;
                if (iObj != null) {
                    passed = BH.Engine.CIH.Query.IsContaining(containingBox, iObj);
                    if (passed)
                        result.PassedObjects.Add(obj);
                    else
                    {
                        result.FailedObjects.Add(obj);
                        info.Add($"Object was not {new IsInBoundingBox() { BoundingBox = containingBox }.ToString()}.");
                    }
                }
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"Could not evaluate containment for an object of type `{obj.GetType().Name}`.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }
    }
}
