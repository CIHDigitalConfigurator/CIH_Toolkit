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
using BH.oM.Data.Conditions;
using System;
using System.Collections.Generic;

namespace BH.oM.Data.Specifications
{
    public class SpecificationResult : IObject
    {
        public virtual List<object> PassedObjects { get; set; } = new List<object>();
        public virtual List<object> FailedObjects { get; set; } = new List<object>();
        public virtual List<object> NotAssessedObjects { get; set; } = new List<object>();
        public virtual Specification Specification { get; set; }
    }

    public class CombinedSpecificationsResult : IObject
    {
        public virtual List<object> PassedObjects { get; set; } //objects that passed all specifications.
        public virtual List<object> FailedObjects { get; set; } = new List<object>(); //objects that failed one specification or more.
        public virtual List<object> NotAssessedObjects { get; set; } = new List<object>(); // objects that were not assessed by any of the specifications.
        public virtual List<Specification> Specifications { get; set; } // All specifications that were applied.
        public List<Failures> Failures { get; set; } // objects that failed one or more specifications, and what specifications they failed.
    }

    public class Failures : IObject
    {
        public virtual object Object { get; set; } // Object that failed the specifications.
        public virtual List<ISpecification> FailedSpecifications { get; set; } // What specifications were failed.
    }
}