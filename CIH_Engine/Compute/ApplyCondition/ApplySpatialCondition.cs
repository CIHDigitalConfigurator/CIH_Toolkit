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

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static ConditionResult ApplyCondition(List<object> objects, Element1DSpecification cond)
        {
            // All spatial conditions can be converted to a "is in box" condition.
            // BoundingBox is limiting because it's only aligned to the global axes; in future, consider switching to Cuboid.
            BoundingBox bb = Query.GetBoundingBox(cond.ReferenceLine, cond.LocalYDimension, cond.LocalZDimension);

            // SpatialConditions hold a condition that only holds in a certain location in space.
            // Therefore a first step is filtering the objects that are in that space location; then the condition will apply to them.
            // Objects that are not in the space location must be returned as failed.



            return null;

        }
    }
}
