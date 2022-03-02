using BH.oM.CIH.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.CIH
{
    public static partial class Query
    {
        [Description("Dispatch input objects in two lists, depending on whether they are of the provided Type or not.")]
        [Input("objs", "Objects to dispatch.")]
        [Input("Type", "The Type can either be a string representing the FullName of the type or a `System.Type` instance.")]
        [MultiOutputAttribute(0, "OfType", "Objects that are of the input Type.")]
        [MultiOutputAttribute(1, "NotOfType", "Objects that are not of the input Type.")]
        public static Output<List<object>, List<object>> OfType(List<object> objs, object Type)
        {
            Type type = Type is string ? BH.Engine.Base.Create.Type(Type.ToString()) : Type as Type;

            if (type == null)
            {
                BH.Engine.Base.Compute.RecordError("Invalid type provided.");
                return null;
            }

            var output = new Output<List<object>, List<object>>();

            output.Item1 = objs.Where(o => type.IsAssignableFrom(o.GetType())).ToList();
            output.Item2 = objs.Where(o => !type.IsAssignableFrom(o.GetType())).ToList();

            return output;
        }
    }
}
