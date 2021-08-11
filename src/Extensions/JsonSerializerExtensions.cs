using Client.Json;
using Newtonsoft.Json;

#nullable enable
namespace Client.Extensions
{
	internal static class JsonSerializerExtensions
	{
		internal static bool TryDeserialize<T>(string json, out T? result)
		{
			try
			{
				result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
				{
					ContractResolver = new GraphPropertyNameContractResolver()
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