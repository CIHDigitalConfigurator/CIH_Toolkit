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
                containingBox = Query.GetBoundingBox(spec0D.ReferencePoint, spec0D.LocalXDimension, spec0D.LocalYDimension, spec0D.LocalZDimension);

            Element1DCondition spec1D = cond as Element1DCondition;
            if (spec1D != null)
                containingBox = Query.GetBoundingBox(spec1D.ReferenceLine, spec1D.LocalYDimension, spec1D.LocalZDimension);

            Element2DCondition spec2D = cond as Element2DCondition;
            if (spec2D != null)
                containingBox = Query.GetBoundingBox(spec2D.ReferenceElement, spec2D.LocalZDimension);

            BoundingBoxCondition bbc = cond as BoundingBoxCondition;
            if (bbc != null)
                containingBox = bbc.BoundingBox;

            ConditionResult result = new ConditionResult() { Condition = cond };
            List<string> info = new List<string>();

            foreach (var obj in objects)
            {
                bool passed = false;

                IBHoMObject iBHoMObj = obj as IBHoMObject;
                if (iBHoMObj != null)
                {
                    IGeometry geom = null;

                    if (cond.Containment3D)
                        geom = BH.Engine.Base.Query.IGeometry3D(iBHoMObj);

                    if (!cond.Containment3D || geom == null)
                        geom = BH.Engine.Base.Query.IGeometry(iBHoMObj);

                    BoundingBox bb = BH.Engine.Geometry.Query.IBounds(geom);

                    if (containingBox.IsContaining(bb))
                        passed = true;
                }

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    info.Add($"Object was not {new BoundingBoxCondition() { BoundingBox = containingBox }.ToString()}.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = info;
            return result;
        }
    }
}
