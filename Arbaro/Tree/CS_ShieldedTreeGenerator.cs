using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public class CS_ShieldedTreeGenerator : CS_TreeGenerator
    {
        CS_TreeGenerator treeGenerator;

        /**
         * @param treeGenerator a TreeGenerator object without exception handling
         */
        public CS_ShieldedTreeGenerator(CS_TreeGenerator treeGenerator)
        {
            this.treeGenerator = treeGenerator;
        }

        /**
         * Print exceptions to the console using the Console class
         * 
         * @param e the Exception to print
         */
        protected void showException(Exception e)
        {
            Console.WriteLine("Error in tree generator:");
            Console.WriteLine(e.Message);
        }

        /**
         * See TreeGenerator interface
         */
        public override void clearParams()
        {
            try
            {
                treeGenerator.clearParams();
            }
            catch (Exception e)
            {
                showException(e);
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override CS_AbstractParam getParam(String param)
        {
            try
            {
                return treeGenerator.getParam(param);
            }
            catch (Exception e)
            {
                showException(e);
                return null;
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override SortedList<int, CS_AbstractParam> getParamGroup(int level, String group)
        {
            try
            {
                return treeGenerator.getParamGroup(level, group);
            }
            catch (Exception e)
            {
                showException(e);
                return null;
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override CS_Params getParams()
        {
            try
            {
                return treeGenerator.getParams();
            }
            catch (Exception e)
            {
                showException(e);
                return null;
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override int getSeed()
        {
            try
            {
                return treeGenerator.getSeed();
            }
            catch (Exception e)
            {
                showException(e);
                return 13;
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override CS_Tree makeTree(Object progress)
        {
            try
            {
                return treeGenerator.makeTree(progress);
            }
            catch (Exception e)
            {
                showException(e);
                return null;
            }
        }

        /**
         * See TreeGenerator interface
         */
        /*
            public void readParamsFromCfg(StreamReader sris) {
                try {
                    treeGenerator.readParamsFromCfg(is);
                } catch (Exception e) {
                    showException(e);
                }
            }
        */
        /**
         * See TreeGenerator interface
         */
        public override void readParamsFromXML(string filename)
        {
            try
            {
                treeGenerator.readParamsFromXML(filename);
            }
            catch (Exception e)
            {
                showException(e);
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override void setParam(String param, String value)
        {
            try
            {
                treeGenerator.setParam(param, value);
            }
            catch (Exception e)
            {
                showException(e);
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override void setSeed(int seed)
        {
            try
            {
                treeGenerator.setSeed(seed);
            }
            catch (Exception e)
            {
                showException(e);
            }
        }

        /**
         * See TreeGenerator interface
         */
        public override void writeParamsToXML(StreamWriter swout)
        {
            try
            {
                treeGenerator.writeParamsToXML(swout);
            }
            catch (Exception e)
            {
                showException(e);
            }
        }
    }
}
