using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Keycloak.Migration
{
    public class Tests : IClassFixture<TemporaryPasswordGenerator>
    {
        private readonly TemporaryPasswordGenerator _generator;

        public Tests(TemporaryPasswordGenerator generator)
        {
            _generator = generator;
        }

       

        [Theory]
        [Repeat(100)]
        public void PasswordGeneration(int iterationNumber)
        {
          
            var password = _generator.GenerateTemporaryPassword();

            password.Length.Should()
                .BeGreaterThanOrEqualTo(12)
                .And
                .BeLessThanOrEqualTo(15);
        }
    }

    public sealed class RepeatAttribute : Xunit.Sdk.DataAttribute
    {
        private readonly int count;

        public RepeatAttribute(int count)
        {
            if (count < 1)
            {
                throw new System.ArgumentOutOfRangeException(
                    paramName: nameof(count),
                    message: "Repeat count must be greater than 0."
                    );
            }
            this.count = count;
        }

        public override System.Collections.Generic.IEnumerable<object[]> GetData(System.Reflection.MethodInfo testMethod)
        {
            foreach (var iterationNumber in Enumerable.Range(start: 1, count: this.count))
            {
                yield return new object[] { iterationNumber };
            }
        }
    }

    
}
