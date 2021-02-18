using BH.oM.Data.Conditions;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data;
using BH.oM.Data.Specifications;

namespace BH.Engine.CIH
{
    public static partial class Compute
    {
        private static SpecificationResult PopulateSpecificationResult(ISpecification specification, ConditionResult filterResult, ConditionResult checkResult)
        {
            SpecificationResult specRes = new SpecificationResult();

            // Populate the result.
            specRes.PassedObjects.AddRange(checkResult.PassedObjects);
            specRes.FailedObjects.AddRange(checkResult.FailedObjects);
            specRes.NotAssessedObjects.AddRange(filterResult.FailedObjects);

            specRes.Specifications = new List<ISpecification>() { specification };
            specRes.ObjectFailures = new List<ObjectFailures>();

            for (int i = 0; i < specRes.FailedObjects.Count(); i++)
            {
                ObjectFailures f = new ObjectFailures();
                f.Object = specRes.FailedObjects.ElementAtOrDefault(i);
                f.FailedSpecifications = new HashSet<ISpecification>() { specification };

                var fi = string.Join("\n\t", checkResult.FailInfo);
                f.FailInfo = new List<SpecificationFailure>() { new SpecificationFailure() { ParentSpecification = specification, FailedCheckCondition = checkResult.Condition, FailInfo = checkResult.FailInfo[i] } };
                specRes.ObjectFailures.Add(f);
            }

            return specRes;
        }
    }
}
