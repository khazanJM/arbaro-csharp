using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.Arbaro.Params
{
    //TODO this class is specific for tree generation algorithm
    //may be move it to arbaro.tree package and hide it for
    //other packages. param Values can be set and read than
    //through the TreeGenerator class.


    public class CS_LevelParams
    {
        // parameters for the 4 levels

        public int level;

        // stem length and appearance
        public double nTaper; // taper to a point (cone)
        public int nCurveRes;
        public double nCurve;
        public double nCurveV;
        public double nCurveBack;
        public double nLength;
        public double nLengthV;

        // splitting
        public double nSegSplits;
        public double nSplitAngle;
        public double nSplitAngleV;

        // substems
        public int nBranches;

        /**
         * <code>nBranchDist</code>
         * is the substem distance within a segment 
         * <ul>
         * <li>0: all substems at segment base</li>
         * <li>1: distributed over full segment</li>
         * </ul>
         * 
         */
        public double nBranchDist;

        public double nDownAngle;
        public double nDownAngleV;
        public double nRotate;
        public double nRotateV;

        /**
         * <code>mesh_points</code> -
         * how many meshpoints per cross-section
         */
        public int mesh_points;

        // Error values for splitting, substem and leaf distribution
        public double splitErrorValue;
        public double substemErrorValue;

        /**
         * random generators
         */
        public CS_Random random;

        // param DB
        Hashtable paramDB = new Hashtable();


        // variables to store state when making prune test
        private int randstate;
        private double spliterrval;

        public CS_LevelParams(int l, Hashtable parDB)
        {
            level = l;
            paramDB = parDB;

            randstate = Int32.MinValue;
            spliterrval = Double.NaN;
        }

        public int initRandom(int seed)
        {
            random = new CS_Random(seed);
            return random.nextInt();
        }

        public double var(double variation)
        {
            // return a random variation value from (-variation,+variation)
            return random.uniform(-variation, variation);
        }

        public void saveState()
        {
            /*
             if (Double.isNaN(spliterrval)) {
             System.err.println("BUG: cannot override state earlier saved, "
             + "invoke restoreState first");
             System.exit(1);
             }
             */
            randstate = random.getState();
            spliterrval = splitErrorValue;
        }

        public void restoreState()
        {
            if (Double.IsNaN(spliterrval))
            {
                Console.WriteLine("BUG: there is no state saved, cannot restore.");
                Application.Exit();
            }
            random.setState(randstate);
            splitErrorValue = spliterrval;
        }

        /*
    // help methods for output of params
    private void writeParamXml(PrintWriter w, String name, int value) {
        String fullname = "" + level + name.substring(1);
        w.println("    <param name='" + fullname + "' value='"+value+"'/>");
    }
	
    private void writeParamXML(PrintWriter w, String name, double value) {
        String fullname = "" + level + name.substring(1);
        w.println("    <param name='" + fullname + "' value='"+value+"'/>");
    }
	
    void toXML(PrintWriter w, boolean leafLevelOnly) {
        w.println("    <!-- level " + level  + " -->");
        writeParamXML(w,"nDownAngle",nDownAngle);
        writeParamXML(w,"nDownAngleV",nDownAngleV);
        writeParamXML(w,"nRotate",nRotate);
        writeParamXML(w,"nRotateV",nRotateV);
        if (! leafLevelOnly) {
            writeParamXml(w,"nBranches",nBranches);
            writeParamXML(w,"nBranchDist",nBranchDist);
            //	    xml_param(w,"nBranchDistV",nBranchDistV);
            writeParamXML(w,"nLength",nLength);
            writeParamXML(w,"nLengthV",nLengthV);
            writeParamXML(w,"nTaper",nTaper);
            writeParamXML(w,"nSegSplits",nSegSplits);
            writeParamXML(w,"nSplitAngle",nSplitAngle);
            writeParamXML(w,"nSplitAngleV",nSplitAngleV);
            writeParamXml(w,"nCurveRes",nCurveRes);
            writeParamXML(w,"nCurve",nCurve);
            writeParamXML(w,"nCurveBack",nCurveBack);
            writeParamXML(w,"nCurveV",nCurveV);
        }
    }
    */

        // help method for loading params
        private int intParam(String name)
        {
            String fullname = "" + level + name.Substring(1);
            CS_IntParam par = (CS_IntParam)paramDB[fullname];
            if (par != null)
            {
                return par.intValue();
            }

            throw new Exception("bug: param " + fullname + " not found!");
        }

        private double dblParam(String name)
        {
            String fullname = "" + level + name.Substring(1);
            CS_FloatParam par = (CS_FloatParam)paramDB[fullname];
            if (par != null)
            {
                return par.doubleValue();
            }

            throw new Exception("bug: param " + fullname + " not found!");
        }

        void fromDB(bool leafLevelOnly)
        {
            if (!leafLevelOnly)
            {
                nTaper = dblParam("nTaper");
                nCurveRes = intParam("nCurveRes");
                nCurve = dblParam("nCurve");
                nCurveV = dblParam("nCurveV");
                nCurveBack = dblParam("nCurveBack");
                nLength = dblParam("nLength");
                nLengthV = dblParam("nLengthV");
                nSegSplits = dblParam("nSegSplits");
                nSplitAngle = dblParam("nSplitAngle");
                nSplitAngleV = dblParam("nSplitAngleV");
                nBranches = intParam("nBranches");
            }
            nBranchDist = dblParam("nBranchDist");
            //	nBranchDistV = dbl_param("nBranchDistV");
            nDownAngle = dblParam("nDownAngle");
            nDownAngleV = dblParam("nDownAngleV");
            nRotate = dblParam("nRotate");
            nRotateV = dblParam("nRotateV");
        }

    }
}
