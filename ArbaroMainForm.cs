using Arbaro2.Arbaro.GUI;
using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Tree;
using Arbaro2.DX_Engine;
using Arbaro2.DX_Engine.DXTreeMesh;
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
        CS_ParamExplanationView pev;

        CS_Params csParams = new CS_Params();
        CS_Tree tree;
        //


        private string filename = "";

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
            csParams.OnParamChanged += csParams_OnParamChanged;

            pgv = new CS_ParamGroupsView(treeView1);
            pvt = new CS_ParamValueTable(paramTablePanel, paramExplanationPanel, csParams);
            pev = new CS_ParamExplanationView(paramExplanationPanel, paramTablePanel, csParams);

            treeView1.AfterSelect += treeView1_AfterSelect;   
        }

        void csParams_OnParamChanged(object sender, CS_ParamChangedArgs e)
        {
            csParams.OnParamChanged -= csParams_OnParamChanged;
            csParams.enableDisable();
            pvt.Refresh();
            csParams.OnParamChanged += csParams_OnParamChanged;

            MakeTreeFromParams("", true);
        }

        public void RendererInitialized()
        {
            // Generate the view of the default tree only once
            // all the DirectX stuff is properly initialized
            MakeTreeFromParams("");
            MainMenuEnableDisable();
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
                filename = mainOpenFileDialog.FileName;
                MakeTreeFromParams(mainOpenFileDialog.FileName);             
            }

            MainMenuEnableDisable();
        }

        private void MakeTreeFromParams(string filename, bool paramExists = false)
        {
            if (!paramExists)
            {
                csParams = new CS_Params();
                csParams.prepare(13);
               
                ResetMenuItems();

                if (filename != "")
                {
                    Text = "Arbaro C# V0.1 - " + Path.GetFileName(mainOpenFileDialog.FileName);                    
                    csParams.readFromXML(mainOpenFileDialog.FileName);
                }
                else
                    Text = "Arbaro C# V0.1";

                // refresh params GUI
                pgv = new CS_ParamGroupsView(treeView1);
                pvt = new CS_ParamValueTable(paramTablePanel, paramExplanationPanel, csParams);
                pev = new CS_ParamExplanationView(paramExplanationPanel, paramTablePanel, csParams);

                csParams.enableDisable();
                csParams.OnParamChanged += csParams_OnParamChanged;
            }

            CS_PreciseTimer t0 = new CS_PreciseTimer(10);
            DateTime tStart = t0.Now;
            CS_TreeGenerator treeGenerator = CS_TreeGeneratorFactory.createShieldedTreeGenerator(csParams);
            tree = treeGenerator.makeTree(new Object());
            DateTime tEnd = t0.Now;


            // make 3D Tree
            if (Program.Renderer.RenderableList.ContainsKey("Skeleton"))
            {
                DXRenderable s = Program.Renderer.RenderableList["Skeleton"];
                Program.Renderer.RenderableList.Remove("Skeleton");
                s.Dispose();
            }
            DXTreeSkeleton sk = new DXTreeSkeleton(tree, csParams);
            sk.Visible = false;
            Program.Renderer.RenderableList.Add("Skeleton", sk);
            if (skeletonToolStripMenuItem.Checked) sk.Visible = true;
            else sk.Visible = false;

            if (Program.Renderer.RenderableList.ContainsKey("TreeMesh"))
            {
                DXRenderable s = Program.Renderer.RenderableList["TreeMesh"];
                Program.Renderer.RenderableList.Remove("TreeMesh");
                s.Dispose();
            }
            DXTreeMesh me = new DXTreeMesh(tree, csParams);
            Program.Renderer.RenderableList.Add("TreeMesh", me);
            if (solidWireframeToolStripMenuItem.Checked) me.Visible = true;
            else me.Visible = false;

            //DXArbaroTreeMesh me = new DXArbaroTreeMesh(tree, csParams);
            //Program.Renderer.RenderableList.Add("TreeMesh", me);

            // only reset the view when a new tree is loaded
            if (!paramExists)
            {
                Program.Renderer.CameraControler.LookAt(me.BBox);
            }

            float elapsed = (float)(tEnd.Subtract(tStart)).TotalMilliseconds;
            Console.WriteLine(elapsed);
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

        public void ResetMenuItems() 
        {
            renderToolStripMenuItem.Enabled = false;
            skeletonToolStripMenuItem.Checked = false;
            solidWireframeToolStripMenuItem.Checked = true;
            level0ToolStripMenuItem.Checked = true;
            level1ToolStripMenuItem.Checked = true;
            level2ToolStripMenuItem.Checked = true;
            level3ToolStripMenuItem.Checked = true;
            leavesToolStripMenuItem.Checked = true;
        }

        // Menu enabling / disabling
        private void MainMenuEnableDisable()
        {
            if (Program.Renderer.RenderableList.ContainsKey("Skeleton"))           
                renderToolStripMenuItem.Enabled = true;                          
            else
                renderToolStripMenuItem.Enabled = false;
        }

        private void skeletonToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            DXRenderable skeleton = Program.Renderer.RenderableList["Skeleton"];
            skeleton.Visible = (sender as ToolStripMenuItem).Checked;
        }

        private void solidWireframeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            DXRenderable mesh = Program.Renderer.RenderableList["TreeMesh"];
            mesh.Visible = (sender as ToolStripMenuItem).Checked;
        }


        // quite ugly - do that in a better way
        private void level0ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked) Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_0] = true;
            else Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_0] = false;
        }

        private void level1ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked) Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_1] = true;
            else Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_1] = false;
        }

        private void level2ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked) Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_2] = true;
            else Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_2] = false;
        }

        private void level3ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked) Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_3] = true;
            else Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEVEL_3] = false;
        }

        private void leavesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked) Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEAVES] = true;
            else Program.DXSceneOptions.LevelVisibility[(int)DXSceneOptions.LEVELS.LEAVES] = false;
        }

        private void exportAsOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.Renderer.RenderableList.ContainsKey("Mesh")) {
                mainSaveFileDialog.Filter = "obj files (*.obj)|*.obj";
                if (mainSaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Program.Renderer.RenderableList["Mesh"].ExportAsObj(mainSaveFileDialog.FileName);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename != "")
            {
                StreamWriter sw = new StreamWriter(filename);
                csParams.toXML(sw);
                sw.Close();
            }
            else {
                saveAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainSaveFileDialog.Filter = "xml files (*.xml)|*.xml";
            if (mainSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = mainSaveFileDialog.FileName;
                StreamWriter sw = new StreamWriter(filename);
                csParams.toXML(sw);
                sw.Close();
            }
        }

        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filename = "";
            MakeTreeFromParams("", false);
            MainMenuEnableDisable();
        }
    }
}
