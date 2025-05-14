using System.Text.Json.Serialization;
using System.Text.Json;

public class MessageBase : BaseModel
{
    public string? Name { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public SaleSubjectType SubjectType { get; set; }

    public string Content { get; set; }

    public T? ParseContent<T>()
    {
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());

        var parsed = JsonSerializer.Deserialize<T>(Content, options);

        return parsed;
    }
}

public class BaseModel : BaseModel<int>
{ }

public class BaseModel<TId>
{   
    public TId Id { get; set; }

    public bool IsDeleted { get; set; }
}