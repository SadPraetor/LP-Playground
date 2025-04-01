using Keycloak.Net;
using Keycloak.Net.Models.RealmsAdmin;
using Keycloak.Net.Models.Users;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Keycloak.Migration
{
    public class UpdatePasswordActionHandler
    {
        private const string _realm = "drivalia";
        private readonly KeycloakClient _client;
        private readonly ILogger<UpdatePasswordActionHandler> _logger;

        public UpdatePasswordActionHandler(Keycloak.Net.KeycloakClient client,
            ILogger<UpdatePasswordActionHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task SetActionUpdatePasswordAsync()
        {
            List<UserDto> userDtos = new();
            using (FileStream fileStream = File.OpenRead("accounts-prod-created.json"))
            {

                using StreamReader reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    try
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            var userDto = JsonSerializer.Deserialize<UserDto>(line);

                            if (userDto is null)
                            {
                                _logger.LogWarning("Failed to deserialize line {line} into UserDto", line);
                                continue;
                            }

                            if (!userDto.UserCreatedByProcess.GetValueOrDefault())
                            {
                                continue;
                            }

                            _logger.LogInformation("Processing {user}", userDto.PortalUserName);

                            var user = await FindUserAsync(userDto.PortalUserName);

                            if(user is null)
                            {
                                _logger.LogWarning("Failed to find user {portalusername}", userDto.PortalUserName);
                                continue;
                            }

                            user.RequiredActions = new List<string>() { "UPDATE_PASSWORD" };

                            await _client.UpdateUserAsync(_realm, user.Id, user);
                            _logger.LogInformation("UPDATE_PASSWORD action set for {user}", userDto.PortalUserName);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private async Task<User?> FindUserAsync(string username)
        {

            try
            {
                var users = await _client.GetUsersAsync(_realm, username: username);
                return users.FirstOrDefault();
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
