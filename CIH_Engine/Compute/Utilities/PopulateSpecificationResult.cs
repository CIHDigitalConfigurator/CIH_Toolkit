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

using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.CIH.Specifications;
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static SpecificationResult PopulateSpecificationResult(ISpecification specification, ConditionResult filterResult, ConditionResult checkResult)
        {
            SpecificationResult specRes = new SpecificationResult();

            // Populate the result.
            specRes.PassedObjects.AddRange(checkResult.PassedObjects);
            specRes.FailedObjects.AddRange(checkResult.FailedObjects);
            specRes.NotAssessedObjects.AddRange(filterResult.FailedObjects);

            specRes.Specifications = new List<ISpecification>() { specification };
            specRes.ObjectFailures = new List<ObjectFailures>();

            for (int i = 0; i < specRes.FailedObjects.Count(); i++)
            {
                ObjectFailures f = new ObjectFailures();
                f.Object = specRes.FailedObjects.ElementAtOrDefault(i);
                f.FailedSpecifications = new HashSet<ISpecification>() { specification };

                var fi = string.Join("\n\t", checkResult.FailInfo);
                f.FailInfo = new List<SpecificationFailure>() { new SpecificationFailure() { ParentSpecification = specification, FailedCheckCondition = checkResult.Condition, FailInfo = checkResult.FailInfo[i] } };
                specRes.ObjectFailures.Add(f);
            }

            return specRes;
        }
    }
}
