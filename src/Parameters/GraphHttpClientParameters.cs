namespace Client.Parameters
{
	public class GraphHttpClientParameters
	{
		public string Authorization { get; set; } = null;

		internal bool HasAuthorization => !string.IsNullOrEmpty(Authorization);

		public string RequestUri { get; set; } = "";
	}
}