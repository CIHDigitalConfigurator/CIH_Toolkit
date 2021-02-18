using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        public static IBHoMObject SetCustomData(IBHoMObject bhomObj, string customDataKey, object value)
        {
            IBHoMObject copy = bhomObj.DeepClone();
            copy.CustomData[customDataKey] = value;
            return copy;
        }
    }
}
