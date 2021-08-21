using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Attributes.Method;
using LinqToGraphQL.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinqToGraphQL.Json
{
	public class GraphPropertyNameContractResolver : DefaultContractResolver
	{
		private readonly Dictionary<string, MemberInfo> _methodBackingFields = new();

		protected override List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			var serializableMembers = base.GetSerializableMembers(objectType);

			var objectMembers = objectType.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
			
			foreach (var member in objectMembers.Where(e => e is MethodInfo 
			                                                && !e.Name.StartsWith("get_") 
			                                                && !e.Name.StartsWith("set_") 
			                                                && !e.Name.StartsWith("k__")))
			{
				var graphDelegateValueAttribute = member.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphBackingFieldAttribute));

				if (graphDelegateValueAttribute is not null)
				{
					var propertyBackingField = objectMembers.Where(e => e.Name == (string) graphDelegateValueAttribute.ConstructorArguments.FirstOrDefault().Value)?.FirstOrDefault();

					if (propertyBackingField is not null)
					{
						_methodBackingFields.Add(propertyBackingField.Name, member);
					
						serializableMembers.Add(propertyBackingField);
					}
				}
			}
			
			return serializableMembers;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if (_methodBackingFields.TryGetValue(property.PropertyName, out var originalMethod))
			{
				property.PropertyName = originalMethod.Name;

				member = originalMethod;
			}
			
			var graphNameAttribute = member.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameAttribute));

			var graphUnionTypeAttribute = member.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphUnionTypePropertyAttribute));

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