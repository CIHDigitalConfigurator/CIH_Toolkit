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
using System.ComponentModel;

namespace BH.oM.Data.Specifications
{
    public class SpecificationResult : IObject
    {
        [Description("Specification(s) that were applied.")]
        public virtual List<ISpecification> Specifications { get; set; } // All specifications that were applied.
        [Description("Objects that passed all specifications.")]
        public virtual List<object> PassedObjects { get; set; } = new List<object>();
        [Description("Objects that failed one specification or more.")]
        public virtual List<object> FailedObjects { get; set; } = new List<object>();
        [Description("Objects that were not assessed by any of the specifications.")]
        public virtual List<object> NotAssessedObjects { get; set; } = new List<object>();
        [Description("Objects that failed one or more specifications, and what specifications they failed.")]
        public List<ObjectFailures> ObjectFailures { get; set; } = new List<ObjectFailures>();
    }

    public class ObjectFailures : IObject
    {
        [Description("Object that failed the specifications.")]
        public virtual object Object { get; set; }
        [Description("All specifications that this object failed.")]
        public virtual HashSet<ISpecification> FailedSpecifications { get; set; }
        [Description("Info about the specification fails.")]
        public virtual List<SpecificationFailure> FailInfo { get; set; }
    }

    public class SpecificationFailure : IObject
    {
        [Description("From what Specification the failed checkCondition come from.")]
        public virtual ISpecification ParentSpecification { get; set; }
        [Description("The checkCondition that failed.")]
        public virtual ICondition FailedCheckCondition { get; set; }
        [Description("Info from the failed checkCondition")]
        public virtual object FailInfo { get; set; } 
    }
}