using BH.oM.Data.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIH_Engine
{
    public partial class Create
    {
        public DomainCondition DomainCondition(double min, double max)
        {
            return new DomainCondition() { Domain = BH.Engine.Data.Create.Domain(min, max) };
        }
    }
}