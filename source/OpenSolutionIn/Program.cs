using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindAndOpenPrimarySolutionIn
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                return Usage();
            }

            var basePath = args[0];
            var allFileInfos = GetAllSolutionFileInfosUnder(basePath);
            var fileInfos = allFileInfos.ToArray();
            if (!fileInfos.Any())
            {
                return HandleNoSolutionFilesFound(basePath);
            }
            var picker = new SolutionPickerDialog();
            picker.LaunchWithOptions(basePath, fileInfos);
            Application.Run(picker);
            return 0;
        }


        private static int HandleNoSolutionFilesFound(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                MessageBox.Show(String.Format("Unable to find any .sln files under '{0}'", basePath), "No solution files found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format("Unable to find or open the folder '{0}'", basePath), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private static IEnumerable<FileInfo> GetAllSolutionFileInfosUnder(string path)
        {
            var slnFiles = GetSolutionFilePathsUnder(path);
            return slnFiles.Select(p => new FileInfo(p));
        }

        private static IEnumerable<string> GetSolutionFilePathsUnder(string path)
        {
            if (!Directory.Exists(path)) return new string[] { };
            var solutionFiles = new List<string>();
            solutionFiles.AddRange(Directory.GetFiles(path, "*.sln"));
            foreach (var dir in Directory.GetDirectories(path))
            {
                solutionFiles.AddRange(GetSolutionFilePathsUnder(dir));
            }
            return solutionFiles;
        }

        private static int Usage()
        {
            MessageBox.Show("Invoke this utility with the base path to search in. Nothing more. Nothing less.", "Be Enlightened", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return -1;
        }
    }
}
