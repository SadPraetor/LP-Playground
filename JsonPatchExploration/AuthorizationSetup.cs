using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatchExploration
{
    public class IsOwnerRequirement : IAuthorizationRequirement
    {
    }

    public class OverrideRequirement : IAuthorizationRequirement { }
}
