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

using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.oM.Data.Specifications;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Data.Collections;
using BH.oM.Data.Library;

namespace BH.Engine.CIH
{
    public static partial class Create
    {
        public static Specification Specification(List<ICondition> filterConditions, List<ICondition> checkConditions,
            string name = "", string description = "", string clause = "")
        {
            return new Specification()
            {
                FilterConditions = filterConditions.Where(c => c != null).ToList(),
                CheckConditions = checkConditions.Where(c => c != null).ToList(),
                SpecName = name,
                Description = description,
                Clause = clause
            };
        }

    }
}
