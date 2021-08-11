using System.Collections.Generic;
using System.ComponentModel;

namespace LinqToGraphQL.Types
{
	public class TypeSystemHelper
	{
		internal static System.Type GetElementType(System.Type seqType)
		{
			System.Type ienum = FindIEnumerable(seqType);
			if (ienum == null) return seqType;
			return ienum.GetGenericArguments()[0];
		}

		private static System.Type FindIEnumerable(System.Type seqType) 
		{
			if (seqType == null || seqType == typeof(string))
				return null;

			if (seqType.IsArray)
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

			if (seqType.IsGenericType)
			{
				foreach (System.Type arg in seqType.GetGenericArguments())
				{
					System.Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
					if (ienum.IsAssignableFrom(seqType))
						return ienum;
				}
			}

			System.Type[] ifaces = seqType.GetInterfaces();

			if (ifaces != null && ifaces.Length > 0)
			{
				foreach (System.Type iface in ifaces) 
				{
					System.Type ienum = FindIEnumerable(iface);

					if (ienum != null)
						return ienum;
				}
			}

			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return FindIEnumerable(seqType.BaseType);
			}

			return null;
		}
		
		public static bool TryCast<T>(object obj, out T result)
		{
			result = default(T);
			if (obj is T)
			{
				result = (T)obj;
				return true;
			}

			// If it's null, we can't get the type.
			if (obj != null)
			{
				var converter = TypeDescriptor.GetConverter(typeof (T));
				if(converter.CanConvertFrom(obj.GetType()))
					result = (T) converter.ConvertFrom(obj);
				else
					return false;

				return true;
			}

			//Be permissive if the object was null and the target is a ref-type
			return !typeof(T).IsValueType; 
		}


	}
}