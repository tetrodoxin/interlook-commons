using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Interlook.Text
{
	public static class ExceptionExtensions
	{
		private const int DEFAULT_INNER_EXCEPTION_DEPTH = 10;

		public static string ToCompleteString(this Exception ex)
		{
			return ToCompleteString(ex, DEFAULT_INNER_EXCEPTION_DEPTH);
		}

		public static string ToCompleteString(this Exception ex, int maxDepth)
		{
			return exceptionString(ex, new List<Exception>(), 0, maxDepth);
		}

		private static string exceptionString(Exception ex, ICollection<Exception> traversed, int deep, int maxDepth)
		{
			if (ex == null)
			{
				return String.Empty;
			}
			else if (deep >= maxDepth)
			{
				return "Reached max recursion depth for inner exceptions.";
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine(ex.ToString());
				traversed.Add(ex);
				if (ex.InnerException != null && ex != ex.InnerException && !traversed.Contains(ex))
				{
					var inner = exceptionString(ex.InnerException, traversed, deep + 1, maxDepth);
					if (!inner.IsNullOrEmpty())
					{
						sb.AppendLine();
						sb.AppendLine("---INNER EXCEPTION:");
						sb.AppendLine(inner);
					}
				}

				return sb.ToString();
			}
		}
	}
}