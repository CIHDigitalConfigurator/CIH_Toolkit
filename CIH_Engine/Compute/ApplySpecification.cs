using BH.oM.Base;
using BH.oM.Data.Filters;
using BH.oM.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        public static List<object> ApplySpecification(List<object> objects, Specification specification)
        {
            // First apply filter to get relevant objects
            List<object> filteredObjects = ApplyFilter(objects, specification.Filter);

            // Then apply the check to the filteredObject

            return null;
        }
    }
}
