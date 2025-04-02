// See https://aka.ms/new-console-template for more information
using Keycloak.Migration;
using System.Text.Json;

Console.WriteLine("Hello, World!");

List<string> prodList = new();
List<string> testList = new();

using (FileStream fileStream = File.OpenRead("accounts-prod-v2-created-password_set.json"))
{
    using StreamReader reader = new StreamReader(fileStream);

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

		try
		{
			if (string.IsNullOrEmpty(line))
			{
                var userDto = JsonSerializer.Deserialize<UserDto>(line);

                if (userDto is null)
                {
                    //fs_logger.LogWarning("Failed to deserialize line {line} into UserDto", line);
                    continue;
                }
                prodList.Add(userDto.PortalUserName);
            }
		}
		catch (Exception)
		{

			throw;
		}
    }
}