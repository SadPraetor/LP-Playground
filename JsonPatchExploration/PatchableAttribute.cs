using Microsoft.AspNetCore.Authorization;

namespace JsonPatchExploration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PatchableAttribute : Attribute
    {
        public PatchableAttribute(params Type[] requirementTypes)
        {
            RequirementTypes = requirementTypes;
        }

        public Type[] RequirementTypes { get; set; }

        public string Roles { get; set; }

        public string Policies { get; set; }

        public string Claims { get; set; }
    }
}
