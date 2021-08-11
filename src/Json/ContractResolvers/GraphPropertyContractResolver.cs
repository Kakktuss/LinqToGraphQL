using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinqToGraphQL.Json
{
	public class GraphPropertyNameContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			var graphNameAttribute = member.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameAttribute));

			var graphUnionTypeAttribute = member.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphUnionTypePropertyAttribute));

			if (graphNameAttribute is not null)
			{
				property.PropertyName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
			}

			if (graphUnionTypeAttribute is not null)
			{
				var unionTypes = new List<Type>();
				
				foreach (var graphUnionType in (IReadOnlyCollection<CustomAttributeTypedArgument>) graphUnionTypeAttribute.ConstructorArguments.FirstOrDefault().Value)
				{
					unionTypes.Add((Type) graphUnionType.Value);
				}
				
				property.Converter = new GraphPropertyUnionTypeConverter(unionTypes);
			}

			return property;
		}
	}
}