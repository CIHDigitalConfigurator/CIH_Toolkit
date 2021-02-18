/*
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

using BH.oM.Base;
using BH.oM.Data;
using BH.oM.Data.Collections;
using BH.oM.CIH.Conditions;
using BH.oM.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.CIH.Specifications;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SpecificationResult ApplySpecifications(List<object> objects, List<ISpecification> specifications)
        {
            SpecificationResult combinedResult = new SpecificationResult();

            // Find unique objects.
            HashSet<object> allObjs = new HashSet<object>(objects); // TODO: evaluate whether to use our HashComparer.
            objects = allObjs.ToList();

            List<object> passedObjs = new List<object>();
            List<object> failedObjs = new List<object>();
            List<object> NotAssessed = new List<object>();
            Dictionary<object, ObjectFailures> objFailDict = new Dictionary<object, ObjectFailures>();  // one "Failures" per failed object, stating what specification(s) that object failed.

            foreach (var spec in specifications)
            {
                SpecificationResult specRes = ApplySpecification(objects, spec);
                SpecificationFailure failuresForThisSpec = new SpecificationFailure();

                foreach (var obj in specRes.PassedObjects)
                {
                    NotAssessed.Remove(obj);
                    passedObjs.Add(obj);
                }

                foreach (var obj in specRes.FailedObjects)
                {
                    NotAssessed.Remove(obj);
                    passedObjs.Remove(obj);
                    failedObjs.Add(obj);

                    ObjectFailures f;
                    if (!objFailDict.TryGetValue(obj, out f))
                    {
                        f = new ObjectFailures();
                        f.Object = obj;
                        f.FailedSpecifications = new HashSet<ISpecification>();
                        f.FailInfo = new List<SpecificationFailure>();
                    }

                    f.FailedSpecifications.Add(spec);
                    var singleSpecFailure = specRes.ObjectFailures.First();
                    f.FailInfo.Add(new SpecificationFailure() { ParentSpecification = spec, FailedCheckCondition = singleSpecFailure.FailInfo.First().FailedCheckCondition, FailInfo = singleSpecFailure.FailInfo.FirstOrDefault()?.FailInfo });

                    objFailDict[obj] = f;
                }
            }

            NotAssessed = (allObjs.Except(failedObjs)).Except(passedObjs).ToList();

            combinedResult = new SpecificationResult()
            {
                PassedObjects = passedObjs,
                FailedObjects = failedObjs.ToList(),
                NotAssessedObjects = NotAssessed.ToList(),
                Specifications = specifications.Distinct().ToList(),
                ObjectFailures = objFailDict.Values.ToList()
            };

            return combinedResult;
        }
    }
}
