using System.Collections.Generic;
using System.Reflection;

namespace Client.Translator.Details
{
	public class IncludeDetail
	{
		public string Name { get; set; }
		
		public List<IncludeDetail> Includes { get; set; }

		public List<InputDetail> Inputs { get; set; }
		
		public ICustomAttributeProvider Attribute { get; set; }

		public IncludeDetail(string name,
			ICustomAttributeProvider attribute)
		{
			Name = name;
			
			Includes = new List<IncludeDetail>();

			Inputs = new List<InputDetail>();

			Attribute = attribute;
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