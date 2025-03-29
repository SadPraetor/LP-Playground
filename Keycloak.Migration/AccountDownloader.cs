using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IO;

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
            var accounts = await _context.Accounts
                .ToArrayAsync();

            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
            {
                filename = filename + ".txt";
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            var stream = new FileStream(filename, FileMode.OpenOrCreate);

            foreach(var account in accounts)
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes(account.PortalUserName));
                stream.WriteByte(Convert.ToByte('\r'));
                stream.WriteByte(Convert.ToByte('\n'));
            }

            await stream.FlushAsync();
            
        }
    }
}
