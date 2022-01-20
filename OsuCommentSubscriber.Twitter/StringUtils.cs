using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OsuCommentSubscriber.Twitter
{
    internal static class StringUtils
    {
        // https://regex101.com/r/DnKbVm/1
        internal static List<string> ChunkWords(this string s, int size) =>
            Regex.Matches(s, $@"(.{{1,{size}}})(?:\s|$)", RegexOptions.Singleline).Select(x => x.Value).ToList();
    }
}
