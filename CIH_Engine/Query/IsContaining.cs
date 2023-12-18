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
using System.Reflection;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static bool IsContaining(IGeometry container, IObject iObj, ContainmentRules containmentRule = ContainmentRules.Geometry, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            bool passed = false;

            if (iObj == null)
                return passed;

            IGeometry geom = null;

            if (containmentRule == ContainmentRules.AtLeastOneGeometryPoint)
            {
                geom = BH.Engine.CIH.Query.IGeometry(iObj);
                List<Point> points = BH.Engine.CIH.Query.IPoints(geom);
                foreach (Point pt in points)
                {
                    if (container.IIsContaining(pt))
                    {
                        passed = true;
                        break;
                    }
                }
            }
            else if (containmentRule == ContainmentRules.Geometry3D)
            {
                geom = BH.Engine.Base.Query.IGeometry3D(iObj);
                if (geom == null)
                {
                    BH.Engine.Base.Compute.RecordError($"Could not get Geometry3D for {iObj.GetType().FullName}.");
                    passed = false;
                }
                else
                {
                    BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geom);

                    if (container.IIsContaining(bb))
                        passed = true;
                }
            }
            else if (containmentRule == ContainmentRules.BoundingBoxCentre)
            {
                geom = BH.Engine.Base.Query.IGeometry3D(iObj);
                BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geom);

                if (container.IIsContaining(bb.Centre()))
                    passed = true;
            }
            else
            {
                // Fall back on ContainmentRule = "Geometry"
                geom = BH.Engine.CIH.Query.IGeometry(iObj);

                BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geom);

                //BoundingBox containerBB = container as BoundingBox;

                ////containerBB.Max += new Vector() { X = 1, Y = 1, Z = 1 };
                ////containerBB.Min += new Vector() { X = -1, Y = -1, Z = -1 };

                if (container.IIsContaining(bb))
                    passed = true;
            }

            return passed;
        }

        private static bool IIsContaining(this IGeometry container, IGeometry geom, ContainmentRules containmentRule = ContainmentRules.Geometry, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            Cuboid cuboid = container as Cuboid;
            if (cuboid != null)
                return BH.Engine.Geometry.Query.IsContaining(cuboid, geom, acceptOnEdge, tolerance);

            BoundingBox bb = container as BoundingBox;
            if (bb != null)
                return BH.Engine.Geometry.Query.IsContaining(bb, geom, acceptOnEdge, tolerance);


            BH.Engine.Base.Compute.RecordError($"No valid IsContaining method found for container object of type {container.GetType().Name}.");
            return false;
        }

    }
}
