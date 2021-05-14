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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.CIH
{
    /*
     * The RoomObject is used for storing properties of layout elements and can be verified
     * against the RoomTypology Specifications.
     * It is currently used in the following parts of the Reference Implementation:
     *  - Module 2 (Floor Plan Configurator): used for storing the relevant parameters of RoomObjects generated within the configurator
     *  - Module 2 (Floor Plan Configurator): Verification against RoomTypology specs
     *  - Module 3 (Zone Reference Element Configurator): RoomObject parameters are used as inputs
    */

    [Description("Exposes parameters used for RoomTypology verification and Zone Reference generation" +
        "\n Interfaces with operations of Floor Plan Configurator (Unity app)")]
    public class RoomObject : IObject
    {
        ///<remarks>
        ///TypologyName and Area are parameters used for RoomTypology Specification verification
        ///</remarks> 
        public string TypologyName { get; set; }
        public float Area { get; set; }

        ///<remarks>
        ///The following properties below are used for Zone Reference Element generation in Module 3 
        ///</remarks> 
        public string RoomName { get; set; }
        public List<(float, float)> Outline { get; set; }
        public List<bool> EdgesExternal { get; set; }
        public float Height { get; set; }
        public int Level { get; set; }

        ///<remarks>
        ///The following parameters from the RoopTypology Specification can be implemented in the future
        /*
        public float MinArea { get; set; }
        public bool IsCirculation { get; set; }
        public float DaylightFactor { get; set; }
        public int AcousticSeparation { get; set; }
        */
        ///</remarks> 
    }
}