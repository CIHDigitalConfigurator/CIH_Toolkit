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

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Data
{
    [Description("Describes what kind of rule should be applied to evaluate whether a BoundingBox actually contains some BHoMObject." +
        "\n Geometry = checks the containment of the 'essential' Geometry of the object. E.g. for a Beam, this is the Beam's centreline." +
        "\n Geometry3D = checks the containment of the 'volumetric' Geometry of the object. E.g. for a Beam, this is the Beam's extruded view of its profile section." +
        "\n BoundingBoxCentre = Centre defined as the centre of the object's BoundingBox." +
        "\n AtLeastOneGeometryPoint = checks the containment of at least one of the object's 'essential' Geometry's points. Points of the object include any surface corner, mesh vertex, curve endpoint, curve kink, etc.")]
    public enum ContainmentRules
    {
        Geometry,
        Geometry3D,
        BoundingBoxCentre,
        AtLeastOneGeometryPoint,
    }
}

