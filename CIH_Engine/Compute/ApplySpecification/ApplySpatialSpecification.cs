using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.Data.Conditions;
using BH.oM.Data.Specifications;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult ApplySpecification(List<object> objects, SpatialSpecification spatialSpec)
        {
            SpecificationResult res = new SpecificationResult();
            res.NotAssessedObjects = objects.Where(o => !(o is BHoMObject)).ToList();

            var passedObjs = FilterObjects(objects.OfType<BHoMObject>().ToList(), spatialSpec);



            return null;
        }

        private static List<BHoMObject> FilterObjects(List<BHoMObject> objects, SpatialSpecification spatialSpec)
        {
            List<BHoMObject> bhomObjs = new List<BHoMObject>();

            Dictionary<BHoMObject, Tuple<IElement, IGeometry>> objsReferenceElement = new Dictionary<BHoMObject, Tuple<IElement, IGeometry>>();

            foreach (var obj in objects)
            {
                BHoMObject bhomObj = obj as BHoMObject;
                bhomObjs.Add(bhomObj);

                IGeometry geom = BH.Engine.Base.Query.IGeometry(bhomObj);
                IElement referenceElement = geom as IElement;

                Tuple<IElement, IGeometry> tup = new Tuple<IElement, IGeometry>(referenceElement, Base.Query.IGeometry(bhomObj));

                objsReferenceElement[bhomObj] = tup;
            }

            // Filter objects based on the spatialSpecifications.
            List<HashSet<BHoMObject>> passedObjsSets = new List<HashSet<BHoMObject>>();
            foreach (var zoneSpec in spatialSpec.ZoneSpecifications)
            {
                HashSet<BHoMObject> passedObjs_thisSpec = new HashSet<BHoMObject>();

                // A bounding box is calculated per each object as defined by the individual ZoneSpecification
                Dictionary<BHoMObject, BoundingBox> bbPerObj = new Dictionary<BHoMObject, BoundingBox>();

                foreach (BHoMObject bhomObj in bhomObjs)
                {
                    BoundingBox bb = Query.IElementBoundingBox(objsReferenceElement[bhomObj].Item1, zoneSpec.Width, zoneSpec.Height, zoneSpec.Depth);

                    // If the zone bounding box contains the bhomObject's Geometry, it's passed.
                    if (bb.IsContaining(objsReferenceElement[bhomObj].Item2))
                        passedObjs_thisSpec.Add(bhomObj);
                }

                passedObjsSets.Add(passedObjs_thisSpec);
            }

            // Only objects that passed all the ZoneSpecifications of this SpatialSpecification are actually passed.
            List<BHoMObject> passedObjs = passedObjsSets.Aggregate<IEnumerable<BHoMObject>>((s1, s2) => s1.Intersect(s2)).ToList();

            return passedObjs;
        }
    }
}
