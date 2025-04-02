using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Keycloak.Migration
{
    public class SetPasswordHandler
    {
        private const string _realm = "drivalia";
        private readonly KeycloakClient _client;
        private readonly ILogger<UpdatePasswordActionHandler> _logger;

        public SetPasswordHandler(Keycloak.Net.KeycloakClient client,
            ILogger<UpdatePasswordActionHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task SetNewPasswordAsync(string newPassword)
        {
            List<UserDto> userDtos = new();
            using (FileStream fileStream = File.OpenRead("accounts-test-v2-migration.json"))
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

                            _logger.LogInformation("Processing {user}", userDto.PortalUserName);

                            if (!userDto.UserCreatedByProcess.GetValueOrDefault())
                            {
                                _logger.LogInformation("Skipping set password for user {user}", userDto.PortalUserName);
                                continue;
                            }


                            var user = await FindUserAsync(userDto.PortalUserName);

                            if (user is null)
                            {
                                _logger.LogWarning("Failed to find user {portalusername}", userDto.PortalUserName);
                                continue;
                            }
                            //var response = await _client.ResetUserPasswordAsync(_realm, user.Id, newPassword, true);
                            var response = await _client.SetUserPasswordAsync(_realm, user.Id, newPassword);
                            if (response.Success)
                            {
                                _logger.LogInformation("Password set for user {user} password {password}" , userDto.PortalUserName,newPassword);
                                userDto.TemporaryGeneratedPassword = newPassword;
                                userDtos.Add(userDto);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogWarning("Failed to process line");
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
