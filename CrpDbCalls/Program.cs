// See https://aka.ms/new-console-template for more information
using CrpDbCalls;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var context = new CrpContext("Data Source=localhost; Database=crp-test; User ID=SA;Password=superStrongPassw0rd;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");


//var template = context.Templates
//    .FirstOrDefault(x => x.Id == 999);

var template = context.Templates
    .ToArray()
    .MaxBy(x => System.Text.ASCIIEncoding.Unicode.GetByteCount(x.Content));



var size = System.Text.ASCIIEncoding.Unicode.GetByteCount(template.Content);

Console.WriteLine(size);
Console.WriteLine();
Console.WriteLine("breakpoint");


void AddTemplate(Template template)
{
    var content = template.Content + template.Content;

    template = GetTemplate(content);

    using var transaction = context.Database.BeginTransaction();

    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Templates ON;");

    context.Add(template);
    context.SaveChanges();
    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Templates OFF;");
    transaction.Commit();

}

Template GetTemplate(string content)
{
    string culture = "cs";
    return new Template()
    {
        Id = 999,
        Culture = culture,
        Name = "DS - Potvrzený zájem vozidlo na ODP - FO",
        Order = 53,
        IsDeleted = false,
        ApplicationType = ApplicationType.EmailBuyReq,
        SubjectType = SaleSubjectType.Individual,

        Content = JsonSerializer.Serialize(new EmailMessage()
        {
            Culture = culture,
            From = "carremarketing@drivalia.com",
            To = "{Vehicle.Redeemer.Email}",
            Subject = "Žádost o odkup vozidla {Vehicle.LicensePlate}",
            Body = new StoreDocument()
            {
                Def = new[]
                {
                    new StorePage()
                    {
                        Content =
                            content
                    }
                }
            }
        })

    };
}