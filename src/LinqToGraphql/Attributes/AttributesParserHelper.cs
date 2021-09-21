using System;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Extensions;
using LinqToGraphQL.Translator.Behavior;

namespace LinqToGraphQL.Attributes
{
	public class AttributesParserHelper
	{
		internal static void CheckMethodNameAttributes(ref string nodeName, MethodInfo methodInfo)
        {
            var graphNameAttribute = methodInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameAttribute));

            var graphNameTranslatorBehaviorAttribute = methodInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameBehaviorAttribute));
							
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
            var graphNameAttribute = parameterInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameAttribute));

            var graphNameTranslatorBehaviorAttribute = parameterInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameBehaviorAttribute));
            
            if (graphNameAttribute is { })
            {
                nodeName = (string) graphNameAttribute.ConstructorArguments.FirstOrDefault().Value;
            }

            if (graphNameTranslatorBehaviorAttribute is { })
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
            var graphPropertyTypeAttribute = parameterInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphParameterTypeAttribute));
            
            var graphPropertyTypeNameAttribute = parameterInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphParameterTypeNameAttribute));

            var graphNonNullablePropertyAttribute = parameterInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNonNullableParameterAttribute));
            
            if (graphPropertyTypeAttribute is { })
            {
                type = ((Type) graphPropertyTypeAttribute.ConstructorArguments.FirstOrDefault().Value)?.Name;
            }
            
            if (graphPropertyTypeNameAttribute is { })
            {
                type = ((Type) graphPropertyTypeNameAttribute.ConstructorArguments.FirstOrDefault().Value)?.Name;
            }
            
            if (graphNonNullablePropertyAttribute is { })
            {
                type = $"{type}!";
            }
        }

        internal static void CheckPropertyNameAttributes(ref string nodeName, PropertyInfo propertyInfo)
        {
            var graphNameAttribute = propertyInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameAttribute));

            var graphNameTranslatorBehaviorAttribute = propertyInfo.GetCustomAttributesData().FirstOrDefault(e => e.AttributeType == typeof(GraphNameBehaviorAttribute));

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