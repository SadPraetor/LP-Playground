using AwesomeAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;
using Xunit.Abstractions;

namespace JsonPatchExploration;

public class Tests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Tests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    [Fact]
    public void Test1()
    {
        var customAdapter = new CustomAdapter(new DefaultContractResolver(), (error) => { _testOutputHelper.WriteLine("failed"); throw new InvalidOperationException(); }, new CustomAdapterFactory());

        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Department = "Sales",
            DateOfBirth = new DateOnly(1985, 2, 20)
        };

        string jsonPatchLiteral = @"
                [
                    { ""op"": ""replace"", ""path"": ""/Department"", ""value"": ""Marketing"" },
{ ""op"": ""replace"", ""path"": ""/FirstName"", ""value"": ""Hans"" }
                ]";

        JsonPatchDocument<Person> patchDoc =
            JsonConvert.DeserializeObject<JsonPatchDocument<Person>>(jsonPatchLiteral);

        try
        {
            patchDoc.ApplyTo(person,
                customAdapter, (error) => throw new Exception("error in patching"));
        }
        catch (Exception e)
        {
            _testOutputHelper.WriteLine($"Error: {e}");
        }

        person.Department
            .Should()
            .Be("Marketing");

        person.FirstName
            .Should()
            .Be("John");
        // Expected output: After patch, Department = Marketing
    }
}

public class CustomAdapter : ObjectAdapter
{
    public CustomAdapter(IContractResolver contractResolver, Action<JsonPatchError> logErrorAction, IAdapterFactory adapterFactory) : base(contractResolver, logErrorAction, adapterFactory)
    {
    }
}

public class CustomAdapterFactory : IAdapterFactory
{
    public IAdapter Create(object target, IContractResolver contractResolver)
    {
        var jsonContract = contractResolver.ResolveContract(target.GetType());

        if (target is JObject)
        {
            return new JObjectAdapter();
        }
        if (target is IList)
        {
            return new ListAdapter();
        }
        else if (jsonContract is JsonDictionaryContract jsonDictionaryContract)
        {
            var type = typeof(DictionaryAdapter<,>).MakeGenericType(jsonDictionaryContract.DictionaryKeyType, jsonDictionaryContract.DictionaryValueType);
            return (IAdapter)Activator.CreateInstance(type);
        }
        else if (jsonContract is JsonDynamicContract)
        {
            return new DynamicObjectAdapter();
        }
        else
        {
            return new CustomPocoAdapter();
        }
    }
}

public class CustomPocoAdapter : PocoAdapter
{
    public override bool TryReplace(object target, string segment, IContractResolver contractResolver, object value, out string errorMessage)
    {
        if (!TryGetJsonProperty(target, contractResolver, segment, out var jsonProperty))
        {
            errorMessage = "error";
            return false;
        }

        var patchable = target.GetType().GetProperty(jsonProperty.PropertyName).GetCustomAttribute(typeof(PatchableAttribute), true);

        if (patchable is null)
        {
            errorMessage = "Property not patchable";
            return false;
        }

        return base.TryReplace(target, segment, contractResolver, value, out errorMessage);
    }
}
