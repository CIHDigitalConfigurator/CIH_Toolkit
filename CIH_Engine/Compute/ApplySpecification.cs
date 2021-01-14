using BH.oM.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        public static List<object> ApplySpecification(List<object> objects, ISpecification specification)
        {
            List<object> filteredObjects = ApplyFilter(objects, specification.Filter);
            List<>
        }
    }
}
