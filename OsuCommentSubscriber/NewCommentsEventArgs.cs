using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCommentSubscriber
{
    public class NewCommentsEventArgs : EventArgs
    {
        public string UserId { get; }
        public List<Comment> Comments { get; }

        public NewCommentsEventArgs(string userId, List<Comment> comments)
        {
            UserId = userId;
            Comments = comments;
        }
    }
}
