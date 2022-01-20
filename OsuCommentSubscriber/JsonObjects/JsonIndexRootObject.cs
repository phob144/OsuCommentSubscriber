using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCommentSubscriber.JsonObjects
{
    internal class JsonIndexRootObject
    {
        [JsonProperty("comments")]
        public List<CommentObject>? Comments { get; set; }

        [JsonProperty("user")]
        public UserObject? User { get; set; }

        internal class CommentObject
        {
            [JsonProperty("id")]
            public int? Id { get; set; }

            [JsonProperty("created_at")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }
        }

        internal class UserObject
        {
            [JsonProperty("username")]
            public string? Username { get; set; }
        }
    }
}
