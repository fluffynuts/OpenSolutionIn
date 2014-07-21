using System.Diagnostics;

namespace FindAndOpenPrimarySolutionIn
{
    public static class SolutionLauncher
    {
        public static void LaunchSolutionAt(string fullName)
        {
            var proc = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = fullName;
            startInfo.UseShellExecute = true;
            proc.StartInfo = startInfo;
            proc.Start();
        }
    }
}