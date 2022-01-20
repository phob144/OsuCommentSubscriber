using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace OsuCommentSubscriber
{
    public struct Comment
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("user_id")]
        public string? Username { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreationTime { get; set; }

        public Comment(string? id, string? username, string? message, DateTime? creationTime)
        {
            Id = id;
            Username = username;
            Message = message;
            CreationTime = creationTime;
        }
    }
}
