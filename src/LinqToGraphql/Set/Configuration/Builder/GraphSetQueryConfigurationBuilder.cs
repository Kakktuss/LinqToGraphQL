namespace LinqToGraphQL.Set.Configuration.Builder
{
	public class GraphSetQueryConfigurationBuilder
	{
		protected GraphSetTypes Type;

		public GraphSetQueryConfigurationBuilder WithType(GraphSetTypes type)
		{
			Type = type;

			return this;
		}
		
		internal GraphSetQueryConfiguration Build()
		{
			return new GraphSetQueryConfiguration(Type);
		}
	}
}