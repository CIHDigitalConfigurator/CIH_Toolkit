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

namespace BH.Engine.CIH
{
    public static partial class Convert
    {
        public static string IToBoundingboxCondition(ISpatialCondition spatialCondition)
        {
            if (spatialCondition == null) return null;

            return ToBoundingboxCondition(spatialCondition as dynamic);
        }

        private static BoundingBoxCondition ToBoundingboxCondition(Element2DCondition spatialCondition)
        {
            // Get boundingBox of the Element.

            // In order to extract the geometry out of the IElement, I need to cast it to BHoMObject
            // (the oM.Engine.Base method needs BHoMObject - may be worth adding one/changing it?)
            BoundingBox bb = GetBoundingBox(spatialCondition.ReferenceElement);

            bb.Min.Z -= spatialCondition.LocalZDimension;

            return new BoundingBoxCondition() { BoundingBox = bb, Condition = spatialCondition.Condition, Comment = spatialCondition.Comment, Source = spatialCondition.Source };
        }

        private static BoundingBoxCondition ToBoundingboxCondition(Element1DCondition spatialCondition)
        {
            Line line = spatialCondition.ReferenceLine;
            BoundingBox bb = null;

            // Works only for horizontal or vertical objects.
            Vector vecLocalX = BH.Engine.Geometry.Create.Vector(line.Start, line.End);

            bool isHorizontal = (vecLocalX.Z < 0.001);
            bool isVertical = !isHorizontal && (vecLocalX.X < 0.001 && vecLocalX.Y < 0.001);

            if (!isHorizontal && !isVertical)
            {
                BH.Engine.Reflection.Compute.RecordError("Element must be either horizontal or vertical.");
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

                bb.Max.ITranslate(vecLocalY * spatialCondition.LocalYDimension / 2);
                bb.Min.ITranslate(-vecLocalY * spatialCondition.LocalYDimension / 2);
                bb.Min.ITranslate(-vecLocalZ * spatialCondition.LocalZDimension);
            }

            if (isVertical)
            {
                // Bounding box is centered on the element.

                double minZ = Math.Min(line.Start.Z, line.End.Z);

                Point basePoint = line.Start.DeepClone();
                basePoint.Z = minZ;

                Vector Zlenght = new Vector() { Z = BH.Engine.Geometry.Query.ILength(line) };

                bb = BH.Engine.Geometry.Query.IBounds(BH.Engine.Geometry.Create.Cylinder(basePoint, Zlenght));
            }

            if (bb == null)
            {
                BH.Engine.Reflection.Compute.RecordError($"Error computing the BoundingBox of the {nameof(spatialCondition)}.");
                return null;
            }

            return new BoundingBoxCondition() { BoundingBox = bb, Condition = spatialCondition.Condition, Comment = spatialCondition.Comment, Source = spatialCondition.Source };
        }


        private static Vector UnitVector(this Vector vec)
        {
            double length = vec.Length();
            return new Vector() { X = vec.X / length, Y = vec.Y / length, Z = vec.Z / length };
        }

        private static BoundingBox GetBoundingBox(IElement element)
        {
            if (element == null)
                return null;

            // To get the boundingBox of the Element, I need to first get the geometry out of the element.

            // In order to extract the geometry out of the IElement, I need to cast it to BHoMObject, if it's not a Geometry already.
            // (the oM.Engine.Base method needs BHoMObject - may be worth adding one/changing it?)
            IGeometry geometry = element as IGeometry;

            if (geometry == null)
                geometry = BH.Engine.Base.Query.IGeometry(element as BHoMObject);

            if (geometry == null)
                return null;

            BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geometry);

            return bb;
        }
    }
}
