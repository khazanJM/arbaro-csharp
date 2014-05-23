namespace Arbaro2
{
    partial class ArbaroMainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skeletonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.solidWireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.level0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.level3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leavesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.mainOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.paramTablePanel = new System.Windows.Forms.Panel();
            this.exportAsOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mainSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.paramExplanationPanel = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.paramExplanationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.renderToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(799, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exportAsOBJToolStripMenuItem,
            this.toolStripMenuItem4,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // renderToolStripMenuItem
            // 
            this.renderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skeletonToolStripMenuItem,
            this.toolStripMenuItem2,
            this.solidWireframeToolStripMenuItem,
            this.toolStripMenuItem3,
            this.level0ToolStripMenuItem,
            this.level1ToolStripMenuItem,
            this.level2ToolStripMenuItem,
            this.level3ToolStripMenuItem,
            this.leavesToolStripMenuItem});
            this.renderToolStripMenuItem.Enabled = false;
            this.renderToolStripMenuItem.Name = "renderToolStripMenuItem";
            this.renderToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.renderToolStripMenuItem.Text = "Render";
            // 
            // skeletonToolStripMenuItem
            // 
            this.skeletonToolStripMenuItem.CheckOnClick = true;
            this.skeletonToolStripMenuItem.Name = "skeletonToolStripMenuItem";
            this.skeletonToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.skeletonToolStripMenuItem.Text = "Skeleton";
            this.skeletonToolStripMenuItem.CheckedChanged += new System.EventHandler(this.skeletonToolStripMenuItem_CheckedChanged);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 6);
            // 
            // solidWireframeToolStripMenuItem
            // 
            this.solidWireframeToolStripMenuItem.Checked = true;
            this.solidWireframeToolStripMenuItem.CheckOnClick = true;
            this.solidWireframeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.solidWireframeToolStripMenuItem.Name = "solidWireframeToolStripMenuItem";
            this.solidWireframeToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.solidWireframeToolStripMenuItem.Text = "Solid wireframe";
            this.solidWireframeToolStripMenuItem.CheckedChanged += new System.EventHandler(this.solidWireframeToolStripMenuItem_CheckedChanged);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 6);
            // 
            // level0ToolStripMenuItem
            // 
            this.level0ToolStripMenuItem.Checked = true;
            this.level0ToolStripMenuItem.CheckOnClick = true;
            this.level0ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.level0ToolStripMenuItem.Name = "level0ToolStripMenuItem";
            this.level0ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.level0ToolStripMenuItem.Text = "Level 0";
            this.level0ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.level0ToolStripMenuItem_CheckedChanged);
            // 
            // level1ToolStripMenuItem
            // 
            this.level1ToolStripMenuItem.Checked = true;
            this.level1ToolStripMenuItem.CheckOnClick = true;
            this.level1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.level1ToolStripMenuItem.Name = "level1ToolStripMenuItem";
            this.level1ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.level1ToolStripMenuItem.Text = "Level 1";
            this.level1ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.level1ToolStripMenuItem_CheckedChanged);
            // 
            // level2ToolStripMenuItem
            // 
            this.level2ToolStripMenuItem.Checked = true;
            this.level2ToolStripMenuItem.CheckOnClick = true;
            this.level2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.level2ToolStripMenuItem.Name = "level2ToolStripMenuItem";
            this.level2ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.level2ToolStripMenuItem.Text = "Level 2";
            this.level2ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.level2ToolStripMenuItem_CheckedChanged);
            // 
            // level3ToolStripMenuItem
            // 
            this.level3ToolStripMenuItem.Checked = true;
            this.level3ToolStripMenuItem.CheckOnClick = true;
            this.level3ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.level3ToolStripMenuItem.Name = "level3ToolStripMenuItem";
            this.level3ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.level3ToolStripMenuItem.Text = "Level 3";
            this.level3ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.level3ToolStripMenuItem_CheckedChanged);
            // 
            // leavesToolStripMenuItem
            // 
            this.leavesToolStripMenuItem.Checked = true;
            this.leavesToolStripMenuItem.CheckOnClick = true;
            this.leavesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.leavesToolStripMenuItem.Name = "leavesToolStripMenuItem";
            this.leavesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.leavesToolStripMenuItem.Text = "Leaves";
            this.leavesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.leavesToolStripMenuItem_CheckedChanged);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.Location = new System.Drawing.Point(6, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(291, 253);
            this.treeView1.TabIndex = 0;
            // 
            // mainOpenFileDialog
            // 
            this.mainOpenFileDialog.FileName = "openFileDialog1";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 24);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.paramExplanationPanel);
            this.mainSplitContainer.Panel1.Controls.Add(this.paramTablePanel);
            this.mainSplitContainer.Panel1.Controls.Add(this.treeView1);
            this.mainSplitContainer.Panel1MinSize = 300;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.mainSplitContainer.Size = new System.Drawing.Size(799, 567);
            this.mainSplitContainer.SplitterDistance = 300;
            this.mainSplitContainer.TabIndex = 2;
            // 
            // paramTablePanel
            // 
            this.paramTablePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paramTablePanel.Location = new System.Drawing.Point(6, 262);
            this.paramTablePanel.Name = "paramTablePanel";
            this.paramTablePanel.Size = new System.Drawing.Size(291, 146);
            this.paramTablePanel.TabIndex = 1;
            // 
            // exportAsOBJToolStripMenuItem
            // 
            this.exportAsOBJToolStripMenuItem.Name = "exportAsOBJToolStripMenuItem";
            this.exportAsOBJToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportAsOBJToolStripMenuItem.Text = "Export as OBJ...";
            this.exportAsOBJToolStripMenuItem.Click += new System.EventHandler(this.exportAsOBJToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(181, 6);
            // 
            // paramExplanationPanel
            // 
            this.paramExplanationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paramExplanationPanel.Controls.Add(this.webBrowser1);
            this.paramExplanationPanel.Location = new System.Drawing.Point(6, 414);
            this.paramExplanationPanel.Name = "paramExplanationPanel";
            this.paramExplanationPanel.Size = new System.Drawing.Size(291, 146);
            this.paramExplanationPanel.TabIndex = 2;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(289, 144);
            this.webBrowser1.TabIndex = 0;
            // 
            // ArbaroMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 591);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "ArbaroMainForm";
            this.Text = "Arbaro C# V0.1";
            this.Shown += new System.EventHandler(this.ArbaroMainForm_Shown);
            this.Resize += new System.EventHandler(this.ArbaroMainForm_Resize);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.paramExplanationPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.OpenFileDialog mainOpenFileDialog;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.Panel paramTablePanel;
        private System.Windows.Forms.ToolStripMenuItem renderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skeletonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem solidWireframeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem level0ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem level3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leavesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.SaveFileDialog mainSaveFileDialog;
        private System.Windows.Forms.Panel paramExplanationPanel;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}

