using System;
using System.Diagnostics;
using LinqToGraphQL.Json;
using Newtonsoft.Json;

#nullable enable
namespace LinqToGraphQL.Extensions
{
	internal static class JsonSerializerExtensions
	{
		internal static bool TryDeserialize<T>(string json, out T? result)
		{
			try
			{
				result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
				{
					ContractResolver = GraphPropertyNameContractResolver.Instance
				});
				
				return true;
			}
			catch (JsonException e)
			{
				result = default(T);
				
				return false;
			}
		}
		
	}
}
#nullable disable