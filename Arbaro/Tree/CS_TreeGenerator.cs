using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public abstract class CS_TreeGenerator
    {
        public abstract CS_Tree makeTree(Object progress);

        public abstract void setSeed(int seed);

        public abstract int getSeed();

        public abstract CS_Params getParams();

        public abstract void setParam(String param, String value);

        // TODO: not used at the moment, may be the GUI
        // should get a TreeGenerator as a ParamContainer
        // and tree maker, and not work directly with Params
        // class
        public abstract CS_AbstractParam getParam(String param);

        /**
         * Returns a parameter group
         * 
         * @param level The branch level (0..3)
         * @param group The parameter group name
         * @return A hash table with the parameters
         */
        // TODO: not used at the moment, may be the GUI
        // should get a TreeGenerator as a ParamContainer
        // and tree maker, and not work directly with Params
        // class
        public abstract SortedList<int, CS_AbstractParam> getParamGroup(int level, String group);

        /**
         * Writes out the parameters to an XML definition file
         * 
         * @param out The output stream
         * @throws ParamException
         */
        // TODO: not used at the moment, may be the GUI
        // should get a TreeGenerator as a ParamContainer
        // and tree maker, and not work directly with Params
        // class
        public abstract void writeParamsToXML(StreamWriter swout);

        /**
         * Clear all parameter values of the tree.
         */
        public abstract void clearParams();

        /**
         * Read parameter values from an XML definition file
         * 
         * @param is The input XML stream
         * @throws ParamException
         */
        public abstract void readParamsFromXML(string filename);



    }
}
