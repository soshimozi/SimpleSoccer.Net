namespace SimpleSoccer.Net
{
    partial class SimpleSoccer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleSoccer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.debugAidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noAidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIDsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRegionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSupportSpotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showControllingTeamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightIfThreatenedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.simulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugAidsToolStripMenuItem,
            this.simulationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(694, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // debugAidsToolStripMenuItem
            // 
            this.debugAidsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noAidsToolStripMenuItem,
            this.showIDsToolStripMenuItem,
            this.showStatesToolStripMenuItem,
            this.showRegionsToolStripMenuItem,
            this.showSupportSpotsToolStripMenuItem,
            this.showTargetsToolStripMenuItem,
            this.showControllingTeamToolStripMenuItem,
            this.highlightIfThreatenedToolStripMenuItem});
            this.debugAidsToolStripMenuItem.Name = "debugAidsToolStripMenuItem";
            this.debugAidsToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.debugAidsToolStripMenuItem.Text = "Debug Aids";
            this.debugAidsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.debugAidsToolStripMenuItem_DropDownOpening);
            // 
            // noAidsToolStripMenuItem
            // 
            this.noAidsToolStripMenuItem.Name = "noAidsToolStripMenuItem";
            this.noAidsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.noAidsToolStripMenuItem.Text = "No Aids";
            this.noAidsToolStripMenuItem.Click += new System.EventHandler(this.noAidsToolStripMenuItem_Click);
            // 
            // showIDsToolStripMenuItem
            // 
            this.showIDsToolStripMenuItem.Name = "showIDsToolStripMenuItem";
            this.showIDsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showIDsToolStripMenuItem.Text = "Show IDs";
            this.showIDsToolStripMenuItem.Click += new System.EventHandler(this.showIDsToolStripMenuItem_Click);
            // 
            // showStatesToolStripMenuItem
            // 
            this.showStatesToolStripMenuItem.Name = "showStatesToolStripMenuItem";
            this.showStatesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showStatesToolStripMenuItem.Text = "Show States";
            this.showStatesToolStripMenuItem.Click += new System.EventHandler(this.showStatesToolStripMenuItem_Click);
            // 
            // showRegionsToolStripMenuItem
            // 
            this.showRegionsToolStripMenuItem.Name = "showRegionsToolStripMenuItem";
            this.showRegionsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showRegionsToolStripMenuItem.Text = "Show Regions";
            this.showRegionsToolStripMenuItem.Click += new System.EventHandler(this.showRegionsToolStripMenuItem_Click);
            // 
            // showSupportSpotsToolStripMenuItem
            // 
            this.showSupportSpotsToolStripMenuItem.Name = "showSupportSpotsToolStripMenuItem";
            this.showSupportSpotsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSupportSpotsToolStripMenuItem.Text = "Show Support Spots";
            this.showSupportSpotsToolStripMenuItem.Click += new System.EventHandler(this.showSupportSpotsToolStripMenuItem_Click);
            // 
            // showTargetsToolStripMenuItem
            // 
            this.showTargetsToolStripMenuItem.Name = "showTargetsToolStripMenuItem";
            this.showTargetsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showTargetsToolStripMenuItem.Text = "Show Targets";
            this.showTargetsToolStripMenuItem.Click += new System.EventHandler(this.showTargetsToolStripMenuItem_Click);
            // 
            // showControllingTeamToolStripMenuItem
            // 
            this.showControllingTeamToolStripMenuItem.Name = "showControllingTeamToolStripMenuItem";
            this.showControllingTeamToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showControllingTeamToolStripMenuItem.Text = "Show Controlling Team";
            this.showControllingTeamToolStripMenuItem.Click += new System.EventHandler(this.showControllingTeamToolStripMenuItem_Click);
            // 
            // highlightIfThreatenedToolStripMenuItem
            // 
            this.highlightIfThreatenedToolStripMenuItem.Name = "highlightIfThreatenedToolStripMenuItem";
            this.highlightIfThreatenedToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.highlightIfThreatenedToolStripMenuItem.Text = "Highlight If Threatened";
            this.highlightIfThreatenedToolStripMenuItem.Click += new System.EventHandler(this.highlightIfThreatenedToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(694, 344);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // simulationToolStripMenuItem
            // 
            this.simulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.pauseToolStripMenuItem});
            this.simulationToolStripMenuItem.Name = "simulationToolStripMenuItem";
            this.simulationToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.simulationToolStripMenuItem.Text = "Simulation";
            this.simulationToolStripMenuItem.DropDownOpening += new System.EventHandler(this.simulationToolStripMenuItem_DropDownOpening);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("startToolStripMenuItem.Image")));
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripMenuItem.Image")));
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pauseToolStripMenuItem.Image")));
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 368);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "SimpleSoccer.Net";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem debugAidsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noAidsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showIDsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showRegionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSupportSpotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTargetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlightIfThreatenedToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem showControllingTeamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;

    }
}

