using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatchExploration
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Patchable(typeof(IsOwnerRequirement),typeof(OverrideRequirement),Claims = "testuser")]
        public string Department { get; set; }

        public DateOnly DateOfBirth { get; set; }

    }
}
