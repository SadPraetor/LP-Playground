// See https://aka.ms/new-console-template for more information
using Keycloak.Migration;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

Console.WriteLine("Hello, World!");




var command = new SqlCommand(@"INSERT INTO dbo.Checklist
                    (Checked, VehicleId, DefinitionId, Active, Activated, IsDeleted)
                    SELECT
                        0 AS Checked,
                        v.Id AS VehicleId,
                        80 AS DefinitionId,
                        1 AS Active,
                        @timestamp AS Activated,
                        0 AS IsDeleted
                    FROM dbo.Vehicle v
                    WHERE
                      v.Channel = 3"
                );

var para = new SqlParameter("@timestamp", DateTime.UtcNow);
command.Parameters.Add(para);


var test = command.CommandText;


Console.WriteLine();
Console.WriteLine("brakpoint");