namespace TS3SimRipper
{
    partial class PathsPrompt
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
            this.Save_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Folder_button2 = new System.Windows.Forms.Button();
            this.TSUserPathString = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Folder_button3 = new System.Windows.Forms.Button();
            this.TSSavesPathString = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.GamePaths_dataGridView = new System.Windows.Forms.DataGridView();
            this.GamePaths = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectPath = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.BlankGamePaths_button = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.AddGamePath_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GamePaths_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // Save_button
            // 
            this.Save_button.Location = new System.Drawing.Point(250, 421);
            this.Save_button.Margin = new System.Windows.Forms.Padding(2);
            this.Save_button.Name = "Save_button";
            this.Save_button.Size = new System.Drawing.Size(183, 52);
            this.Save_button.TabIndex = 2;
            this.Save_button.Text = "Save";
            this.Save_button.UseVisualStyleBackColor = true;
            this.Save_button.Click += new System.EventHandler(this.Save_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Location of TS3 base\r\ngame installation folder:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(127, 224);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(324, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ex: C:\\Program Files (x86)\\Electronic Arts\\The Sims 3  (base game)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(127, 290);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(335, 39);
            this.label4.TabIndex = 10;
            this.label4.Text = "Select the Mods folder in your Electronic Arts\\The Sims 3 folder\r\nin your Documen" +
    "ts directory.\r\nEx: C:\\Users\\MyName\\Documents\\Electronic Arts\\The Sims 3\\Mods";
            // 
            // Folder_button2
            // 
            this.Folder_button2.Location = new System.Drawing.Point(553, 263);
            this.Folder_button2.Margin = new System.Windows.Forms.Padding(2);
            this.Folder_button2.Name = "Folder_button2";
            this.Folder_button2.Size = new System.Drawing.Size(70, 29);
            this.Folder_button2.TabIndex = 9;
            this.Folder_button2.Text = "Select";
            this.Folder_button2.UseVisualStyleBackColor = true;
            this.Folder_button2.Click += new System.EventHandler(this.Folder_button2_Click);
            // 
            // TSUserPathString
            // 
            this.TSUserPathString.Location = new System.Drawing.Point(130, 268);
            this.TSUserPathString.Margin = new System.Windows.Forms.Padding(2);
            this.TSUserPathString.Name = "TSUserPathString";
            this.TSUserPathString.Size = new System.Drawing.Size(419, 20);
            this.TSUserPathString.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 263);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 26);
            this.label5.TabIndex = 7;
            this.label5.Text = "Location of TS3 user\r\nMods folder:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Please enter correct paths or make blank to auto detect, then restart";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(127, 367);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(337, 39);
            this.label6.TabIndex = 15;
            this.label6.Text = "Select the saves folder in your Electronic Arts\\The Sims 3 folder\r\nin your Docume" +
    "nts directory.\r\nEx: C:\\Users\\MyName\\Documents\\Electronic Arts\\The Sims 3\\saves";
            // 
            // Folder_button3
            // 
            this.Folder_button3.Location = new System.Drawing.Point(553, 338);
            this.Folder_button3.Margin = new System.Windows.Forms.Padding(2);
            this.Folder_button3.Name = "Folder_button3";
            this.Folder_button3.Size = new System.Drawing.Size(70, 29);
            this.Folder_button3.TabIndex = 14;
            this.Folder_button3.Text = "Select";
            this.Folder_button3.UseVisualStyleBackColor = true;
            this.Folder_button3.Click += new System.EventHandler(this.Folder_button3_Click);
            // 
            // TSSavesPathString
            // 
            this.TSSavesPathString.Location = new System.Drawing.Point(130, 343);
            this.TSSavesPathString.Margin = new System.Windows.Forms.Padding(2);
            this.TSSavesPathString.Name = "TSSavesPathString";
            this.TSSavesPathString.Size = new System.Drawing.Size(419, 20);
            this.TSSavesPathString.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 338);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 26);
            this.label7.TabIndex = 12;
            this.label7.Text = "Location of TS3 user\r\nsaves folder:";
            // 
            // GamePaths_dataGridView
            // 
            this.GamePaths_dataGridView.AllowUserToAddRows = false;
            this.GamePaths_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GamePaths_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GamePaths,
            this.SelectPath,
            this.Delete});
            this.GamePaths_dataGridView.Location = new System.Drawing.Point(130, 65);
            this.GamePaths_dataGridView.Name = "GamePaths_dataGridView";
            this.GamePaths_dataGridView.RowHeadersVisible = false;
            this.GamePaths_dataGridView.Size = new System.Drawing.Size(540, 156);
            this.GamePaths_dataGridView.TabIndex = 16;
            this.GamePaths_dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GamePaths_dataGridView_CellClick);
            // 
            // GamePaths
            // 
            this.GamePaths.HeaderText = "Path";
            this.GamePaths.Name = "GamePaths";
            this.GamePaths.Width = 350;
            // 
            // SelectPath
            // 
            this.SelectPath.HeaderText = "Select";
            this.SelectPath.Name = "SelectPath";
            this.SelectPath.Text = "Select";
            this.SelectPath.UseColumnTextForButtonValue = true;
            this.SelectPath.Width = 70;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Text = "Delete";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 70;
            // 
            // BlankGamePaths_button
            // 
            this.BlankGamePaths_button.Location = new System.Drawing.Point(12, 104);
            this.BlankGamePaths_button.Name = "BlankGamePaths_button";
            this.BlankGamePaths_button.Size = new System.Drawing.Size(112, 53);
            this.BlankGamePaths_button.TabIndex = 18;
            this.BlankGamePaths_button.Text = "Make Game\r\nPaths Blank";
            this.BlankGamePaths_button.UseVisualStyleBackColor = true;
            this.BlankGamePaths_button.Click += new System.EventHandler(this.BlankGamePaths_button_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(257, 475);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(169, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Restart for changes to take effect!";
            // 
            // AddGamePath_button
            // 
            this.AddGamePath_button.Location = new System.Drawing.Point(12, 163);
            this.AddGamePath_button.Name = "AddGamePath_button";
            this.AddGamePath_button.Size = new System.Drawing.Size(112, 41);
            this.AddGamePath_button.TabIndex = 20;
            this.AddGamePath_button.Text = "Add Game Path";
            this.AddGamePath_button.UseVisualStyleBackColor = true;
            this.AddGamePath_button.Click += new System.EventHandler(this.AddGamePath_button_Click);
            // 
            // PathsPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 512);
            this.Controls.Add(this.AddGamePath_button);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.BlankGamePaths_button);
            this.Controls.Add(this.GamePaths_dataGridView);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Folder_button3);
            this.Controls.Add(this.TSSavesPathString);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Folder_button2);
            this.Controls.Add(this.TSUserPathString);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Save_button);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PathsPrompt";
            this.Text = "TS3 SimRipper Setup";
            ((System.ComponentModel.ISupportInitialize)(this.GamePaths_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Save_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Folder_button2;
        private System.Windows.Forms.TextBox TSUserPathString;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Folder_button3;
        private System.Windows.Forms.TextBox TSSavesPathString;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView GamePaths_dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn GamePaths;
        private System.Windows.Forms.DataGridViewButtonColumn SelectPath;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.Button BlankGamePaths_button;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button AddGamePath_button;
    }
}