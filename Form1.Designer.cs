namespace TS3SimRipper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveGameFolder = new System.Windows.Forms.TextBox();
            this.SaveGameFile_button = new System.Windows.Forms.Button();
            this.sims_listBox = new System.Windows.Forms.ListBox();
            this.SaveOBJ_button = new System.Windows.Forms.Button();
            this.SaveDAE_button = new System.Windows.Forms.Button();
            this.SaveMS3D_button = new System.Windows.Forms.Button();
            this.SaveGEOM_button = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Working_label = new System.Windows.Forms.Label();
            this.Outfits_comboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AllTextures_checkBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Occults_comboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Pregnancy_trackBar = new System.Windows.Forms.TrackBar();
            this.CleanDAE_checkBox = new System.Windows.Forms.CheckBox();
            this.SimInfo_button = new System.Windows.Forms.Button();
            this.SimError_button = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SaveTextures_button = new System.Windows.Forms.Button();
            this.SimFilter_checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.BoneSize_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ShowPreset_button = new System.Windows.Forms.Button();
            this.SimTrouble_button = new System.Windows.Forms.Button();
            this.GameMode_comboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SimsSort_comboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.morphPreview1 = new TS3SimRipper.MorphPreview();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pregnancy_trackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BoneSize_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Save Game or SavedSims Folder:";
            // 
            // SaveGameFolder
            // 
            this.SaveGameFolder.Location = new System.Drawing.Point(184, 55);
            this.SaveGameFolder.Name = "SaveGameFolder";
            this.SaveGameFolder.Size = new System.Drawing.Size(513, 20);
            this.SaveGameFolder.TabIndex = 1;
            // 
            // SaveGameFile_button
            // 
            this.SaveGameFile_button.Location = new System.Drawing.Point(703, 48);
            this.SaveGameFile_button.Name = "SaveGameFile_button";
            this.SaveGameFile_button.Size = new System.Drawing.Size(223, 39);
            this.SaveGameFile_button.TabIndex = 2;
            this.SaveGameFile_button.Text = "Select";
            this.SaveGameFile_button.UseVisualStyleBackColor = true;
            this.SaveGameFile_button.Click += new System.EventHandler(this.SaveGameFolder_button_Click);
            // 
            // sims_listBox
            // 
            this.sims_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sims_listBox.FormattingEnabled = true;
            this.sims_listBox.Location = new System.Drawing.Point(12, 231);
            this.sims_listBox.Name = "sims_listBox";
            this.sims_listBox.Size = new System.Drawing.Size(222, 368);
            this.sims_listBox.TabIndex = 4;
            this.sims_listBox.SelectedIndexChanged += new System.EventHandler(this.sims_listBox_SelectedIndexChanged);
            // 
            // SaveOBJ_button
            // 
            this.SaveOBJ_button.Location = new System.Drawing.Point(703, 90);
            this.SaveOBJ_button.Name = "SaveOBJ_button";
            this.SaveOBJ_button.Size = new System.Drawing.Size(223, 39);
            this.SaveOBJ_button.TabIndex = 6;
            this.SaveOBJ_button.Text = "Save as OBJ";
            this.SaveOBJ_button.UseVisualStyleBackColor = true;
            this.SaveOBJ_button.Click += new System.EventHandler(this.SaveOBJ_button_Click);
            // 
            // SaveDAE_button
            // 
            this.SaveDAE_button.Location = new System.Drawing.Point(703, 168);
            this.SaveDAE_button.Name = "SaveDAE_button";
            this.SaveDAE_button.Size = new System.Drawing.Size(223, 39);
            this.SaveDAE_button.TabIndex = 7;
            this.SaveDAE_button.Text = "Save as DAE";
            this.SaveDAE_button.UseVisualStyleBackColor = true;
            this.SaveDAE_button.Click += new System.EventHandler(this.SaveDAE_button_Click);
            // 
            // SaveMS3D_button
            // 
            this.SaveMS3D_button.Location = new System.Drawing.Point(703, 129);
            this.SaveMS3D_button.Name = "SaveMS3D_button";
            this.SaveMS3D_button.Size = new System.Drawing.Size(223, 39);
            this.SaveMS3D_button.TabIndex = 8;
            this.SaveMS3D_button.Text = "Save as MS3D";
            this.SaveMS3D_button.UseVisualStyleBackColor = true;
            this.SaveMS3D_button.Click += new System.EventHandler(this.SaveMS3D_button_Click);
            // 
            // SaveGEOM_button
            // 
            this.SaveGEOM_button.Location = new System.Drawing.Point(524, 13);
            this.SaveGEOM_button.Name = "SaveGEOM_button";
            this.SaveGEOM_button.Size = new System.Drawing.Size(104, 23);
            this.SaveGEOM_button.TabIndex = 9;
            this.SaveGEOM_button.Text = "Save as GEOM";
            this.SaveGEOM_button.UseVisualStyleBackColor = true;
            this.SaveGEOM_button.Visible = false;
            this.SaveGEOM_button.Click += new System.EventHandler(this.SaveGEOM_button_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(938, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.setupToolStripMenuItem.Text = "Setup";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // Working_label
            // 
            this.Working_label.AutoSize = true;
            this.Working_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Working_label.Location = new System.Drawing.Point(395, 433);
            this.Working_label.Name = "Working_label";
            this.Working_label.Size = new System.Drawing.Size(172, 37);
            this.Working_label.TabIndex = 12;
            this.Working_label.Text = "Working....";
            this.Working_label.Visible = false;
            // 
            // Outfits_comboBox
            // 
            this.Outfits_comboBox.FormattingEnabled = true;
            this.Outfits_comboBox.Location = new System.Drawing.Point(747, 362);
            this.Outfits_comboBox.Name = "Outfits_comboBox";
            this.Outfits_comboBox.Size = new System.Drawing.Size(179, 21);
            this.Outfits_comboBox.TabIndex = 14;
            this.Outfits_comboBox.SelectedIndexChanged += new System.EventHandler(this.Outfits_comboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(700, 365);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Outfit:";
            // 
            // AllTextures_checkBox
            // 
            this.AllTextures_checkBox.AutoSize = true;
            this.AllTextures_checkBox.Location = new System.Drawing.Point(708, 281);
            this.AllTextures_checkBox.Name = "AllTextures_checkBox";
            this.AllTextures_checkBox.Size = new System.Drawing.Size(218, 17);
            this.AllTextures_checkBox.TabIndex = 18;
            this.AllTextures_checkBox.Text = "Process and save normal/emission maps";
            this.toolTip1.SetToolTip(this.AllTextures_checkBox, "If checked, normal maps and emission maps will also be saved.");
            this.AllTextures_checkBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(700, 338);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Occult:";
            this.label4.Visible = false;
            // 
            // Occults_comboBox
            // 
            this.Occults_comboBox.FormattingEnabled = true;
            this.Occults_comboBox.Location = new System.Drawing.Point(747, 335);
            this.Occults_comboBox.Name = "Occults_comboBox";
            this.Occults_comboBox.Size = new System.Drawing.Size(179, 21);
            this.Occults_comboBox.TabIndex = 21;
            this.Occults_comboBox.Visible = false;
            this.Occults_comboBox.SelectedIndexChanged += new System.EventHandler(this.Occults_comboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(700, 431);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Pregnant:";
            this.label5.Visible = false;
            // 
            // Pregnancy_trackBar
            // 
            this.Pregnancy_trackBar.LargeChange = 1;
            this.Pregnancy_trackBar.Location = new System.Drawing.Point(762, 425);
            this.Pregnancy_trackBar.Name = "Pregnancy_trackBar";
            this.Pregnancy_trackBar.Size = new System.Drawing.Size(164, 45);
            this.Pregnancy_trackBar.TabIndex = 23;
            this.Pregnancy_trackBar.Visible = false;
            this.Pregnancy_trackBar.Scroll += new System.EventHandler(this.Pregnancy_trackBar_Scroll);
            // 
            // CleanDAE_checkBox
            // 
            this.CleanDAE_checkBox.AutoSize = true;
            this.CleanDAE_checkBox.Checked = true;
            this.CleanDAE_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CleanDAE_checkBox.Location = new System.Drawing.Point(708, 258);
            this.CleanDAE_checkBox.Name = "CleanDAE_checkBox";
            this.CleanDAE_checkBox.Size = new System.Drawing.Size(206, 17);
            this.CleanDAE_checkBox.TabIndex = 24;
            this.CleanDAE_checkBox.Text = "Clean DAE mesh? (Removes doubles)";
            this.CleanDAE_checkBox.UseVisualStyleBackColor = true;
            // 
            // SimInfo_button
            // 
            this.SimInfo_button.Location = new System.Drawing.Point(703, 481);
            this.SimInfo_button.Name = "SimInfo_button";
            this.SimInfo_button.Size = new System.Drawing.Size(223, 39);
            this.SimInfo_button.TabIndex = 25;
            this.SimInfo_button.Text = "Show Sim Information Listing";
            this.SimInfo_button.UseVisualStyleBackColor = true;
            this.SimInfo_button.Click += new System.EventHandler(this.SimInfo_Click);
            // 
            // SimError_button
            // 
            this.SimError_button.Location = new System.Drawing.Point(703, 520);
            this.SimError_button.Name = "SimError_button";
            this.SimError_button.Size = new System.Drawing.Size(223, 39);
            this.SimError_button.TabIndex = 26;
            this.SimError_button.Text = "Show Errors";
            this.SimError_button.UseVisualStyleBackColor = true;
            this.SimError_button.Click += new System.EventHandler(this.SimError_Click);
            // 
            // SaveTextures_button
            // 
            this.SaveTextures_button.Location = new System.Drawing.Point(703, 207);
            this.SaveTextures_button.Name = "SaveTextures_button";
            this.SaveTextures_button.Size = new System.Drawing.Size(223, 39);
            this.SaveTextures_button.TabIndex = 27;
            this.SaveTextures_button.Text = "Save Textures Only";
            this.SaveTextures_button.UseVisualStyleBackColor = true;
            this.SaveTextures_button.Click += new System.EventHandler(this.SaveTextures_button_Click);
            // 
            // SimFilter_checkedListBox
            // 
            this.SimFilter_checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SimFilter_checkedListBox.CheckOnClick = true;
            this.SimFilter_checkedListBox.ColumnWidth = 100;
            this.SimFilter_checkedListBox.FormattingEnabled = true;
            this.SimFilter_checkedListBox.Items.AddRange(new object[] {
            "Human",
            "Animal",
            "Male",
            "Female",
            "Toddler",
            "Child",
            "Teen",
            "YA",
            "Adult",
            "Elder"});
            this.SimFilter_checkedListBox.Location = new System.Drawing.Point(12, 90);
            this.SimFilter_checkedListBox.MultiColumn = true;
            this.SimFilter_checkedListBox.Name = "SimFilter_checkedListBox";
            this.SimFilter_checkedListBox.Size = new System.Drawing.Size(222, 109);
            this.SimFilter_checkedListBox.TabIndex = 36;
            this.SimFilter_checkedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SimFilter_checkedListBox_ItemCheck);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(705, 306);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Bone Size in Blender:";
            // 
            // BoneSize_numericUpDown
            // 
            this.BoneSize_numericUpDown.Location = new System.Drawing.Point(819, 304);
            this.BoneSize_numericUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.BoneSize_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BoneSize_numericUpDown.Name = "BoneSize_numericUpDown";
            this.BoneSize_numericUpDown.Size = new System.Drawing.Size(83, 20);
            this.BoneSize_numericUpDown.TabIndex = 40;
            this.BoneSize_numericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // ShowPreset_button
            // 
            this.ShowPreset_button.Location = new System.Drawing.Point(747, 389);
            this.ShowPreset_button.Name = "ShowPreset_button";
            this.ShowPreset_button.Size = new System.Drawing.Size(179, 29);
            this.ShowPreset_button.TabIndex = 42;
            this.ShowPreset_button.Text = "Show Presets";
            this.ShowPreset_button.UseVisualStyleBackColor = true;
            this.ShowPreset_button.Click += new System.EventHandler(this.ShowPreset_button_Click);
            // 
            // SimTrouble_button
            // 
            this.SimTrouble_button.Location = new System.Drawing.Point(703, 559);
            this.SimTrouble_button.Name = "SimTrouble_button";
            this.SimTrouble_button.Size = new System.Drawing.Size(223, 39);
            this.SimTrouble_button.TabIndex = 43;
            this.SimTrouble_button.Text = "Save Sim Troubleshooting Info";
            this.SimTrouble_button.UseVisualStyleBackColor = true;
            this.SimTrouble_button.Click += new System.EventHandler(this.SimTrouble_button_Click);
            // 
            // GameMode_comboBox
            // 
            this.GameMode_comboBox.FormattingEnabled = true;
            this.GameMode_comboBox.Items.AddRange(new object[] {
            "The Sims 3",
            "The Sims Medieval"});
            this.GameMode_comboBox.Location = new System.Drawing.Point(184, 28);
            this.GameMode_comboBox.Name = "GameMode_comboBox";
            this.GameMode_comboBox.Size = new System.Drawing.Size(171, 21);
            this.GameMode_comboBox.TabIndex = 44;
            this.GameMode_comboBox.SelectedIndexChanged += new System.EventHandler(this.GameMode_comboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Game Mode:";
            // 
            // SimsSort_comboBox
            // 
            this.SimsSort_comboBox.FormattingEnabled = true;
            this.SimsSort_comboBox.Items.AddRange(new object[] {
            "Last name",
            "First name"});
            this.SimsSort_comboBox.Location = new System.Drawing.Point(62, 205);
            this.SimsSort_comboBox.Name = "SimsSort_comboBox";
            this.SimsSort_comboBox.Size = new System.Drawing.Size(172, 21);
            this.SimsSort_comboBox.TabIndex = 46;
            this.SimsSort_comboBox.SelectedIndexChanged += new System.EventHandler(this.SimsSort_comboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 208);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Sort By:";
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(240, 90);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(457, 508);
            this.elementHost1.TabIndex = 17;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.morphPreview1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 605);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SimsSort_comboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GameMode_comboBox);
            this.Controls.Add(this.SimTrouble_button);
            this.Controls.Add(this.ShowPreset_button);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BoneSize_numericUpDown);
            this.Controls.Add(this.SimFilter_checkedListBox);
            this.Controls.Add(this.SaveTextures_button);
            this.Controls.Add(this.SimError_button);
            this.Controls.Add(this.SimInfo_button);
            this.Controls.Add(this.CleanDAE_checkBox);
            this.Controls.Add(this.Pregnancy_trackBar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Occults_comboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AllTextures_checkBox);
            this.Controls.Add(this.Working_label);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Outfits_comboBox);
            this.Controls.Add(this.SaveGEOM_button);
            this.Controls.Add(this.SaveMS3D_button);
            this.Controls.Add(this.SaveDAE_button);
            this.Controls.Add(this.SaveOBJ_button);
            this.Controls.Add(this.sims_listBox);
            this.Controls.Add(this.SaveGameFile_button);
            this.Controls.Add(this.SaveGameFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pregnancy_trackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BoneSize_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SaveGameFolder;
        private System.Windows.Forms.Button SaveGameFile_button;
        private System.Windows.Forms.ListBox sims_listBox;
        private System.Windows.Forms.Button SaveOBJ_button;
        private System.Windows.Forms.Button SaveDAE_button;
        private System.Windows.Forms.Button SaveMS3D_button;
        private System.Windows.Forms.Button SaveGEOM_button;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label Working_label;
        private System.Windows.Forms.ComboBox Outfits_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private MorphPreview morphPreview3;
        private System.Windows.Forms.CheckBox AllTextures_checkBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Occults_comboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar Pregnancy_trackBar;
        private System.Windows.Forms.CheckBox CleanDAE_checkBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button SimInfo_button;
        private System.Windows.Forms.Button SimError_button;
        private System.Windows.Forms.Button SaveTextures_button;
        private System.Windows.Forms.CheckedListBox SimFilter_checkedListBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown BoneSize_numericUpDown;
        private System.Windows.Forms.Button ShowPreset_button;
        private System.Windows.Forms.Button SimTrouble_button;
        private MorphPreview morphPreview1;
        private System.Windows.Forms.ComboBox GameMode_comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox SimsSort_comboBox;
        private System.Windows.Forms.Label label7;
    }
}

