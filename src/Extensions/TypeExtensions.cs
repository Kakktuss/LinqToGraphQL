using System;

namespace Client.Extensions
{
	public static class TypeExtensions
	{
		internal static string ToGraphQlType(this Type type)
		{
			if (type == typeof(bool))
			{
				return "Boolean";
			}

			if (type == typeof(int))
			{
				return "Int";
			}

			if (type == typeof(string))
			{
				return "String";
			}

			if (type == typeof(float))
			{
				return "Float";
			}

			return type.Name;
		}
	}
}