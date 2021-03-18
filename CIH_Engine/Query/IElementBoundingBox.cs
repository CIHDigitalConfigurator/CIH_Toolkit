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

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts the Bounding Box of a IElement using its geometrical component(s) and additional parameters to indicate any missing dimension.")]
        public static BoundingBox IElementBoundingBox(IObject geometricalObj, params double[] pars)
        {
            if (geometricalObj == null)
                return null;

            IElement element = geometricalObj as IElement;
            if (geometricalObj == null)
                element = IGeometry(geometricalObj) as IElement;

            if (element is IElement2D || geometricalObj is ISurface)
                if (pars[0] != 0 && (pars.Count() == 1 || (pars[1] == 0 && pars[2] == 0)))
                    return ElementBoundingBox(geometricalObj, pars.FirstOrDefault());
                else
                    BH.Engine.Reflection.Compute.RecordError($"Incorrect number of dimensions specified for this {geometricalObj.GetType().Name}. Specified {pars.Count()} dimensions while only 1 is required.");

            if (element is IElement1D)
                if (pars[0] != 0 && pars[1] != 0 && (pars.Count() == 2 || pars[0] != 0) )
                    return ElementBoundingBox(element as IElement1D, pars[0], pars[1]);
                else
                    BH.Engine.Reflection.Compute.RecordError($"Incorrect number of dimensions specified for this {geometricalObj.GetType().Name}. Specified {pars.Count()} dimensions while 2 are required.");

            if (element is IElement0D)
                if (pars.Count() == 3)
                    return ElementBoundingBox(element as IElement0D, pars[0], pars[1], pars[2]);
                else
                    BH.Engine.Reflection.Compute.RecordError($"Incorrect number of dimensions specified for this {geometricalObj.GetType().Name}. Specified {pars.Count()} dimensions while 3 are required.");

            BH.Engine.Reflection.Compute.RecordError($"No matching method found for element {geometricalObj.GetType().Name} and dimensions {string.Join(", ", pars)}");
            return null;

        }

        /***************************************************/

        public static BoundingBox ElementBoundingBox(IObject element2d, double localZDimension)
        {
            // Get boundingBox of the Element.

            // In order to extract the geometry out of the IElement, I need to cast it to BHoMObject
            // (the oM.Engine.Base method needs BHoMObject - may be worth adding one/changing it?)

            IGeometry geometry = BH.Engine.Base.Query.IGeometry(element2d as BHoMObject);
            if (element2d is IGeometry && geometry == null)
                geometry = element2d as IGeometry;

            BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geometry);

            bb.Min.Z -= localZDimension;

            return bb;
        }

        /***************************************************/

        public static BoundingBox ElementBoundingBox(IElement1D element1d, double localYDimension, double localZDimension)
        {
            Line line = element1d as Line;
            if (line != null)
                return ElementBoundingBox(line, localYDimension, localZDimension);
            
            ICurve crv = element1d as ICurve;
            if (element1d != null)
            {
                line = BH.Engine.Geometry.Create.Line(crv.IStartPoint(), crv.IEndPoint());
                return ElementBoundingBox(line, localYDimension, localZDimension);
            }

            BH.Engine.Reflection.Compute.RecordError($"This method currently supports only Elements that are of type {nameof(Line)}.");
            return null;
        }

        /***************************************************/

        public static BoundingBox ElementBoundingBox(Line line, double localYDimension, double localZDimension)
        {
            BoundingBox bb = null;

            // Works only for horizontal or vertical objects.
            Vector vecLocalX = BH.Engine.Geometry.Create.Vector(line.Start, line.End);

            bool isHorizontal = (vecLocalX.Z < 0.001);
            bool isVertical = !isHorizontal && (vecLocalX.X < 0.001 && vecLocalX.Y < 0.001);

            if (!isHorizontal && !isVertical)
            {
                BH.Engine.Reflection.Compute.RecordError("Line must be either horizontal or vertical.");
                return null;
            }

            Vector vecLocalY;
            Vector vecLocalZ;
            if (isHorizontal)
            {
                // Bounding box is below-centered wrt the referenceLine.

                vecLocalY = BH.Engine.Geometry.Query.CrossProduct(new Vector() { Z = 1 }, vecLocalX).UnitVector();
                vecLocalZ = new Vector() { Z = 1 };

                bb = BH.Engine.Geometry.Query.IBounds(line);

                bb.Max = bb.Max.ITranslate(vecLocalY * localYDimension / 2) as Point;
                bb.Min = bb.Min.ITranslate(-vecLocalY * localYDimension / 2) as Point;
                bb.Min = bb.Min.ITranslate(-vecLocalZ * localZDimension) as Point;
            }

            if (isVertical)
            {
                // Bounding box is centered on the element.

                vecLocalY = BH.Engine.Geometry.Query.CrossProduct(new Vector() { Y = 1 }, vecLocalX).UnitVector();
                vecLocalZ = new Vector() { Y = 1 };

                double minZ = Math.Min(line.Start.Z, line.End.Z);

                Point basePoint = line.Start.DeepClone();
                basePoint.Z = minZ;

                Vector Zlenght = new Vector() { Z = BH.Engine.Geometry.Query.ILength(line) };

                bb = BH.Engine.Geometry.Query.IBounds(line);

                bb.Max = bb.Max.ITranslate(vecLocalY * localYDimension / 2) as Point;
                bb.Max = bb.Max.ITranslate(vecLocalZ * localZDimension / 2) as Point;
                bb.Min = bb.Min.ITranslate(-vecLocalY * localYDimension / 2) as Point;
                bb.Min = bb.Min.ITranslate(-vecLocalZ * localZDimension / 2) as Point;

            }

            if (bb == null)
            {
                BH.Engine.Reflection.Compute.RecordError($"Error computing the BoundingBox of the Reference Line.");
                return null;
            }

            // Ensure validity of the Bounding Box. The min/max have to be lowest/highest in all coordinates.
            Point min = new Point()
            {
                X = new List<double>() { bb.Max.X, bb.Min.X }.Min(),
                Y = new List<double>() { bb.Max.Y, bb.Min.Y }.Min(),
                Z = new List<double>() { bb.Max.Z, bb.Min.Z }.Min(),
            };

            Point max = new Point()
            {
                X = new List<double>() { bb.Max.X, bb.Min.X }.Max(),
                Y = new List<double>() { bb.Max.Y, bb.Min.Y }.Max(),
                Z = new List<double>() { bb.Max.Z, bb.Min.Z }.Max(),
            };

            bb.Min = min;
            bb.Max = max;

            return bb;
        }

        /***************************************************/

        public static BoundingBox ElementBoundingBox(IElement0D pt, double localXDimension, double localYDimension, double localZDimension)
        {
            throw new NotImplementedException(); //TODO.
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Vector UnitVector(this Vector vec)
        {
            double length = vec.Length();
            return new Vector() { X = vec.X / length, Y = vec.Y / length, Z = vec.Z / length };
        }
    }
}
