using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinqToGraphQL.Json.Converters
{
	public class GraphInputVariableConverter : JsonConverter
	{
		public GraphInputVariableConverter()
		{
		}

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			if (value is Dictionary<string, object> valuesDictionary)
			{
				writer.WriteStartObject();

				foreach ((string keyName, object keyValue) in valuesDictionary)
				{
					var valueType = keyValue.GetType();
					
					writer.WritePropertyName(keyName);
					
					// Check if the type is not a primitive or a string
					if (!valueType.IsPrimitive && valueType.Name is not "String")
					{
						// Check if the type is a GenericType
						if (valueType.IsGenericType)
						{
							var jTokenArray = (JArray) JToken.FromObject(keyValue);
							
							_writeArray(in writer, jTokenArray, valueType.GetGenericArguments().FirstOrDefault());

							continue;
						} 
						
						_writeObject(in writer, keyValue);
					} else
					{
						writer.WriteValue(keyValue);
					}
				}
				
				writer.WriteEndObject();
			}
		}

		private void _writeArray(in JsonWriter writer, object value, Type parentType = null)
		{
			writer.WriteStartArray();
			
			var enumerableValues = value as IEnumerable;

			foreach (var enumerableValue in enumerableValues)
			{
				var tokenObject = (JObject) enumerableValue;
				
				_writeObject(in writer, tokenObject, parentType);
			}
							
			writer.WriteEndArray();
		}

		private void _writeObject(in JsonWriter writer, object value, Type parentType = null)
		{
			var token = JToken.FromObject(value);

			if (token.Type == JTokenType.Object)
			{
				var valueType = parentType ?? value.GetType();

				var objectDeclaredProperties = valueType.GetProperties();

				JObject tokenObject = (JObject) token;

				var tokenDeclaredProperties = tokenObject.Properties();

				writer.WriteStartObject();

				foreach (var declaredProperty in tokenDeclaredProperties)
				{
					var declaredPropertyName = declaredProperty.Name;

					var objectProperty = objectDeclaredProperties.FirstOrDefault(e => e.Name == declaredProperty.Name);

					if (objectProperty is { })
					{
						AttributesParserHelper.CheckPropertyNameAttributes(ref declaredPropertyName, objectProperty);

						if (declaredProperty.Value is JValue { Value: null })
						{
							continue;
						}

						writer.WritePropertyName(declaredPropertyName);

						if (declaredProperty.Value.Type == JTokenType.Object)
						{
							_writeObject(in writer, declaredProperty.Value, objectProperty.PropertyType);
						} else if (declaredProperty.Value.Type == JTokenType.Array)
						{
							_writeArray(in writer, declaredProperty.Value, objectProperty.PropertyType.GetGenericArguments().FirstOrDefault());
						} else
						{
							writer.WriteValue(declaredProperty.Value);
						}
					}
				}

				writer.WriteEndObject();
			}
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}

		public override bool CanRead => false;
	}
}