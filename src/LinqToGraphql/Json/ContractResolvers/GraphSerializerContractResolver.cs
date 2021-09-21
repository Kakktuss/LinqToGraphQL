using System;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinqToGraphQL.Json
{
	public class GraphSerializerContractResolver : DefaultContractResolver
	{
		
		public static readonly GraphSerializerContractResolver Instance = new GraphSerializerContractResolver();

		protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			var dictionaryContract = base.CreateDictionaryContract(objectType);

			dictionaryContract.Converter = new GraphInputVariableConverter();

			return dictionaryContract;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			property.Converter = new GraphInputVariableConverter();
			
			return base.CreateProperty(member, memberSerialization);
		}
	}
}