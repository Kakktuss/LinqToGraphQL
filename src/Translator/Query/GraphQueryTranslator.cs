using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqToGraphQL.Attributes;
using LinqToGraphQL.Extensions;
using LinqToGraphQL.Json;
using LinqToGraphQL.Set.Configuration;
using LinqToGraphQL.Translator.Details;
using Newtonsoft.Json;

namespace LinqToGraphQL.Translator.Query
{
    public class GraphQueryTranslator
    {

        private readonly Dictionary<string, InputDetail> _inputs;

        public GraphQueryTranslator()
        {
            _inputs = new();
        }

        internal string Translate(GraphSetQueryConfiguration graphSetQueryConfiguration, List<IncludeDetail> includeDetails)
        {
            return JsonConvert.SerializeObject(new QueryDetail
            {
                Query = TranslateEntireQuery(graphSetQueryConfiguration, includeDetails),
                Variables = _inputs.ToDictionary(p => p.Key, p => p.Value.VariableValue)
            }, 
                new JsonSerializerSettings
                {
                    ContractResolver = new GraphSerializerContractResolver()
                });
        }
        
        internal string TranslateEntireQuery(GraphSetQueryConfiguration graphSetQueryConfiguration, List<IncludeDetail> includeDetails)
        {
            var queryTemplate = "{0} {1}{{ result: {2}{3} {{ {4} }} }}";

            string typeDefinition = "query";

            if (graphSetQueryConfiguration.Type == GraphSetTypes.Mutation)
            {
                typeDefinition = "mutation";
            }

            var queryName = graphSetQueryConfiguration.Name;

            var queryInputs = "";

            if (graphSetQueryConfiguration.Arguments.Any(e => e.Value.Item2 is not null))
            {
                queryInputs += "(";
                
                foreach (((var queryArgumentName, (var queryArgumentPropertyInfo, var queryArgumentValue)), var queryArgumentIndex) in graphSetQueryConfiguration.Arguments.Select((item, index) => (item, index)))
                {
                    var inputName = $"{queryArgumentName}";

                    queryInputs += $"{inputName}:${inputName}{(queryArgumentIndex != graphSetQueryConfiguration.Arguments.Count - 1 ? ", " : "")}";
                
                    _inputs.Add(inputName, new InputDetail(inputName, queryArgumentValue.GetType(), queryArgumentName, queryArgumentValue, queryArgumentPropertyInfo));
                }
                
                queryInputs += ")";
            }
            
            string subIncludes = TranslateSubIncludes(includeDetails);

            string variableDefinitions = "";

            if (_inputs.Any())
            {
                variableDefinitions += "(";
                foreach (((string inputName, InputDetail inputDetail), var inputArgumentIndex) in _inputs.Select((item, index) => (item, index)))
                {
                    var inputDetailType = inputDetail.Type.ToGraphQlType();
                    
                    AttributesParserHelper.CheckMethodParameterTypeAttributes(ref inputDetailType, inputDetail.Parameter);
                    
                    variableDefinitions += $"${inputName}:{inputDetailType}{(inputArgumentIndex != _inputs.Count - 1 ? ", " : "")}";

                }
                variableDefinitions += ")";
            }

            return string.Format(queryTemplate, typeDefinition, variableDefinitions, queryName, queryInputs, subIncludes);
        }
        
        internal string TranslateSubIncludes(List<IncludeDetail> includeDetails)
        {

            if (!includeDetails.Any())
            {
                return "";
            }

            var currentQuery = "";
            
            foreach ((var includeDetail, var includeDetailIndex) in includeDetails.Select((item, index) => (item, index)))
            {
                if (includeDetail.Attribute is MethodInfo methodInfo)
                {
                    var includeTemplate = "{0}{1} {{ {2} }}";

                    var currentInputs = "";

                    if (includeDetail.Inputs.Any())
                    {
                        currentInputs += "(";
                        
                        foreach ((var inputDetail, var inputDetailIndex) in includeDetail.Inputs.Select((item, index) => (item, index)))
                        {
                            var inputName = $"{inputDetail.Name}";

                            AttributesParserHelper.CheckMethodParameterNameAttributes(ref inputName, inputDetail.Parameter);
                            
                            currentInputs += $"{inputName}:${inputDetail.Name}{(inputDetailIndex != includeDetail.Inputs.Count - 1 ? ", " : "")}";

                            _inputs.Add(inputDetail.Name, inputDetail);
                        }
                        
                        currentInputs += ")";
                    }

                    var includeDetailName = includeDetail.Name;
                    
                    AttributesParserHelper.CheckMethodNameAttributes(ref includeDetailName, methodInfo);
                    
                    currentQuery += string.Format(includeTemplate, includeDetailName, currentInputs, TranslateSubIncludes(includeDetail.Includes));
                } else if (includeDetail.Attribute is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType.Name is not "String")
                    {
                        if (propertyInfo.PropertyType.IsGenericType)
                        {
                            var propertyGenericArgument = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

                            if (propertyGenericArgument is not null && (propertyGenericArgument.IsPrimitive || propertyGenericArgument.Name is "String"))
                            {
                                var genericIncludeDetailName = includeDetail.Name;
                                
                                AttributesParserHelper.CheckPropertyNameAttributes(ref genericIncludeDetailName, propertyInfo);
                                
                                currentQuery += $"{genericIncludeDetailName}{(includeDetailIndex != includeDetails.Count - 1 ? ", " : "")}";

                                continue;
                            }
                        } else if (propertyInfo.PropertyType.IsEnum)
                        {
                            var enumIncludeDetailName = includeDetail.Name;
                            
                            AttributesParserHelper.CheckPropertyNameAttributes(ref enumIncludeDetailName, propertyInfo);
                                
                            currentQuery += $"{enumIncludeDetailName}{(includeDetailIndex != includeDetails.Count - 1 ? ", " : "")}";

                            continue;
                        }

                        var includeTemplate = "{0} {{ {1} }}";
                        
                        var nonPrimitiveIncludeDetailName = includeDetail.Name;

                        AttributesParserHelper.CheckPropertyNameAttributes(ref nonPrimitiveIncludeDetailName, propertyInfo);
                        
                        currentQuery += $"{string.Format(includeTemplate, nonPrimitiveIncludeDetailName, TranslateSubIncludes(includeDetail.Includes))}{(includeDetailIndex != includeDetails.Count - 1 ? ", " : "")}";
                        
                        continue;
                    }
                    var includeDetailName = includeDetail.Name;

                    AttributesParserHelper.CheckPropertyNameAttributes(ref includeDetailName, propertyInfo);

                    currentQuery += $"{includeDetailName}{(includeDetailIndex != includeDetails.Count - 1 ? ", " : "")}";
                }
            }
            
            return currentQuery;
        }

    }
}