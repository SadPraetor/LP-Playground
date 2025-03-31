using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keycloak.Migration
{
    public class TemporaryPasswordGenerator
    {
        private readonly Faker _faker;
        private string[] _base = ["auticko", "lease", "drivalia"];
        private string[] _spice = ["Plan",  "Call"];
        private string[] _special = ["*"];

        public TemporaryPasswordGenerator()
        {
            Randomizer.Seed = new Random(999);
            _faker = new Faker();
        }


        public string GenerateTemporaryPassword()
        {
            return _faker.PickRandom(_base) + _faker.PickRandom(_spice) + _faker.PickRandom(_special) + _faker.Random.Int(10, 99).ToString();
        }
    }
}
