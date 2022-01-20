using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using t = System.Timers;

namespace OsuCommentSubscriber
{
    public class CommentSubscriber
    {
        private t.Timer? _timer;

        public event EventHandler? ScrapingStarted;
        public event NewCommentsEventHandler? NewCommentsReceived;

        public List<string> UserIds { get; set; }
        public Dictionary<string, DateTime> LastCommentTimes { get; set; }

        public CommentSubscriber(List<string> userIds, Dictionary<string, DateTime> lastCommentTimes)
        {
            UserIds = userIds;
            LastCommentTimes = lastCommentTimes;

            foreach (var userId in userIds)
                if (!LastCommentTimes.ContainsKey(userId))
                    LastCommentTimes.Add(userId, CommentScraper.Scrape(userId).Max(x => x.CreationTime) ?? default);
        }

        public CommentSubscriber(List<string> userIds) : this(userIds, new Dictionary<string, DateTime>())
        {
        }

        public void Run(TimeSpan interval)
        {
            _timer = new t.Timer(interval.TotalMilliseconds);

            _timer.Elapsed += (sender, e) =>
            {
                Run();
            };

            _timer.Start();
        }

        public void Run()
        {
            ScrapingStarted?.Invoke(this, EventArgs.Empty);

            foreach (var userId in UserIds)
            {
                var comments = CommentScraper.Scrape(userId);
                var newComments = new List<Comment>();

                // make sure we don't emit en entire page of comments on the first go
                if (LastCommentTimes[userId] == default)
                {
                    LastCommentTimes[userId] = comments.Max(x => x.CreationTime) ?? default;
                    continue;
                }

                foreach (var comment in comments)
                {
                    if (comment.CreationTime <= LastCommentTimes[userId])
                        break;

                    newComments.Add(comment);
                }

                if (newComments.Count > 0)
                {
                    LastCommentTimes[userId] = newComments.Max(x => x.CreationTime) ?? default;

                    NewCommentsReceived?.Invoke(this, new NewCommentsEventArgs(userId, newComments));
                }
            }
        }
    }
}
