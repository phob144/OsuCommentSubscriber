using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCommentSubscriber
{
    internal static class HtmlUtils
    {
        internal static List<HtmlNode> GetAllNodes(this HtmlDocument document)
            => GetSubordinateNodes(document.DocumentNode);

        internal static List<HtmlNode> GetSubordinateNodes(this HtmlNode node)
        {
            var result = new List<HtmlNode>();

            foreach (var childNode in node.ChildNodes)
            {
                result.Add(childNode);
                result.AddRange(GetSubordinateNodes(childNode));
            }

            return result;
        }
    }
}
