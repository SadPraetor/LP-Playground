using Keycloak.Net;
using Keycloak.Net.Models.Groups;
using Keycloak.Net.Models.Users;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Keycloak.Migration
{
    class UserMigrationService
    {
        private readonly KeycloakClient _client;
        private readonly TemporaryPasswordGenerator _passwordGenerator;
        private readonly ILogger<UserMigrationService> _logger;
        private const string _realm = "drivalia";

        public UserMigrationService(Keycloak.Net.KeycloakClient client,
            TemporaryPasswordGenerator passwordGenerator,
            ILogger<UserMigrationService> logger)
        {
            _client = client;
            _passwordGenerator = passwordGenerator;
            _logger = logger;
        }

        public async Task MigrateUsersFromFile(string environment = "test")
        {
            List<UserDto> userDtos = new();
            using (FileStream fileStream = File.OpenRead("accounts-prod-v2.json"))
            {

                using StreamReader reader = new StreamReader(fileStream);

                var group = await _client.GetRealmGroupByPathAsync(_realm, $"/cc3_{environment}");                

                if(group is null)
                {
                    _logger.LogError("Group {group} is missing", $"cc3_{environment}");
                    return;
                }

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

                            _logger.LogInformation("Processing {user}", userDto.PortalUserName);

                            var existingUser = await FindUserAsync(userDto.PortalUserName);

                            if(existingUser is not null)
                            {
                                _logger.LogInformation("User {user} exists in realm {realm}", userDto.PortalUserName, _realm);
                                userDto.UserCreatedByProcess = false;
                            }
                            else
                            {
                                var user = await CreateUserAsync(userDto,environment);
                                if (user is null)
                                {
                                    continue;
                                }
                                existingUser = user;
                            }

                            await UpdateUserGroupAsync(existingUser,group, environment);
                            await UpdateUserPortalUsernameAttributeAsync(existingUser, userDto, environment);
                            userDtos.Add(userDto);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Failed to process line");                        
                    }
                }
            }

            using var stream = new FileStream("accounts.json", FileMode.OpenOrCreate);

            foreach (var user in userDtos)
            {
                await JsonSerializer.SerializeAsync(stream, user);
                //await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.SerializeAsync( user));
                stream.WriteByte(Convert.ToByte('\r'));
                stream.WriteByte(Convert.ToByte('\n'));
            }

            await stream.FlushAsync();

        }

        internal async Task<User?> FindUserAsync(string username)
        {

            try
            {
                var users = await _client.GetUsersAsync(_realm,username: username);
                return users.FirstOrDefault();
            }
            catch (Exception exception)
            {                
                return null;
            }            
        }

        private async Task<User?> CreateUserAsync(UserDto userDto, string environment)
        {
            var user = new User()
            {
                UserName = userDto.PortalUserName,
                Attributes = new ()
                {
                    {"cc3_portalusername",[userDto.PortalUserName] }
                },
                FirstName = userDto.FirstName,
                LastName = userDto.LastName ?? string.Empty,
                Email = userDto.Email ?? string.Empty,
                Groups = [$"cc3_{environment}"],
                Enabled = true,
               // RequiredActions = new List<string>() { "UPDATE_PASSWORD" }
            };
            

            var created = await _client.CreateUserAsync(_realm, user);
            
            if(!created)
            {
                _logger.LogWarning("Failed to create user {portalusername}", userDto.PortalUserName);
                userDto.UserCreatedByProcess = false;
                return null;
            }                       

            userDto.UserCreatedByProcess = true;
            _logger.LogInformation("Created user {portalusernam} in realm {realm}", userDto.PortalUserName, _realm);

            user = (await _client.GetUsersAsync(_realm, username: userDto.PortalUserName)).First();

            user.RequiredActions = new List<string>() { "UPDATE_PASSWORD" };

            await _client.UpdateUserAsync(_realm, user.Id, user);

            var password = _passwordGenerator.GenerateTemporaryPassword();
            var response = await _client.SetUserPasswordAsync(_realm, user.Id, password);
            if (response.Success)
            {
                _logger.LogInformation("Password set for user {user}", userDto.PortalUserName);
                userDto.TemporaryGeneratedPassword = password;
                return user;
            }

            
            _logger.LogWarning("Failed to set password for {user}", userDto.PortalUserName);
            return user;
        }

        private async Task UpdateUserGroupAsync(User user,Group group, string environment)
        {
            var groupName = $"cc3_{environment}";

            var userGroups = await _client.GetUserGroupsAsync(_realm, user.Id);

            if (userGroups is not null && userGroups.Select(x=>x.Name).ToList().Contains(groupName))
            {
                _logger.LogInformation("User {user} is already member of group {group}", user.UserName, groupName);
                return;
            }

            var updated = await _client.UpdateUserGroupAsync(_realm, user.Id,group.Id,group);

            if (updated)
            {
                _logger.LogInformation("User {user} added to group {group}", user.UserName, groupName);
                return;
            }

            _logger.LogWarning("Failed to add {user} to group {group}", user.UserName,groupName);
        }

        private async Task UpdateUserPortalUsernameAttributeAsync(User user, UserDto userDto, string environment)
        {
            var attribute = "cc3_portalusername";
            if (user.Attributes is not null && user.Attributes.ContainsKey(attribute))
            {
                _logger.LogInformation("User {user} already has attribute cc3_portalusername", user.UserName);
                return;
            }
            user.Attributes ??= new Dictionary<string, IEnumerable<string>>();
            user.Attributes.Add("cc3_portalusername", [userDto.PortalUserName]);

            var updated = await _client.UpdateUserAsync(_realm, user.Id, user);

            if (updated)
            {
                _logger.LogInformation("Attribute cc3_portalusername added to user {user}", user.UserName);
                return;
            }
            _logger.LogWarning("Failed to add attribute cc3_portalusername to user {user}", user.UserName);
        }
    }
}
