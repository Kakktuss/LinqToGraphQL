using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToGraphQL.Translator.Details
{
	public class IncludeDetail
	{
		public string Name { get; }

		public List<IncludeDetail> Includes { get; }

		public List<InputDetail> Inputs { get; }
		
		public ICustomAttributeProvider Attribute { get; }
		
		public Type Type { get; }

		public IncludeDetail(string name,
			ICustomAttributeProvider attribute,
			Type type)
		{
			Name = name;
			
			Includes = new List<IncludeDetail>();

			Inputs = new List<InputDetail>();

			Attribute = attribute;

			Type = type;
		}

		public void AddSubInclude(IncludeDetail includeDetail)
		{
			Includes.Add(includeDetail);
		}

		public void AddInput(InputDetail inputDetail)
		{
			Inputs.Add(inputDetail);
		}
	}
}