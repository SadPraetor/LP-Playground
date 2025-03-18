// See https://aka.ms/new-console-template for more information
using FirstCallsDb;

Console.WriteLine("Hello, World!");

var context = new CrpContext("Data Source=localhost; Database=crp-test; User ID=SA;Password=superStrongPassw0rd;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

var checkItems = context.Checklist.Where(c => c.VehicleId == 13)
    .ToList();
//provola db
var test = context.Checklist.FirstOrDefault(c => c.VehicleId == 13 && c.DefinitionId == ChecklistItemCode.TransferProtocol);

var checkItem = checkItems.FirstOrDefault(c => c.DefinitionId == ChecklistItemCode.TransferProtocol);

Console.WriteLine(checkItem.Checked);
checkItem.Checked = !checkItem.Checked;
//provola db
test = context.Checklist.FirstOrDefault(c => c.VehicleId == 13 && c.DefinitionId == ChecklistItemCode.TransferProtocol);


var test2 = context.Checklist.Find(test.Id);

Console.WriteLine();
Console.WriteLine("breakpoint");