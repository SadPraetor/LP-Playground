using Keycloak.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keycloak.Migration
{
    public class FindUsers
    {
        private readonly KeycloakClient _client;
        private readonly ILogger<UpdatePasswordActionHandler> _logger;
        private const string _realm = "drivalia";
        public FindUsers(Keycloak.Net.KeycloakClient client,
            ILogger<UpdatePasswordActionHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var group = await _client.GetRealmGroupByPathAsync(_realm, $"/cc3_production");

            var groupUsers = (await _client.GetGroupUsersAsync(_realm, group.Id,max:500)).ToList();

            groupUsers = groupUsers.Where(x => x.RequiredActions.Contains("UPDATE_PASSWORD"))
                .ToList();

            var builder = new StringBuilder();

            foreach(var user in groupUsers)
            {
                builder.AppendLine(user.UserName);
            }

            File.WriteAllText("production-new_users.txt",builder.ToString());

            _logger.LogInformation(groupUsers.Count.ToString());
        }

        public void FilterNewProdUsers()
        {
            var testUsers = File.ReadAllLines("accounts-test-new_users.txt");
            var prodUsers = File.ReadAllLines("accounts-production-new_users.txt");

            var onTop = prodUsers.Except(testUsers);

            var builder = new StringBuilder();

            foreach (var user in onTop)
            {
                builder.AppendLine(user);
            }

            File.WriteAllText("accounts-production-excluded-test-new_users.txt", builder.ToString());


        }
    }
}
