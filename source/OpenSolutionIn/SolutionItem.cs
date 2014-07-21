using System;
using System.IO;

namespace FindAndOpenPrimarySolutionIn
{
    public class SolutionItem
    {
        private string _basePath;
        public string Name { get; private set; }
        public string FullPath { get; private set; }
        public string Folder { get; private set; }
        public string Size { get; private set; }

        public SolutionItem(string basePath, FileInfo fi)
        {
            _basePath = basePath;
            Name = fi.Name;
            FullPath = fi.FullName;
            Folder = fi.DirectoryName;
            Size = GetHumanReadableSizeFor(fi.Length);
        }

        private string GetHumanReadableSizeFor(long length)
        {
            var units = new[] {"b", "Kb", "Mb", "Gb"};
            var idx = 0;
            decimal size = length;
            while (size > 1024 && idx < (units.Length-1))
            {
                size /= 1024;
                idx++;
            }
            return String.Format("{0:0.0} {1}", size, units[idx]);
        }

        public override string ToString()
        {
            var relativeFolder = Folder.Replace(_basePath, "");
            if (!String.IsNullOrEmpty(relativeFolder))
            {
                return Name + " (" + Size + ", " + relativeFolder + ")";
            }
            else
            {
                return Name + " (" + Size + ")";
            }
        }
    }
}