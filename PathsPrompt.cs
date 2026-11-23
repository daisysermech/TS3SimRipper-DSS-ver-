/* TS4 MorphMaker, a tool for creating custom content for The Sims 4,
   Copyright (C) 2014  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using Microsoft.Win32;
using Ookii.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace TS3SimRipper
{
    public partial class PathsPrompt : Form
    {
        Game currentGame;

        private static readonly Regex PackDirRegex =
            new Regex(@"^(EP|SP)0*(\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Returns the base dir plus any EP1-EP11 / SP1-SP9 subfolders, ordered EP then SP, numerically
        private static List<string> ExpandTS3Paths(string baseDir)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
                return result;

            result.Add(baseDir);

            var packs = new List<(string type, int num, string path)>();
            foreach (string dir in Directory.GetDirectories(baseDir))
            {
                var m = PackDirRegex.Match(System.IO.Path.GetFileName(dir));
                if (!m.Success) continue;

                string type = m.Groups[1].Value.ToUpperInvariant();
                int num = int.Parse(m.Groups[2].Value);

                bool valid = (type == "EP" && num >= 1 && num <= 11) ||
                             (type == "SP" && num >= 1 && num <= 9);
                if (valid)
                    packs.Add((type, num, dir));
            }

            result.AddRange(packs
                .OrderBy(p => p.type == "EP" ? 0 : 1)   // EP first, then SP
                .ThenBy(p => p.num)
                .Select(p => p.path));

            return result;
        }

        private bool GridContains(string path)
        {
            foreach (DataGridViewRow row in GamePaths_dataGridView.Rows)
            {
                string existing = row.Cells[0].Value as string;
                if (string.Equals(existing?.TrimEnd('\\'), path.TrimEnd('\\'),
                                  StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void AddWithPacks(string baseDir)
        {
            foreach (string p in ExpandTS3Paths(baseDir))
                if (!GridContains(p))
                    GamePaths_dataGridView.Rows.Add(p);
        }

        public PathsPrompt(Game game)
        {
            InitializeComponent();
            currentGame = game;
            if (game == Game.SimsMedieval)
            {
                label2.Text = "Location of TSM base" + Environment.NewLine + "game installation folder: ";
                label3.Text = "Ex: C:\\Program Files (x86)\\Electronic Arts\\The Sims Medieval  (base game)";
                label4.Text = "Select the Mods folder in your Electronic Arts\\The Sims Medieval folder" + Environment.NewLine +
                              "in your Documents directory." + Environment.NewLine +
                              "Ex: C: \\Users\\MyName\\Documents\\Electronic Arts\\The Sims Medieval\\Mods";
                label5.Text = "Location of TSM user" + Environment.NewLine + "Mods folder:";
                label6.Text = "Select the saved sims folder in your Electronic Arts\\The Sims Medieval folder" + Environment.NewLine +
                              "in your Documents directory." +
                              "Ex: C: \\Users\\MyName\\Documents\\Electronic Arts\\The Sims Medieval\\SavedSims";
                label7.Text = "Location of TSM user" + Environment.NewLine + "saves folder:";
                System.Collections.Specialized.StringCollection TSMPathStrings = Properties.Settings.Default.TSMPath;
                if (TSMPathStrings == null) TSMPathStrings = new System.Collections.Specialized.StringCollection();
                TSUserPathString.Text = Properties.Settings.Default.TSMModsPath;
                TSSavesPathString.Text = Properties.Settings.Default.TSMSavesPath;
                foreach (string path in TSMPathStrings)
                {
                    GamePaths_dataGridView.Rows.Add(path);
                }
            }
            else
            {
                label2.Text = "Location of TS3 base" + Environment.NewLine + "game installation folder: ";
                label3.Text = "Ex: C:\\Program Files (x86)\\Electronic Arts\\The Sims 3  (base game)";
                label4.Text = "Select the Mods folder in your Electronic Arts\\The Sims 3 folder" + Environment.NewLine +
                              "in your Documents directory." + Environment.NewLine +
                              "Ex: C: \\Users\\MyName\\Documents\\Electronic Arts\\The Sims 3\\Mods";
                label5.Text = "Location of TS3 user" + Environment.NewLine + "Mods folder:";
                label6.Text = "Select the saves folder in your Electronic Arts\\The Sims 3 folder" + Environment.NewLine +
                              "in your Documents directory." +
                              "Ex: C: \\Users\\MyName\\Documents\\Electronic Arts\\The Sims 3\\saves";
                label7.Text = "Location of TS3 user" + Environment.NewLine + "saves folder:";
                System.Collections.Specialized.StringCollection TS3PathStrings = Properties.Settings.Default.TS3Path;
                if (TS3PathStrings == null) TS3PathStrings = new System.Collections.Specialized.StringCollection();
                TSUserPathString.Text = Properties.Settings.Default.TS3ModsPath;
                TSSavesPathString.Text = Properties.Settings.Default.TS3SavesPath;
                foreach (string path in TS3PathStrings)
                {
                    GamePaths_dataGridView.Rows.Add(path);
                }
            }
        }

        private void Folder_button2_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            folder.ShowNewFolderButton = false;
            folder.Description = "Select the folder where your Sims mods and custom content are located";
            folder.UseDescriptionForTitle = true;
            folder.SelectedPath = TSUserPathString.Text;
            DialogResult res = folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                TSUserPathString.Text = folder.SelectedPath;
            }
        }

        private void Folder_button3_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            folder.Description = currentGame == Game.SimsMedieval ? "Select the folder where your Sims 3 savegame files are located" :
                                                                    "Select the folder where your Sims Medieval saved sims files are located";
            folder.ShowNewFolderButton = false;
            folder.UseDescriptionForTitle = true;
            folder.SelectedPath = TSSavesPathString.Text;
            DialogResult res = folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                TSSavesPathString.Text = folder.SelectedPath;
            }
        }

        private void Save_button_Click(object sender, EventArgs e)
        {
            if (currentGame == Game.SimsMedieval)
            {
                System.Collections.Specialized.StringCollection gamePaths = new System.Collections.Specialized.StringCollection();
                for (int i = 0; i < GamePaths_dataGridView.Rows.Count; i++)
                {
                    if (GamePaths_dataGridView.Rows[i].Cells[0].Value != null) gamePaths.Add(GamePaths_dataGridView.Rows[i].Cells[0].Value.ToString());
                }
                Properties.Settings.Default.TSMPath = gamePaths;
                Properties.Settings.Default.TSMModsPath = TSUserPathString.Text;
                Properties.Settings.Default.TSMSavesPath = TSSavesPathString.Text;
            }
            else
            {
                System.Collections.Specialized.StringCollection gamePaths = new System.Collections.Specialized.StringCollection();
                for (int i = 0; i < GamePaths_dataGridView.Rows.Count; i++)
                {
                    if (GamePaths_dataGridView.Rows[i].Cells[0].Value != null) gamePaths.Add(GamePaths_dataGridView.Rows[i].Cells[0].Value.ToString());
                }
                Properties.Settings.Default.TS3Path = gamePaths;
                Properties.Settings.Default.TS3ModsPath = TSUserPathString.Text;
                Properties.Settings.Default.TS3SavesPath = TSSavesPathString.Text;
            }
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void BlankGamePaths_button_Click(object sender, EventArgs e)
        {
            GamePaths_dataGridView.Rows.Clear();
        }

        private void GamePaths_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 1) return;
            if (e.RowIndex > GamePaths_dataGridView.Rows.Count) return;
            if (e.ColumnIndex == 1)
            {
                VistaFolderBrowserDialog findFolder = new VistaFolderBrowserDialog();
                findFolder.Description = "Select the folder where your game packages are located";
                findFolder.UseDescriptionForTitle = true;
                findFolder.ShowNewFolderButton = false;
                findFolder.SelectedPath = (string)GamePaths_dataGridView.Rows[e.RowIndex].Cells[0].Value;
                DialogResult res = findFolder.ShowDialog();
                if (res == DialogResult.OK)
                {
                    GamePaths_dataGridView.Rows[e.RowIndex].Cells[0].Value = findFolder.SelectedPath;
                }
            }
            else if (e.ColumnIndex == 2)
            {
                GamePaths_dataGridView.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void AddGamePath_button_Click(object sender, EventArgs e)
        {
            string selectedPath=null;
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the The Sims 3 base game installation folder.";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = folderDialog.SelectedPath;
                    GamePaths_dataGridView.Rows.Add(selectedPath);
                }
            }
            if (GamePaths_dataGridView.Rows.Count == 1)
                AddWithPacks(selectedPath);
            //GamePaths_dataGridView.Rows.Add();
        }
    }
}
