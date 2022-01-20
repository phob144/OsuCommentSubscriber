using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OsuCommentSubscriber
{
    internal static class Phantom
    {
        internal static string GetHtml(string url)
        {
            const string phantomjs_dir = "phantomjs";
            const string phantomjs_exe = $"{phantomjs_dir}/phantomjs.exe";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = phantomjs_dir,
                    FileName = phantomjs_exe,
                    Arguments = $"script.js {url}",

                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
    }
}
