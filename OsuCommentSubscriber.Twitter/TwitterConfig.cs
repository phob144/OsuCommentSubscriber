using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCommentSubscriber.Twitter
{
    public class TwitterConfig
    {
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? AccessToken { get; set; }
        public string? AccessSecret { get; set; }

        public string? OAuthToken { get; set; }
        public string? OAuthSecret { get; set; }
        public string? UserId { get; set; }
        public string? ScreenName { get; set; }

        public static TwitterConfig? Read(string path)
            => JsonConvert.DeserializeObject<TwitterConfig>(File.ReadAllText(path));
    }
}
