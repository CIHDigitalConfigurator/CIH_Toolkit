using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, BoundingBoxCondition bbc)
        {
            ConditionResult result = new ConditionResult() { Condition = bbc };
            List<string> failInfo = new List<string>();

            foreach (var obj in objects)
            {
                IGeometry geom = null;
                BHoMObject bhomObj = obj as BHoMObject;
                if (bhomObj != null)
                {
                    if (bbc.Containment3D)
                        geom = bhomObj.IGeometry3D();
                    else
                        geom = bhomObj.IGeometry();
                }

                if (obj is IGeometry)
                    geom = obj as IGeometry;

                BoundingBox geomBB = Geometry.Query.IBounds(geom);

                bool passed = bbc.BoundingBox.IsContaining(geomBB, true);

                if (passed)
                    result.PassedObjects.Add(obj);
                else
                {
                    result.FailedObjects.Add(obj);
                    failInfo.Add($"Object not in the specified Bounding Box.");
                }

                result.Pattern.Add(passed);
            }

            result.FailInfo = failInfo;
            return result;
        }
    }
}
