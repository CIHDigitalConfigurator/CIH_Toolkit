﻿/*
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

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.CIH.Conditions;
using BH.oM.Data.Specifications;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.CIH.Specifications;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult ApplySpecification(List<object> objects, SpatialSpecification spatialSpec)
        {
            SpecificationResult specResult = new SpecificationResult();
            specResult.NotAssessedObjects = objects.Where(o => !(o is BHoMObject)).ToList();

            // Apply filters.
            ConditionResult filterResultAggregated = new ConditionResult();

            List<BHoMObject> bhomObjs = objects.OfType<BHoMObject>().ToList();

            Dictionary<BHoMObject, BoundingBox> passedObjs_bb = FilterObjects(bhomObjs, spatialSpec);

            filterResultAggregated.PassedObjects = passedObjs_bb.Keys.OfType<object>().ToList();
            filterResultAggregated.FailedObjects = bhomObjs.Except(passedObjs_bb.Keys).OfType<object>().ToList();

            // Apply checks.
            ConditionResult checkResultAggregated = new ConditionResult();

            foreach (var kv in passedObjs_bb)
            {
                BHoMObject objToCheck = kv.Key;
                BoundingBox zoneBB = kv.Value;
                IGeometry geom3D = BH.Engine.Base.Query.IGeometry3D(kv.Key);

                bool isContained = zoneBB.IsContaining(geom3D);

                if (!isContained)
                    checkResultAggregated.FailInfo.Add("The 3D geometry of the object is not contained in the Zone.");


                var checkResult_obj = ApplyConditions(new List<object>() { objToCheck }, spatialSpec.ZoneSpecification.CheckConditions);
                if (isContained)
                {
                    checkResultAggregated.PassedObjects.AddRange(checkResult_obj.PassedObjects);
                    checkResultAggregated.FailedObjects.AddRange(checkResult_obj.FailedObjects);
                    checkResultAggregated.Condition = checkResult_obj.Condition;
                }
                else
                {
                    checkResultAggregated.FailedObjects.Add(objToCheck);
                    List<ICondition> failedConditions = new List<ICondition> { new BoundingBoxCondition() { BoundingBox = zoneBB } };
                    failedConditions.AddRange(spatialSpec.ZoneSpecification.CheckConditions);
                    checkResultAggregated.Condition = new LogicalCondition() { Conditions = failedConditions };
                }

                checkResultAggregated.FailInfo.AddRange(checkResult_obj.FailInfo);
            }

            return PopulateSpecificationResult(spatialSpec, filterResultAggregated, checkResultAggregated);
        }

        private static Dictionary<BHoMObject, BoundingBox> FilterObjects(List<BHoMObject> objects, SpatialSpecification spatialSpec)
        {
            Dictionary<BHoMObject, BoundingBox> passedObj_zoneBox = new Dictionary<BHoMObject, BoundingBox>();

            Dictionary<BHoMObject, IGeometry> objsGeometry = new Dictionary<BHoMObject, IGeometry>();

            // Compute the geometry for each object.
            foreach (var obj in objects)
            {
                BHoMObject bhomObj = obj as BHoMObject;
                IGeometry geom = BH.Engine.Base.Query.IGeometry(bhomObj);

                if (geom == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning($"Could not Query the Geometry out of a given {bhomObj.GetType().Name}.");
                    continue;
                }

                objsGeometry[bhomObj] = geom;
            }

            // Filter objects based on the spatialSpecifications.
            ZoneSpecification zoneSpec = spatialSpec.ZoneSpecification;

            // A bounding box is calculated per each object as defined by the individual ZoneSpecification
            Dictionary<BHoMObject, BoundingBox> bbPerObj = new Dictionary<BHoMObject, BoundingBox>();
            List<BoundingBox> allZoneBoxes = spatialSpec.SpatialBoundingBoxes();

            foreach (BHoMObject bhomObj in objsGeometry.Keys)
            {
                // Check if any of the Specification's boundingBoxes contains the object
                foreach (var zoneBB in allZoneBoxes)
                {
                    if (zoneBB.IsContaining(objsGeometry[bhomObj]))
                    {
                        // If the zone bounding box contains the bhomObject's Geometry, let's apply the other filters.
                        var res = ApplyCondition(new List<object>() { bhomObj }, new LogicalCondition() { Conditions = zoneSpec.FilterConditions });
                        if (res.PassedObjects.Count == 1) // if the object passed all the provided Conditions, then it's passed.
                            passedObj_zoneBox[res.PassedObjects.First() as BHoMObject] = zoneBB;
                    }
                }
            }

            return passedObj_zoneBox;
        }
    }
}
