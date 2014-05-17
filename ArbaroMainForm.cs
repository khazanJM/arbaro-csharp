using Arbaro2.Arbaro.GUI;
using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Tree;
using Arbaro2.DX_Engine;
using Arbaro2.DX_Engine.TreeClasses;
using Arbaro2.Utilities;
using SharpDX.Windows;
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

namespace Arbaro2
{
    public partial class ArbaroMainForm : RenderForm
    {
        // Render control
        private DXRenderPanel renderHwnd = new DXRenderPanel();
        public Control renderCtrl { get { return renderHwnd; } }
        //

        // Tree params etc... should be gathered somewhere else
        CS_ParamGroupsView pgv;
        CS_ParamValueTable pvt;
        CS_Params csParams = new CS_Params();
        CS_Tree tree;
        //

        public ArbaroMainForm()
        {
            InitializeComponent();
            Width = 1024;
            Height = 768;

            renderHwnd.Parent = mainSplitContainer.Panel2;
            renderHwnd.Dock = DockStyle.Fill;
            renderHwnd.Select();

            csParams.prepare(13);
            csParams.enableDisable();
        }

        private void ArbaroMainForm_Shown(object sender, EventArgs e)
        {
            pgv = new CS_ParamGroupsView(treeView1);
            pvt = new CS_ParamValueTable(paramTablePanel, csParams);
            treeView1.AfterSelect += treeView1_AfterSelect;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Tag != null)
                {
                    CS_GroupNode gn = e.Node.Tag as CS_GroupNode;
                    pvt.ShowGroup(gn.getGroupName(), gn.getGroupLevel());
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainOpenFileDialog.Filter = "xml files (*.xml)|*.xml";
            if (mainOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Text = "Arbaro C# V0.1 - " + Path.GetFileName(mainOpenFileDialog.FileName);

                csParams = new CS_Params();
                csParams.prepare(13);
                csParams.readFromXML(mainOpenFileDialog.FileName);
                csParams.enableDisable();
                CS_PreciseTimer t0 = new CS_PreciseTimer(10);
                DateTime tStart = t0.Now;
                CS_TreeGenerator treeGenerator = CS_TreeGeneratorFactory.createShieldedTreeGenerator(csParams);
                tree = treeGenerator.makeTree(new Object());
                DateTime tEnd = t0.Now;
                float elapsed = (float)(tEnd.Subtract(tStart)).TotalMilliseconds;
                Console.WriteLine(elapsed);

                // make 3D Tree
                if (Program.Renderer.RenderableList.ContainsKey("Skeleton"))
                {
                    DXRenderable s = Program.Renderer.RenderableList["Skeleton"];
                    Program.Renderer.RenderableList.Remove("Skeleton");
                    s.Dispose();
                }
                DXTreeSkeleton sk = new DXTreeSkeleton(tree, csParams);
                Program.Renderer.RenderableList.Add("Skeleton", sk);

                if (Program.Renderer.RenderableList.ContainsKey("TreeMesh"))
                {
                    DXRenderable s = Program.Renderer.RenderableList["TreeMesh"];
                    Program.Renderer.RenderableList.Remove("TreeMesh");
                    s.Dispose();
                }
                DXTreeMesh me = new DXTreeMesh(tree, csParams);
                Program.Renderer.RenderableList.Add("TreeMesh", me);

                Program.Renderer.CameraControler.LookAt(me.BBox);
            }

            MainMenuEnableDisable();
        }

        private void ArbaroMainForm_Resize(object sender, EventArgs e)
        {
            mainSplitContainer.SplitterDistance = 300;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //
        //  Main menu management
        //

        // Menu enabling / disabling
        private void MainMenuEnableDisable()
        {
            if (Program.Renderer.RenderableList.ContainsKey("Skeleton"))
            {
                renderToolStripMenuItem.Enabled = true;
                skeletonToolStripMenuItem.Checked = true;

            }
            else
            {
                renderToolStripMenuItem.Enabled = false;
            }
        }

        private void skeletonToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            DXRenderable skeleton = Program.Renderer.RenderableList["Skeleton"];
            skeleton.Visible = (sender as ToolStripMenuItem).Checked;
        }
    }
}
