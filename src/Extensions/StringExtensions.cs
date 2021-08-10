namespace Client.Extensions
{
	public static class StringExtensions
	{
		internal static string ToCamel(this string input)
		{
			if (char.IsLower(input[0]))
			{
				return input;
			}
			return input.Substring(0, 1).ToLower() + input.Substring(1);
		}
	}
}