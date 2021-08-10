using System.Linq;
using System.Reflection;
using Client.Extensions;
using Client.Translator.Behavior;

namespace Client.Attributes
{
	public class AttributesParserHelper
	{
		internal static void CheckMethodNameAttributes(ref string nodeName, MethodInfo methodInfo)
        {
            var graphNameAttribute = methodInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameAttribute));

            var graphNameTranslatorBehaviorAttribute = methodInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameBehaviorAttribute));
							
            if (graphNameAttribute != null)
            {
                nodeName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
            }

            if (graphNameTranslatorBehaviorAttribute != null)
            {
                var graphNameTranslatorBehavior = (TranslatorBehavior) graphNameTranslatorBehaviorAttribute.ConstructorArguments.FirstOrDefault().Value;

                if (graphNameTranslatorBehavior == TranslatorBehavior.CamelCase)
                {
                    nodeName = nodeName.ToCamel();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.LowerCase)
                {
                    nodeName = nodeName.ToLower();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.UpperCase)
                {
                    nodeName = nodeName.ToUpper();
                }
            }
        }
        
        internal static void CheckMethodParameterNameAttributes(ref string nodeName, ParameterInfo parameterInfo)
        {
            var graphNameAttribute = parameterInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameAttribute));

            var graphNameTranslatorBehaviorAttribute = parameterInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameBehaviorAttribute));
            
            if (graphNameAttribute is not null)
            {
                nodeName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
            }

            if (graphNameTranslatorBehaviorAttribute is not null)
            {
                var graphNameTranslatorBehavior = (TranslatorBehavior) graphNameTranslatorBehaviorAttribute.ConstructorArguments.FirstOrDefault().Value;

                if (graphNameTranslatorBehavior == TranslatorBehavior.CamelCase)
                {
                    nodeName = nodeName.ToCamel();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.LowerCase)
                {
                    nodeName = nodeName.ToLower();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.UpperCase)
                {
                    nodeName = nodeName.ToUpper();
                }
            }
        }
        
        internal static void CheckMethodParameterTypeAttributes(ref string type, ParameterInfo parameterInfo)
        {
            var graphNonNullablePropertyAttribute = parameterInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphNonNullablePropertyAttribute));
            
            if (graphNonNullablePropertyAttribute is not null)
            {
                type = $"{type}!";
            }
        }

        internal static void CheckPropertyNameAttributes(ref string nodeName, PropertyInfo propertyInfo)
        {
            var graphNameAttribute = propertyInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameAttribute));

            var graphNameTranslatorBehaviorAttribute = propertyInfo.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(GraphPropertyNameBehaviorAttribute));
            
            if (graphNameAttribute != null)
            {
                nodeName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
            }

            if (graphNameTranslatorBehaviorAttribute != null)
            {
                var graphNameTranslatorBehavior = (TranslatorBehavior) graphNameTranslatorBehaviorAttribute.ConstructorArguments.FirstOrDefault().Value;

                if (graphNameTranslatorBehavior == TranslatorBehavior.CamelCase)
                {
                    nodeName = nodeName.ToCamel();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.LowerCase)
                {
                    nodeName = nodeName.ToLower();
                } else if (graphNameTranslatorBehavior == TranslatorBehavior.UpperCase)
                {
                    nodeName = nodeName.ToUpper();
                }
            }
        }
    }
}