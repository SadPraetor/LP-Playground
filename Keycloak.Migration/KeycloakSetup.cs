using Keycloak.Net.Models.Clients;
using Keycloak.Net.Models.Groups;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Keycloak.Migration
{
    public class KeycloakSetup
    {
        private readonly Keycloak.Net.KeycloakClient _client;        
        private readonly ILogger<KeycloakSetup> _logger;
        private const string _realm = "drivalia";

        public KeycloakSetup(Keycloak.Net.KeycloakClient client,           
            ILogger<KeycloakSetup> logger)
        {
            _client = client;   
            _logger = logger;
        }

        public async Task SetupKeycloakAsync(string environment = "test")
        {
            await CreateGroupAsync(environment);
            await CreateClient(environment);
            return;

            var realm = "drivalia";
            var clientExists = await _client.GetClientAsync(realm,$"cc3_{environment}");

            if (clientExists is null)
            {
                var newClient = new Client()
                {
                    ClientId = $"cc3_{environment}",
                    Name = $"cc3_{environment}",
                    ClientAuthenticatorType = "client-secret",
                    StandardFlowEnabled = true,
                    ImplicitFlowEnabled = true,
                    PublicClient = false,
                    Protocol = "openid-connect",
                    RedirectUris = ["https://localhost:7227/*"],
                    WebOrigins = ["https://localhost:7227/*"],
                    Attributes = { 
                        { "post.logout.redirect.uris", "https://localhost:7227" } 
                    },


                };
            }
        }

        private async Task CreateGroupAsync(string environment)
        {
            var existingGroup = await _client.GetRealmGroupByPathAsync(_realm, $"/cc3_{environment}");

            if(existingGroup is not null)
            {
                _logger.LogInformation("Group {group} already exists in realm {realm}", $"cc3_{environment}", _realm);
                return;
            }

            var group = new Group()
            {
                Name = $"cc3_{environment}",
                Path= $"/cc3_{environment}",
            };

            await _client.CreateGroupAsync(_realm, group);
            _logger.LogInformation("Group {group} created in realm {realm}", $"cc3_{environment}", _realm);
        }

        private async Task CreateClient (string environment)
        {
            var existingClients = await _client.GetClientsAsync(_realm);

            if (existingClients is not null && existingClients.Any(x => x.ClientId == $"cc3_{environment}"))
            {
                _logger.LogInformation("Client {clientId} already exists", $"cc3_{environment}");
                return;
            }

            using FileStream stream = File.OpenRead(@".\Input\client_template.json");
            var template = await JsonSerializer.DeserializeAsync<Client>(stream);

            if(template is null)
            {
                throw new InvalidOperationException("Failed to load client template");
            }

            template.ClientId = $"cc3_{environment}";
            template.Name = $"cc3_{environment}";

            await _client.CreateClientAsync(_realm, template);
            _logger.LogInformation("Client {client} created in realm {realm}", $"cc3_{environment}", _realm);
        }
    }
}
