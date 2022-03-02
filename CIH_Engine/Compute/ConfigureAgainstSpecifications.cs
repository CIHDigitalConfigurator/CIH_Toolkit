using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.CIH.Conditions;
using BH.Engine.Geometry;
using BH.oM.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.Engine.Base;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static HashSet<IFramingElement> previousSet = new HashSet<IFramingElement>();
        private static List<IFramingElement> previousConfig = new List<IFramingElement>();

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        // Configure to Order workflow.
        public static List<IFramingElement> ConfigureAgainstSpecifications(HashSet<IFramingElement> objs, List<Specification> specifications, bool sortDirection = true)
        {
            if (!previousSet.Any())
                previousSet = objs;

            if (!previousSet.Intersect(objs).Any())
                return previousConfig;

            // Override hack.
            previousConfig = ConfigureAgainstSpecifications(objs, specifications);
            return previousConfig;

            List<IFramingElement> result = new List<IFramingElement>();

            List<Specification> compatibleSpecs = new List<Specification>();
            var compatibleObject = CompatibleObjects(objs, specifications, out compatibleSpecs, 1, sortDirection).FirstOrDefault();

            if (compatibleObject == null)
                return new List<IFramingElement>();

            var appliedZoneSpecs = specifications.Where(s => s.IsAppliedZoneSpec());

            List<Point> allDistinctClosedVolumeCentres = new List<Point>();

            foreach (var spec in compatibleSpecs)
            {
                var allClosedVolumes = spec.FilterConditions.OfType<IsInZone>().SelectMany(c => c.ClosedVolumes);
                List<Point> closedVolumesCentres = allClosedVolumes.Select(cv => cv.IBounds().Centre()).ToList();

                closedVolumesCentres = BH.Engine.Geometry.Compute.CullDuplicates(closedVolumesCentres.ToList());

                foreach (var pt in closedVolumesCentres)
                    allDistinctClosedVolumeCentres.Add(pt);
            }

            allDistinctClosedVolumeCentres = BH.Engine.Geometry.Compute.CullDuplicates(allDistinctClosedVolumeCentres);

            foreach (var pt in allDistinctClosedVolumeCentres)
            {
                // The object satisfied this spec. 
                // Move the object into the "location of this spec".
                ICurve geom = compatibleObject.Location;
                var moveVector = BH.Engine.Geometry.Create.Vector(geom.IBounds().Centre(), pt);

                var movedGeom = BH.Engine.Geometry.Modify.ITranslate(geom, moveVector);

                IFramingElement objCopy = compatibleObject.DeepClone();
                objCopy.Location = movedGeom;

                result.Add(objCopy);
            }

            return result.Distinct().ToList();
        }

        public static List<IFramingElement> CompatibleObjects(HashSet<IFramingElement> objects, List<Specification> specifications, out List<Specification> compatibleSpecs, int limit = 1, bool sortDirection = true)
        {
            List<IFramingElement> res = new List<IFramingElement>();
            compatibleSpecs = new List<Specification>();
            List<string> valueCondPropName = new List<string>();

            // Only use specifications that are "applied" Zone Specifications.
            var appliedZoneSpecs = specifications.Where(s => s.IsAppliedZoneSpec()).ToList();

            if (appliedZoneSpecs.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError($"No Applied Zone Specification found. The input Zone specifications do not have any {nameof(IsInZone)} condition, or do not have their {nameof(IsInZone.ClosedVolumes)} populated.");
                return new List<IFramingElement>();
            }

            foreach (var obj in objects)
            {
                foreach (var spec in appliedZoneSpecs)
                {
                    // Remove any IsInZone condition from the specs.
                    // The kit of parts objects are just "archetypes" - not placed in a meaningful location of the model space.
                    Specification specCopy = spec.DeepClone();

                    specCopy.FilterConditions.RemoveAll(fc => fc is IsInZone);
                    specCopy.CheckConditions.RemoveAll(fc => fc is IsInZone);

                    // The Part of the Kit (object archetype) must be moved in its final location,
                    // so we can make sure that all other conditions can be evaluated on the actual object.
                    // e.g. if there are conditions - that are not IsInZone - that are based on position properties.
                    var allClosedVolumes = spec.FilterConditions.OfType<IsInZone>().SelectMany(c => c.ClosedVolumes);
                    List<Point> closedVolumesCentres = allClosedVolumes.Select(cv => cv.IBounds().Centre()).ToList();

                    closedVolumesCentres = BH.Engine.Geometry.Compute.CullDuplicates(closedVolumesCentres.ToList());

                    foreach (var pt in closedVolumesCentres)
                    {
                        ICurve geom = obj.Location;
                        var moveVector = BH.Engine.Geometry.Create.Vector(geom.IBounds().Centre(), pt);

                        var movedGeom = BH.Engine.Geometry.Modify.ITranslate(geom, moveVector);

                        IFramingElement objCopy = obj.DeepClone();
                        objCopy.Location = movedGeom;

                        var specRes = VerifySpecification(new List<object>() { objCopy }, specCopy);
                        if (specRes.PassedObjects.Count != 1)
                            continue;

                        valueCondPropName.AddRange(spec.CheckConditions.OfType<IValueCondition>().Select(c => c.PropertyName));

                        res.Add(obj);
                        compatibleSpecs.Add(spec);
                    }
                }
            }

            res = res.Distinct().ToList();
            valueCondPropName = valueCondPropName.Distinct().ToList();
            compatibleSpecs = compatibleSpecs.Distinct().ToList();

            if (valueCondPropName.Count == 1)
                res = res.OrderBy(obj => obj.ValueFromSource(valueCondPropName.First())).ToList();

            if (!sortDirection)
                res.Reverse();

            if (res.Count() == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("No compatible object found.");
                return new List<IFramingElement>();
            }

            return res.Take(limit).ToList();
        }



        public static List<IFramingElement> ConfigureAgainstSpecifications(HashSet<IFramingElement> objs, List<Specification> specifications)
        {
            List<IFramingElement> res = new List<IFramingElement>();
            List<string> valueCondPropName = new List<string>();

            // Only use specifications that are "applied" Zone Specifications.
            var appliedZoneSpecs = specifications.Where(s => s.IsAppliedZoneSpec()).ToList();

            if (appliedZoneSpecs.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError($"No Applied Zone Specification found. The input Zone specifications do not have any {nameof(IsInZone)} condition, or do not have their {nameof(IsInZone.ClosedVolumes)} populated.");
                return new List<IFramingElement>();
            }

            Dictionary<object, List<IFramingElement>> compatibleObjsPerZone = new Dictionary<object, List<IFramingElement>>();

            foreach (var spec in appliedZoneSpecs)
            {

                // Remove any IsInZone condition from the specs.
                // The kit of parts objects are just "archetypes" - not placed in a meaningful location of the model space.
                Specification specCopy = spec.DeepClone();

                specCopy.FilterConditions.RemoveAll(fc => fc is IsInZone);
                specCopy.CheckConditions.RemoveAll(fc => fc is IsInZone);

                // The Part of the Kit (object archetype) must be moved in its final location,
                // so we can make sure that all other conditions can be evaluated on the actual object.
                // e.g. if there are conditions - that are not IsInZone - that are based on position properties.
                var allClosedVolumes = spec.FilterConditions.OfType<IsInZone>().SelectMany(c => c.ClosedVolumes);
                List<Point> closedVolumesCentres = allClosedVolumes.Select(cv => cv.IBounds().Centre()).ToList();

                closedVolumesCentres = BH.Engine.Geometry.Compute.CullDuplicates(closedVolumesCentres.ToList());


                foreach (var pt in closedVolumesCentres)
                {
                    foreach (var obj in objs)
                    {
                        compatibleObjsPerZone[pt] = compatibleObjsPerZone.ContainsKey(pt) ? compatibleObjsPerZone[pt] : new List<IFramingElement>();

                        ICurve geom = obj.Location;
                        var moveVector = BH.Engine.Geometry.Create.Vector(geom.IBounds().Centre(), pt);

                        var movedGeom = BH.Engine.Geometry.Modify.ITranslate(geom, moveVector);

                        IFramingElement objCopy = obj.DeepClone();
                        objCopy.Location = movedGeom;

                        var specRes = VerifySpecification(new List<object>() { objCopy }, specCopy);
                        if (specRes.PassedObjects.Count != 1)
                            continue;

                        valueCondPropName.AddRange(spec.CheckConditions.OfType<IValueCondition>().Select(c => c.PropertyName));

                        compatibleObjsPerZone[pt].Add(objCopy);
                    }
                }
            }

            List<IFramingElement> result = new List<IFramingElement>();
            foreach (var kv in compatibleObjsPerZone)
            {
                var ordered = kv.Value.OrderBy(v => v.ValueFromSource("Property.IAverageProfileArea")).ToList();

                if (ordered.FirstOrDefault() != null)
                    result.Add(ordered.FirstOrDefault());

            }


            return result;
        }
    }
}
