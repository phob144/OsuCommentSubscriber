using HtmlAgilityPack;
using Newtonsoft.Json;
using OsuCommentSubscriber.JsonObjects;

namespace OsuCommentSubscriber
{
    public static class CommentScraper
    {
        public static List<Comment> Scrape(string userId, int page = 1)
        {
            const string url_format = "https://osu.ppy.sh/comments?user_id={0}&page={1}";

            var rawHtml = Phantom.GetHtml(string.Format(url_format, userId, page));
            
            var doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);

            return ScrapeJsonIndex(doc);
        }

        /// i'll leave that method, cause it might still be useful for static html files
        public static List<Comment> Scrape(HtmlDocument document)
        {
            var result = new List<Comment>();

            var nodes = document.GetAllNodes();

            // create a list of comment scopes, which allows to group the message and the time in the Comment struct
            var commentNodes = nodes.Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "comment__container");

            foreach (var commentNode in commentNodes)
            {
                var commentSubordinateNodes = commentNode.GetSubordinateNodes();

                var urlNode =
                    commentSubordinateNodes
                    .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "click-to-copy");

                var usernameNode =
                    commentSubordinateNodes
                    .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "js-usercard comment__row-item");

                var messageNode =
                    commentSubordinateNodes
                    .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "osu-md__paragraph");

                var timeNode =
                    commentSubordinateNodes
                    .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "js-timeago");

                var url = urlNode?.Attributes["href"].Value;
                var username = usernameNode?.InnerText;
                // InnerText skips the <br> tags, so InnerHtml must be used instead
                var message = messageNode?.InnerHtml.Replace("<br>", "\n");
                var time = HtmlDateTimeParser.Parse(timeNode?.Attributes["datetime"].Value);

                result.Add(new Comment(url, username, message, time));
            }

            return result;
        }

        private static List<Comment> ScrapeJsonIndex(HtmlDocument document)
        {
            var jsonIndexNode = document.GetElementbyId("json-index");

            // convert raw json to an intermediate object
            var jsonIndexDocument = JsonConvert.DeserializeObject<JsonIndexRootObject>(jsonIndexNode.InnerText);

            // convert the intermediate object to a list of comments
            var comments = 
                jsonIndexDocument?.Comments?
                .Select(x => new Comment(x.Id.ToString(), jsonIndexDocument?.User?.Username, x.Message, x.CreatedAt))
                .ToList();

            return comments ?? new List<Comment>();
        }
    }
}