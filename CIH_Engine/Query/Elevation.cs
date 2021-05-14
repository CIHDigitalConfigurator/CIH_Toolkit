using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Physical;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        public static double Elevation(this IFramingElement obj)
        {
            return Elevation(obj, ElevationFrom.MidPoint);
        }

        public static double Elevation(this IFramingElement obj, ElevationFrom elevationFrom = ElevationFrom.MidPoint)
        {
            if (elevationFrom == ElevationFrom.MidPoint)
                return obj.Location.IPointAtParameter(0.5).Z;

            if (elevationFrom == ElevationFrom.LowestPoint)
                return BH.Engine.CIH.Query.Points(obj.Location).OrderBy(p => p.Z).First().Z;

            if (elevationFrom == ElevationFrom.HighestPoint)
                return BH.Engine.CIH.Query.Points(obj.Location).OrderBy(p => p.Z).Last().Z;

            BH.Engine.Reflection.Compute.RecordError($"Could not compute the elevation of a {obj.GetType().Name}.");
            return 0;
        }
    }
}
