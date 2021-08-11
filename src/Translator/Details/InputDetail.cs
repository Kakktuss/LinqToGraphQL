using System;
using System.Reflection;

namespace LinqToGraphQL.Translator.Details
{
	public class InputDetail
	{
		public string Name { get; set; }
		
		public Type Type { get; set; }
		
		public string VariableName { get; set; }
		
		public object VariableValue { get; set; }
		
		public ParameterInfo Parameter { get; set; }

		public InputDetail(string name, Type type, string variableName, object variableValue, ParameterInfo parameter = null)
		{
			Name = name;

			Type = type;

			VariableName = variableName;

			VariableValue = variableValue;

			Parameter = parameter;
		}
	}
}