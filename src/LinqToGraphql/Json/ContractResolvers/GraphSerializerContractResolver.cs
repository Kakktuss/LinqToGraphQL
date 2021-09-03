using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinqToGraphQL.Json
{
	public class GraphSerializerContractResolver : DefaultContractResolver
	{

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			var graphNameAttribute = member.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameAttribute));

			if (graphNameAttribute is not null)
			{
				property.PropertyName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
			}
			
			return base.CreateProperty(member, memberSerialization);
		}

	}
}