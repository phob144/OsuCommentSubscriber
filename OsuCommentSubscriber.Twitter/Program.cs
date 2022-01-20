using LinqToTwitter;
using LinqToTwitter.OAuth;
using Newtonsoft.Json;
using OsuCommentSubscriber;
using OsuCommentSubscriber.Twitter;
using System.Text;

const string TWITTER_CONFIG_PATH = "twitterConfig.json";
const string USER_IDS_PATH = "userIds.txt";
const string LAST_COMMENT_TIMES_PATH = "lastCommentTimes.json";

Console.Write("Initializing the twitter client... ");

if (!File.Exists(TWITTER_CONFIG_PATH))
    File.WriteAllText(TWITTER_CONFIG_PATH, "{}");

var config = TwitterConfig.Read(TWITTER_CONFIG_PATH);
var credentials = new SingleUserInMemoryCredentialStore()
{
    ConsumerKey = config?.ApiKey,
    ConsumerSecret = config?.ApiSecret,
    AccessToken = config?.AccessToken,
    AccessTokenSecret = config?.AccessSecret,
    OAuthToken = config?.OAuthToken,
    OAuthTokenSecret = config?.OAuthSecret,
    UserID = Convert.ToUInt64(config?.UserId),
    ScreenName = config?.ScreenName
};

var twitter = new TwitterContext(new SingleUserAuthorizer()
{
    CredentialStore = credentials,
    AccessType = AuthAccessType.Write
});

Console.WriteLine("Done!");

Console.Write("Initializing the comment subscriber... ");

if (!File.Exists(USER_IDS_PATH))
    File.WriteAllText(USER_IDS_PATH, "");

var userIds = File.ReadAllLines(USER_IDS_PATH).Select(x => x.Trim()).ToList();
var lastCommentTimes = new Dictionary<string, DateTime>();

if (File.Exists(LAST_COMMENT_TIMES_PATH))
    lastCommentTimes = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(LAST_COMMENT_TIMES_PATH)) ?? new Dictionary<string, DateTime>();

var subscriber = new CommentSubscriber(userIds, lastCommentTimes);

subscriber.ScrapingStarted += (sender, e) =>
{
    Console.WriteLine("Scraping...");
};

subscriber.NewCommentsReceived += async (sender, e) =>
{
    foreach (var comment in e.Comments)
    {
        const string comment_url_format = "https://osu.ppy.sh/comments/{0}";

        var builder = new StringBuilder();

        builder.AppendLine($"New comment by {comment.Username}");
        builder.AppendLine($"Link: {string.Format(comment_url_format, comment.Id)}");
        builder.AppendLine();
        builder.AppendLine($"\"{comment.Message}\"");

        // i love the tweet length limits
        var chunks = builder.ToString().ChunkWords(280);

        var mainTweet = default(Tweet);

        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];

            if (i == 0)
            {
                mainTweet = await twitter.TweetAsync(chunk);
                continue;
            }

            if (mainTweet == null)
                continue;

            await twitter.ReplyAsync(chunk, mainTweet.ID ?? "");
        }

        foreach (var chunk in chunks)
            Console.WriteLine("\n" + chunk);
    }
};

Console.WriteLine("Done!");

subscriber.Run(TimeSpan.FromMinutes(5));

Console.WriteLine("Running...");



// watch for changes in the userIds.txt file, so users can be added dynamically
var watcher = new FileSystemWatcher(".", USER_IDS_PATH)
{
    EnableRaisingEvents = true
};

watcher.Changed += (sender, e) =>
{
    subscriber.UserIds = File.ReadAllLines(USER_IDS_PATH).Select(x => x.Trim()).ToList();
};

// save last comment times on exit to avoid duplicate tweets after maintenance/crashes
AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
{
    File.WriteAllText(LAST_COMMENT_TIMES_PATH, JsonConvert.SerializeObject(subscriber.LastCommentTimes));
};

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    File.WriteAllText(LAST_COMMENT_TIMES_PATH, JsonConvert.SerializeObject(subscriber.LastCommentTimes));
};

Thread.Sleep(-1);