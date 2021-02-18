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
using BH.oM.CIH;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SpecificationResult IApplySpecification(List<object> objects, ISpecification specification)
        {
            return ApplySpecification(objects, specification as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static SpecificationResult ApplySpecification(List<object> objects, Specification specification)
        {

            // First apply filter to get relevant objects
            ConditionResult filterResult = ApplyConditions(objects, specification.FilterConditions);

            // Then apply the check to the filteredObject
            ConditionResult checkResult = ApplyConditions(filterResult.PassedObjects, specification.CheckConditions);

            return PopulateSpecificationResult(specification, filterResult, checkResult);
        }

        /***************************************************/

        // Fallback
        private static SpecificationResult ApplySpecification(List<object> objects, ISpecification specification)
        {
            BH.Engine.Reflection.Compute.RecordError($"No method found to apply specification of type {specification.GetType()}.");
            return new SpecificationResult();
        }
    }
}
