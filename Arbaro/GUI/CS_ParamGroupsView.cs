using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.Arbaro.GUI
{
    public class CS_GroupNode
    {
        String groupName;
        String groupLabel;
        int groupLevel;

        public CS_GroupNode(String name, String label, int level)
        {

            groupName = name;
            groupLabel = label;
            groupLevel = level;
        }

        public String getGroupLabel()
        {
            return groupLabel;
        }

        public String getGroupName()
        {
            return groupName;
        }

        public int getGroupLevel()
        {
            return groupLevel;
        }
    }

    public class CS_ParamGroupsView
    {
        private TreeView _tv;

        public CS_ParamGroupsView(TreeView tv)
        {
            _tv = tv;
            CreateNodes();
        }

        private void CreateNodes()
        {
            TreeNode tn;

            CS_GroupNode general = new CS_GroupNode("", "General", CS_AbstractParam.GENERAL);
            TreeNode tnGeneral = _tv.Nodes.Add(general.getGroupLabel());
            tnGeneral.Tag = general;

            CS_GroupNode treeShape = new CS_GroupNode("SHAPE", "Tree shape", CS_AbstractParam.GENERAL);
            tn = tnGeneral.Nodes.Add(treeShape.getGroupLabel());
            tn.Tag = treeShape;

            CS_GroupNode trunkRadius = new CS_GroupNode("TRUNK", "Trunk radius", CS_AbstractParam.GENERAL);
            tn = tnGeneral.Nodes.Add(trunkRadius.getGroupLabel());
            tn.Tag = trunkRadius;

            CS_GroupNode leaves = new CS_GroupNode("LEAVES", "Leaves", CS_AbstractParam.GENERAL);
            tn = tnGeneral.Nodes.Add(leaves.getGroupLabel());
            tn.Tag = leaves;

            CS_GroupNode pruning = new CS_GroupNode("PRUNING", "Pruning/Envelope", CS_AbstractParam.GENERAL);
            tn = tnGeneral.Nodes.Add(pruning.getGroupLabel());
            tn.Tag = pruning;

            CS_GroupNode quality = new CS_GroupNode("QUALITY", "Quality", CS_AbstractParam.GENERAL);
            tn = tnGeneral.Nodes.Add(quality.getGroupLabel());
            tn.Tag = quality;

            for (int i = 0; i < 4; i++)
            {
                String lName = "Level " + i;
                if (i == 0) lName += " (trunk)";
                CS_GroupNode level = new CS_GroupNode("", lName, i);

                tn = _tv.Nodes.Add(level.getGroupLabel());

                CS_GroupNode lentaper = new CS_GroupNode("LENTAPER", "Length and taper", i);
                TreeNode tnn = tn.Nodes.Add(lentaper.getGroupLabel());
                tnn.Tag = lentaper;

                CS_GroupNode curvature = new CS_GroupNode("CURVATURE", "Curvature", i);
                tnn = tn.Nodes.Add(curvature.getGroupLabel());
                tnn.Tag = curvature;

                CS_GroupNode splitting = new CS_GroupNode("SPLITTING", "Splitting", i);
                tnn = tn.Nodes.Add(splitting.getGroupLabel());
                tnn.Tag = splitting;

                CS_GroupNode branching = new CS_GroupNode("BRANCHING", "Branching", i);
                tnn = tn.Nodes.Add(branching.getGroupLabel());
                tnn.Tag = branching;
            }


            //setSelectionPath(new TreePath(firstGroup.getPath()));
        }
    }
}
