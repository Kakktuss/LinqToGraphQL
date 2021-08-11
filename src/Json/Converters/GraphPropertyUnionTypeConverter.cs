using System;
using System.Collections.Generic;
using LinqToGraphQL.Extensions;
using LinqToGraphQL.Set.Configuration.Builder;
using LinqToGraphQL.Types;
using Newtonsoft.Json;

namespace LinqToGraphQL.Json.Converters
{
	public class GraphPropertyUnionTypeConverter : JsonConverter
	{
		private readonly List<Type> _unionTypes;
		
		public GraphPropertyUnionTypeConverter(List<Type> unionTypes)
		{
			_unionTypes = unionTypes;
		}
		
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			foreach (var unionType in _unionTypes)
			{
				var tryCastParameters = new object?[]
				{
					reader.Value,
					null
				};
				
				var tryCast = (bool) typeof(TypeSystemHelper).GetMethod(nameof(TypeSystemHelper.TryCast)).MakeGenericMethod(unionType).Invoke(null, tryCastParameters);
				
				if (tryCast)
				{
					return tryCastParameters[1];
				}
			}

			return reader.Value;
		}

		public override bool CanWrite => false;

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}