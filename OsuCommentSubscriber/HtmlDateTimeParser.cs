using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCommentSubscriber
{
    internal class HtmlDateTimeParser
    {
        internal static DateTime? Parse(string? input)
        {
            if (input == null)
                return null;

            // reformat an input string like 2022-01-16T20:18:26+00:00 to 2022-01-16 20:18:26
            // trim the last 6 characters (+00:00)
            input = input[0..^6];
            input = input.Replace('T', ' ');

            return DateTime.ParseExact(input, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
