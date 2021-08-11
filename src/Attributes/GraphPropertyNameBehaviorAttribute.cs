using System;
using Client.Translator.Behavior;

namespace Client.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class GraphPropertyNameBehaviorAttribute : Attribute
	{

		public GraphPropertyNameBehaviorAttribute(TranslatorBehavior translatorBehavior)
		{
			TranslatorBehavior = translatorBehavior;
		}
		
		public TranslatorBehavior TranslatorBehavior { get; }
		
	}
}