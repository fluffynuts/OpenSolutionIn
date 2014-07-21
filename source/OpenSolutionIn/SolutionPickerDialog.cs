using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PeanutButter.INI;

namespace FindAndOpenPrimarySolutionIn
{
    public partial class SolutionPickerDialog : Form
    {
        private string _forBasePath;
        private const string _defaultSolutionKeyName = "default";

        public SolutionPickerDialog()
        {
            InitializeComponent();
        }

        public void LaunchWithOptions(string forBasePath, IEnumerable<FileInfo> fileInfos)
        {
            if ((LaunchedOnlySolution(fileInfos)) ||
                (LaunchedDefaultSolutionFor(forBasePath, fileInfos)))
            {
                this.Visible = false;
                this.Shown += (e, a) =>
                {
                    this.Close();
                };
                return;
            }
            AllowUserToMakeSelection(forBasePath, fileInfos);
        }

        private bool LaunchedDefaultSolutionFor(string forBasePath, IEnumerable<FileInfo> fileInfos)
        {
            var defaultSolution = GetDefaultSolutionFor(forBasePath);
            if (defaultSolution != null)
            {
                SolutionLauncher.LaunchSolutionAt(defaultSolution);
                return true;
            }
            return false;
        }

        private void AllowUserToMakeSelection(string forBasePath, IEnumerable<FileInfo> fileInfos)
        {
            lstSolutions.Items.Clear();
            lstSolutions.Items.AddRange(fileInfos.Select(fi => new SolutionItem(forBasePath, fi) as object).ToArray());
            _forBasePath = forBasePath;
            this.Show();
        }

        private bool LaunchedOnlySolution(IEnumerable<FileInfo> fileInfos)
        {
            if (fileInfos.Count() == 1)
            {
                SolutionLauncher.LaunchSolutionAt(fileInfos.First().FullName);
                return true;
            }
            return false;
        }

        private string GetDefaultSolutionFor(string forBasePath)
        {
            var iniFile = GetIniFile();
            if (!iniFile.HasSection(forBasePath)) return null;
            if (!iniFile.Sections[forBasePath].ContainsKey(_defaultSolutionKeyName)) return null;
            return iniFile.Sections[forBasePath][_defaultSolutionKeyName];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LaunchSelectedSolution();
        }

        private void LaunchSelectedSolution()
        {
            var selected = lstSolutions.SelectedItem as SolutionItem;
            if (selected == null)
            {
                MessageBox.Show("Please select a solution to continue", "Selection required", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            if (chkRemember.Checked)
            {
                RememberSelectionFor(_forBasePath, selected.FullPath);
            }
            SolutionLauncher.LaunchSolutionAt(selected.FullPath);
            this.Close();
        }

        private void RememberSelectionFor(string forBasePath, string fullPath)
        {
            var iniFile = GetIniFile();
            if (!iniFile.HasSection(forBasePath))
                iniFile.AddSection(forBasePath);
            iniFile.Sections[forBasePath][_defaultSolutionKeyName] = fullPath;
            iniFile.Persist();
        }

        private static INIFile GetIniFile()
        {
            var iniPath = GetIniFilePath();
            return new INIFile(iniPath);
        }

        private static string GetIniFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenSolutionIn", "config.ini");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstSolutions_DoubleClick(object sender, EventArgs e)
        {
            LaunchSelectedSolution();
        }

    }
}
