using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        [Description("Returns the corners of a geometrical objects")]
        public static List<Point> IPoints(IGeometry geometry)
        {
            return IPoints(geometry as dynamic);
        }

        public static List<Point> Points(Point g)
        {
            return new List<Point>() { g };
        }

        public static List<Point> Points(Vector g)
        {
            return new List<Point>() { BH.Engine.Geometry.Modify.ITranslate(new Point() { X = 0, Y = 0, Z = 0 }, g) as Point };
        }

        public static List<Point> Points(Plane g)
        {
            return new List<Point>() { g.Origin };
        }

        public static List<Point> Points(ICurve curve)
        {
            return BH.Engine.Geometry.Query.IControlPoints(curve);
        }


        public static List<Point> Points(Mesh g)
        {
            return g.Vertices;
        }

        public static List<Point> Points(PlanarSurface g)
        {
            return g.ExternalBoundary.IControlPoints().Concat(g.InternalBoundaries.SelectMany(ib => ib.IControlPoints())).ToList();
        }

        public static List<Point> Points(Extrusion g)
        {
            return g.Curve.ITranslate(g.Direction).IControlPoints().Concat(g.Curve.IControlPoints()).ToList();
        }

        public static List<Point> Points(Loft g)
        {
            List<Point> controlPts = new List<Point>();
            foreach (ICurve curve in g.Curves)
            {
                controlPts.AddRange(curve.IControlPoints());
            }
            return controlPts;
        }

        public static List<Point> Points(Sphere g)
        {
            return new List<Point>() { g.Centre };
        }

        public static List<Point> Points(Pipe g)
        {
            return g.Centreline.IControlPoints();
        }

        public static List<Point> IsPlanar(this PolySurface surface)
        {
            List<Point> points = new List<Point>();

            foreach (ISurface s in surface.Surfaces)
            {
                points.AddRange(IPoints(s));
            }

            return points;
        }

        public static List<Point> Points(BoundingBox g)
        {
            List<Point> points = new List<Point>();

            double xDim = g.Max.X - g.Min.X;
            double yDim = g.Max.Y - g.Min.Y;
            double zDim = g.Max.Z - g.Min.Z;

            points.Add(g.Min);
            points.Add(g.Max);

            points.Add(g.Min.ITranslate(new Vector() { X = xDim }) as Point);
            points.Add(g.Min.ITranslate(new Vector() { Y = yDim }) as Point);
            points.Add(g.Min.ITranslate(new Vector() { Z = zDim }) as Point);

            points.Add(g.Max.ITranslate(new Vector() { X = -xDim }) as Point);
            points.Add(g.Max.ITranslate(new Vector() { Y = -yDim }) as Point);
            points.Add(g.Max.ITranslate(new Vector() { Z = -zDim }) as Point);

            return points;
        }

        public static List<Point> Points(Cuboid g)
        {
            List<Point> points = new List<Point>();

            double xDim = g.Length;
            double yDim = g.Depth;
            double zDim = g.Height;

            // Top face
            points.Add(g.CoordinateSystem.Origin.ITranslate(g.CoordinateSystem.X * xDim / 2 + g.CoordinateSystem.Y * yDim / 2 + g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(-g.CoordinateSystem.X * xDim / 2 + g.CoordinateSystem.Y * yDim / 2 + g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(-g.CoordinateSystem.X * xDim / 2 - g.CoordinateSystem.Y * yDim / 2 + g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(g.CoordinateSystem.X * xDim / 2 - g.CoordinateSystem.Y * yDim / 2 + g.CoordinateSystem.Z * zDim / 2) as Point);

            // Bottom face
            points.Add(g.CoordinateSystem.Origin.ITranslate(g.CoordinateSystem.X * xDim / 2 + g.CoordinateSystem.Y * yDim / 2 - g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(-g.CoordinateSystem.X * xDim / 2 + g.CoordinateSystem.Y * yDim / 2 - g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(-g.CoordinateSystem.X * xDim / 2 - g.CoordinateSystem.Y * yDim / 2 - g.CoordinateSystem.Z * zDim / 2) as Point);
            points.Add(g.CoordinateSystem.Origin.ITranslate(g.CoordinateSystem.X * xDim / 2 - g.CoordinateSystem.Y * yDim / 2 - g.CoordinateSystem.Z * zDim / 2) as Point);

            return points;
        }

        public static List<Point> Points(CompositeGeometry g)
        {
            List<Point> points = new List<Point>();

            foreach (var geom in g.Elements)
            {
                points.AddRange(IPoints(geom));
            }

            return points;
        }

        public static List<Point> Points(object g)
        {
            BH.Engine.Base.Compute.RecordWarning($"No `Points()` function implementation found for object of type {g.GetType().Name}.");
            return new List<Point>();
        }
    }
}
