using Arbaro2.Arbaro.GUI;
using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Tree;
using Arbaro2.DX_Engine;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                csParams.readFromXML(mainOpenFileDialog.FileName);
                csParams.enableDisable();

                CS_TreeGenerator treeGenerator = CS_TreeGeneratorFactory.createShieldedTreeGenerator(csParams);
                tree = treeGenerator.makeTree(new Object());
                Console.WriteLine("Done");

                // Display the tree
                CS_TreeTraversal traversal = new CS_DefaultTreeTraversal();
                tree.traverseTree(traversal);
            }
        }

        private void ArbaroMainForm_Resize(object sender, EventArgs e)
        {
            mainSplitContainer.SplitterDistance = 300;
        }
    }
}
