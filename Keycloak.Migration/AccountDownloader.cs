using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Keycloak.Migration
{
    public class AccountDownloader
    {
        private readonly CC3DbContext _context;

        public AccountDownloader(CC3DbContext context)
        {
            _context = context;
        }

        public async Task ExportAccounts(string filename)
        {
            var userDtos = await _context.Accounts                
                .Select(x=> new UserDto(x.PortalUserName,x.Identity.FirstName,x.Identity.LastName,x.Identity.Email))
                .ToArrayAsync();

            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
            {
                filename = filename + ".json";
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            using var stream = new FileStream(filename, FileMode.OpenOrCreate);

            foreach(var user in userDtos)
            {
                await JsonSerializer.SerializeAsync(stream, user);
                //await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.SerializeAsync( user));
                stream.WriteByte(Convert.ToByte('\r'));
                stream.WriteByte(Convert.ToByte('\n'));
            }

            await stream.FlushAsync();
            
        }
    }
}
