using System.Text.Json;

#nullable enable
namespace Client.Extensions
{
	internal static class JsonSerializerExtensions
	{
		internal static bool TryDeserialize<T>(string json, out T? result)
		{
			try
			{
				result = System.Text.Json.JsonSerializer.Deserialize<T>(json);

				return true;
			}
			catch (JsonException exception)
			{
				result = default(T);
				
				return false;
			}
		}
		
	}
}
#nullable disable