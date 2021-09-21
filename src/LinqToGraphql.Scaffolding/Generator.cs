using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using GraphQLinq.Scaffolding;
using Microsoft.CodeAnalysis;

namespace ConsoleApp1
{
	public class Generator
	{
		List<string> usings = new() { "System", "System.Collections.Generic" };

		private Dictionary<string, string> renamedClasses = new();
		private readonly CodeGenerationOptions options;

		private static readonly Dictionary<string, (string Name, Type type)> TypeMapping = new(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "Int", new("int", typeof(int)) },
			{ "Float", new("float", typeof(float)) },
			{ "String", new("string", typeof(string)) },
			{ "ID", new("string", typeof(string)) },
			{ "Date", new("DateTime", typeof(DateTime)) },
			{ "Boolean", new("bool", typeof(bool)) },
			{ "Long", new("long", typeof(long)) },
			{ "uuid", new("Guid", typeof(Guid)) },
			{ "timestamptz", new("DateTimeOffset", typeof(DateTimeOffset)) },
			{ "Uri", new("Uri", typeof(Uri)) }
		};
		
		private static readonly List<string> BuiltInTypes = new()
		{
			"ID",
			"Int",
			"Float",
			"String",
			"Boolean"
		};

		private SyntaxNode GenerateEnum(GraphqlType enumInfo)
		{
			
		}
	}
}