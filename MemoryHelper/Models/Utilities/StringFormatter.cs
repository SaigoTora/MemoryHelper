using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryHelper.Models.Utilities
{
	internal class StringFormatter
	{
		private const int INDENT = 4;
		private const char HORIZONTAL_BOUND_CHAR = '-';
		private const char VERTICAL_BOUND_CHAR = '|';

		internal static string FormatStrings(IEnumerable<string> lines, string caption = "", int startIndex = 1)
		{
			if (lines == null)
				throw new ArgumentNullException(nameof(lines));
			if (caption == null)
				throw new ArgumentNullException(nameof(caption));

			if (!string.IsNullOrEmpty(caption))
				caption += '\n';
			StringBuilder builder = new StringBuilder(caption);

			foreach (string line in lines)
				builder.Append($"{startIndex++}) {line}\n");

			return FormatString(builder.ToString().TrimEnd('\n'), false);
		}
		internal static string FormatString(string textToFormat, bool needToCenter = true)
		{
			if (textToFormat == null)
				throw new ArgumentNullException(nameof(textToFormat));

			StringBuilder result = new StringBuilder();
			string[] arr = textToFormat.Replace("\r", "").Split('\n');
			int maxLength = GetLengthOfTheLongestString(arr) + INDENT;

			result.AppendLine(new string(HORIZONTAL_BOUND_CHAR, maxLength));// Upper bound
			result.AppendLine(VERTICAL_BOUND_CHAR + string.Empty.PadRight(maxLength - 2) + VERTICAL_BOUND_CHAR);

			for (int i = 0; i < arr.Length; i++)
			{// Main part
				if (needToCenter)// Center alignment
					result.AppendLine(GetTextCenterAlign(arr[i], maxLength));
				else// Left alignment
					result.AppendLine(GetTextLeftAlign(arr[i], maxLength));
			}

			result.AppendLine(VERTICAL_BOUND_CHAR + string.Empty.PadRight(maxLength - 2) + VERTICAL_BOUND_CHAR);
			result.Append(new string(HORIZONTAL_BOUND_CHAR, maxLength));// Lower bound

			return result.ToString();
		}

		private static int GetLengthOfTheLongestString(string[] arr)
		{
			if (arr == null)
				throw new ArgumentNullException(nameof(arr));

			int result = 0;
			for (int i = 0; i < arr.Length; i++)
				result = Math.Max(result, arr[i].Length);

			return result;
		}
		private static string GetTextCenterAlign(string str, int maxLength)
		{
			if (str == null)
				throw new ArgumentNullException(nameof(str));

			int centerIndent = (maxLength - str.Length) / 2 - 1;
			return GetText(str, centerIndent, maxLength - INDENT / 2 - centerIndent);
		}
		private static string GetTextLeftAlign(string str, int maxLength)
		{
			if (str == null)
				throw new ArgumentNullException(nameof(str));

			return GetText(str, INDENT / 2 - 1, maxLength - INDENT / 2 - 1);
		}
		private static string GetText(string str, int paddingLeft, int paddingRight)
		{
			return VERTICAL_BOUND_CHAR +
				string.Empty.PadRight(paddingLeft) + str.PadRight(paddingRight) +
				VERTICAL_BOUND_CHAR;
		}
	}
}