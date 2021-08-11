using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
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
			
			if (graphNameAttribute is not null)
			{
				property.PropertyName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
			}

			return property;
		}
	}
}