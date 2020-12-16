// David Wahid
using System;
using System.Diagnostics;

namespace model.Extensions
{
    public static class BashExtension
    {
        public static string Bash(this string bash)
        {
            string result = string.Empty;
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{bash}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}
