using System;
using System.Text;

namespace Dolkens.Framework.Utilities
{
    public static partial class StringUtilities
    {
        public static String StripTags(String input)
        {
            input = input.Replace(@"&nbsp;", " "); // TODO: Convert HTML entities back to raw strings
            Char[] array = new Char[input.Length];
            Int32 arrayIndex = 0;
            Boolean inside = false;
            Char let;

            for (Int32 i = 0; i < input.Length; i++)
            {
                let = input[i];

                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            return new String(array, 0, arrayIndex);
        }

        public static String TrimTo(String input, Int32 maxLength, Boolean stripTags = true)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                String cleanString = input;

                if (stripTags)
                    cleanString = cleanString.StripTags();

                if (cleanString.Length > maxLength)
                {
                    String[] parts = cleanString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    StringBuilder sb = new StringBuilder();
                    Int32 i = 1;
                    Int32 j = parts.Length;
                    if (parts.Length > 0) sb.Append(parts[0]);

                    while (i < j && (sb.Length + parts[i].Length < maxLength))
                    {
                        sb.Append(" ");
                        sb.Append(parts[i++]);
                    }

                    if (parts.Length > 1) sb.Append("...");

                    return sb.ToString().Trim();
                }

                return cleanString;
            }

            return String.Empty;
        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.StringUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Strip HTML/XML tags from any string.
        /// </summary>
        /// <param name="input">HTML content.</param>
        /// <returns>Content free of HTML tags.</returns>
        public static String StripTags(this String input) { return DDRIT.StripTags(input); }

        /// <summary>
        /// Trim markup to the longest string of whole words, under a particular length, and append an elipsis if there is truncated content.
        /// If the length is shorter than the first whole word, then returns the first whole word, and appends an elipsis if there is truncated content.
        /// </summary>
        /// <param name="input">The content to trim</param>
        /// <param name="maxLength">The maximum length of the returned content</param>
        /// <param name="stripTags">(true)Remove HTML tags before trimming</param>
        /// <returns></returns>
        public static String TrimTo(this String input, Int32 maxLength, Boolean stripTags = true) { return DDRIT.TrimTo(input, maxLength, stripTags); }

    }
}