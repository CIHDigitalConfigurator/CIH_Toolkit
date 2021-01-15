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
using BH.oM.Data.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Data.Checks
{
    [Description("Describes the result of a Check on a single object.")]
    public class CheckResult : IObject
    {
        public virtual object Object { get; set; }
        public virtual ICheck Check { get; set; }
        public virtual bool Passed { get; set; } // alternative: CheckStatus Status
    }

    [Description("Describes the status of a Check: Passed, Failed, and 3 different Warning levels. Objects returned with a Warning are still considered `Passed`.")]
    public enum CheckStatus
    {
        Passed, // The check on the object passed with no warning.
        Warning_Severe, // The check on the object passed, but severe problems were found that need immediate attention.
        Warning_Moderate, // The check on the object passed, but moderate problems were found that need attention.
        Warning_Minor,  // The check on the object passed, but minor problems were found.
        Failed // The check on the object failed.
    }
}


