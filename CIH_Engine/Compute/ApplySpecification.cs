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
        public static SpecificationResult ApplySpecification(List<object> objects, Specification specification)
        {
            // First apply filter to get relevant objects
            List<object> filteredObjects = ApplyFilters(objects, specification.Filters).PassedObject;

            // Then apply the check to the filteredObject
            List<object> checkedObjects = ApplyChecks(filteredObjects, specification.Checks);

            return null;
        }
    }
}
