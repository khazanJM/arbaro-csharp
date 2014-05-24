using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.Params
{
    public class CS_Params
    {

        // Tree Shapes 
        public const int CONICAL = 0;
        public const int SPHERICAL = 1;
        public const int HEMISPHERICAL = 2;
        public const int CYLINDRICAL = 3;
        public const int TAPERED_CYLINDRICAL = 4;
        public const int FLAME = 5;
        public const int INVERSE_CONICAL = 6;
        public const int TEND_FLAME = 7;
        public const int ENVELOPE = 8;

        public float leavesErrorValue;

        protected CS_LevelParams[] levelParams;
        public CS_Random random;
        Hashtable paramDB;

        // debugging etc.
        /*	public boolean debug=false;
            public boolean verbose=false;
        */
        public bool preview = false;
        public bool ignoreVParams;
        public int stopLevel;

        // general params
        public String Species;

        public float LeafQuality;

        // this mesh parameters are influenced by Smooth, 
        // this are only defaults here
        public float Smooth;
        public float mesh_quality;  // 0..1 - factor for mesh point number 
        // (1+mesh_quality)
        public int smooth_mesh_level; // -1..Levels - add average normals 
        // to mesh points of all levels below


        // the seed
        //public int Seed;

        // defauls values for tree params
        public int Levels;

        // trunk&radius parameters
        public float Ratio;
        public float RatioPower;
        public int Shape;
        public float BaseSize;
        public float Flare;

        public int Lobes;
        public float LobeDepth;

        // leave parameters
        public int Leaves;
        public String LeafShape;
        public float LeafScale;
        public float LeafScaleX;

        // new introduced - not in the paper
        public float LeafStemLen;
        public float LeafBend;
        public int LeafDistrib;

        // tree scale
        public float Scale;
        public float ScaleV;

        // additional trunk scaling
        public float _0Scale; // only 0SCale used
        public float _0ScaleV; // only 0ScaleV used

        // attraction and pruning/envelope
        public float AttractionUp;
        public float PruneRatio;
        public float PrunePowerLow;
        public float PrunePowerHigh;
        public float PruneWidth;
        public float PruneWidthPeak;

        // base splits
        public int _0BaseSplits;

        // variables need for stem creation
        public float scale_tree;

        // change events
        public event EventHandler<CS_ParamChangedArgs> OnParamChanged;
        /*
    protected ChangeEvent changeEvent = null;
    protected EventListenerList listenerList = new EventListenerList();
    */

        public CS_Params()
        {

            //		debug = false;
            //verbose = true;
            ignoreVParams = false;

            stopLevel = -1;

            Species = "default";

            LeafQuality = 1;

            Smooth = 0.5f;

            // the default seed
            //		Seed = 13;

            // create paramDB
            paramDB = new Hashtable();
            levelParams = new CS_LevelParams[4];
            for (int l = 0; l < 4; l++)
            {
                levelParams[l] = new CS_LevelParams(l, paramDB);
            }
            registerParams();
        }

        public CS_Params(CS_Params other)
        {
            // copy values from other
            //debug = other.debug;
            //verbose = other.verbose;
            ignoreVParams = other.ignoreVParams;
            stopLevel = other.stopLevel;
            Species = other.Species;
            //		Seed = other.Seed;
            Smooth = other.Smooth;

            // create paramDB
            paramDB = new Hashtable();
            levelParams = new CS_LevelParams[4];
            for (int l = 0; l < 4; l++)
            {
                levelParams[l] = new CS_LevelParams(l, paramDB);
            }
            registerParams();

            // copy param values
            foreach (CS_AbstractParam p in paramDB.Values)
            {
                try
                {
                    CS_AbstractParam otherParam = other.getParam(p.name);
                    if (!otherParam.empty())
                    {
                        p.setValue(otherParam.getValue());
                    } // else use default value
                }
                catch (Exception err)
                {
                    Console.WriteLine("Error copying params: " + err.Message);
                }
            }
        }

        public CS_LevelParams getLevelParams(int stemlevel)
        {
            return levelParams[Math.Min(stemlevel, 3)];
        }


        public void setSpecies(String sp)
        {
            Species = sp;
            raiseOnParamChanged("");
        }

        public String getSpecies()
        {
            return Species;
        }

        // help methods for output of params
        private void writeParamXML(StreamWriter w, String name, int value)
        {
            w.WriteLine("    <param name='" + name + "' value='" + value + "'/>");
        }

        private void writeParamXML(StreamWriter w, String name, double value)
        {
            w.WriteLine("    <param name='" + name + "' value='" + value + "'/>");
        }

        private void writeParamXML(StreamWriter w, String name, String value)
        {
            w.WriteLine("    <param name='" + name + "' value='" + value + "'/>");
        }

        public void toXML(StreamWriter w)
        {
            fromDB(); //prepare(); // read parameters from paramDB
            w.WriteLine("<?xml version='1.0' ?>");
            w.WriteLine();
            w.WriteLine("<arbaro>");
            w.WriteLine("  <species name='" + Species + "'>");
            w.WriteLine("    <!-- general params -->");
            // FIXME: maybe use paramDB to print out params
            // thus no one could be forgotten?
            writeParamXML(w, "Shape", Shape);
            writeParamXML(w, "Levels", Levels);
            writeParamXML(w, "Scale", Scale);
            writeParamXML(w, "ScaleV", ScaleV);
            writeParamXML(w, "BaseSize", BaseSize);
            writeParamXML(w, "Ratio", Ratio);
            writeParamXML(w, "RatioPower", RatioPower);
            writeParamXML(w, "Flare", Flare);
            writeParamXML(w, "Lobes", Lobes);
            writeParamXML(w, "LobeDepth", LobeDepth);
            writeParamXML(w, "Smooth", Smooth);
            writeParamXML(w, "Leaves", Leaves);
            writeParamXML(w, "LeafShape", LeafShape);
            writeParamXML(w, "LeafScale", LeafScale);
            writeParamXML(w, "LeafScaleX", LeafScaleX);
            writeParamXML(w, "LeafQuality", LeafQuality);
            writeParamXML(w, "LeafStemLen", LeafStemLen);
            writeParamXML(w, "LeafDistrib", LeafDistrib);
            writeParamXML(w, "LeafBend", LeafBend);
            writeParamXML(w, "AttractionUp", AttractionUp);
            writeParamXML(w, "PruneRatio", PruneRatio);
            writeParamXML(w, "PrunePowerLow", PrunePowerLow);
            writeParamXML(w, "PrunePowerHigh", PrunePowerHigh);
            writeParamXML(w, "PruneWidth", PruneWidth);
            writeParamXML(w, "PruneWidthPeak", PruneWidthPeak);
            writeParamXML(w, "0Scale", _0Scale);
            writeParamXML(w, "0ScaleV", _0ScaleV);
            writeParamXML(w, "0BaseSplits", _0BaseSplits);

            for (int i = 0; i <= Math.Min(Levels, 3); i++)
            {
                levelParams[i].toXML(w, i == Levels); // i==Levels => leaf level only
            }
            w.WriteLine("  </species>");
            w.WriteLine("</arbaro>");
            w.Flush();
        }

        public void clearParams()
        {
            foreach (CS_AbstractParam p in paramDB.Values)
            {
                p.clear();
            }
        }

        // help method for loading params
        private int getIntParam(String name)
        {
            CS_IntParam par = (CS_IntParam)paramDB[name];
            if (par != null)
            {
                return par.intValue();
            }
            else
            {
                throw new Exception("bug: param " + name + " not found!");
            }
        }

        private float getDblParam(String name)
        {
            CS_FloatParam par = (CS_FloatParam)paramDB[name];
            if (par != null)
            {
                return par.doubleValue();
            }
            else
            {
                throw new Exception("bug: param " + name + " not found!");
            }
        }

        private String getStrParam(String name)
        {
            CS_StringParam par = (CS_StringParam)paramDB[name];
            if (par != null)
            {
                return par.getValue();
            }
            else
            {
                throw new Exception("bug: param " + name + " not found!");
            }
        }

        void fromDB()
        {
            LeafQuality = getDblParam("LeafQuality");
            Smooth = getDblParam("Smooth");
            Levels = getIntParam("Levels");
            Ratio = getDblParam("Ratio");
            RatioPower = getDblParam("RatioPower");
            Shape = getIntParam("Shape");
            BaseSize = getDblParam("BaseSize");
            Flare = getDblParam("Flare");
            Lobes = getIntParam("Lobes");
            LobeDepth = getDblParam("LobeDepth");
            Leaves = getIntParam("Leaves");
            LeafShape = getStrParam("LeafShape");
            LeafScale = getDblParam("LeafScale");
            LeafScaleX = getDblParam("LeafScaleX");
            LeafStemLen = getDblParam("LeafStemLen");
            LeafDistrib = getIntParam("LeafDistrib");
            LeafBend = getDblParam("LeafBend");
            Scale = getDblParam("Scale");
            ScaleV = getDblParam("ScaleV");
            _0Scale = getDblParam("0Scale");
            _0ScaleV = getDblParam("0ScaleV");
            AttractionUp = getDblParam("AttractionUp");
            PruneRatio = getDblParam("PruneRatio");
            PrunePowerLow = getDblParam("PrunePowerLow");
            PrunePowerHigh = getDblParam("PrunePowerHigh");
            PruneWidth = getDblParam("PruneWidth");
            PruneWidthPeak = getDblParam("PruneWidthPeak");
            _0BaseSplits = getIntParam("0BaseSplits");
            Species = getStrParam("Species");
            //		Seed = getIntParam("Seed");
            //		outputType = getIntParam("OutFormat");

            for (int i = 0; i <= Math.Min(Levels, 3); i++)
            {
                levelParams[i].fromDB(i == Levels); // i==Levels => leaf level only
            }
        }

        public void prepare(int seed)
        {
            //if (debug) { verbose=false; }

            // read in parameter values from ParamDB
            fromDB();

            if (ignoreVParams)
            {
                ScaleV = 0;
                for (int i = 1; i < 4; i++)
                {
                    CS_LevelParams lp = levelParams[i];
                    lp.nCurveV = 0;
                    lp.nLengthV = 0;
                    lp.nSplitAngleV = 0;
                    lp.nRotateV = 0;
                    // lp.nBranchDistV = 0;
                    if (lp.nDownAngle > 0) { lp.nDownAngle = 0; }
                }
            }

            // additional params checks
            for (int l = 0; l < Math.Min(Levels, 4); l++)
            {
                CS_LevelParams lp = levelParams[l];
                if (lp.nSegSplits > 0 && lp.nSplitAngle == 0)
                {
                    throw new Exception("nSplitAngle may not be 0.");
                }
            }

            // create one random generator for every level
            // so you can develop a tree level by level without
            // influences between the levels
            int ll = levelParams[0].initRandom(seed);
            for (int i = 1; i < 4; i++)
            {
                ll = levelParams[i].initRandom(ll);
            }

            // create a random generator for myself (used in stem_radius)
            random = new CS_Random(seed);

            // mesh settings
            if (Smooth <= 0.2)
            {
                smooth_mesh_level = -1;
            }
            else
            {
                smooth_mesh_level = (int)(Levels * Smooth);
            }
            mesh_quality = Smooth;

            // mesh points per cross-section for the levels
            // minima
            levelParams[0].mesh_points = 4;
            levelParams[1].mesh_points = 3;
            levelParams[2].mesh_points = 2;
            levelParams[3].mesh_points = 1;
            // set meshpoints with respect to mesh_quality and Lobes
            if (Lobes > 0)
            {
                levelParams[0].mesh_points = (int)(Lobes * (Math.Pow(2, (int)(1 + 2.5 * mesh_quality))));
                levelParams[0].mesh_points =
                    Math.Max(levelParams[0].mesh_points, (int)(4 * (1 + 2 * mesh_quality)));
            }
            for (int i = 1; i < 4; i++)
            {
                levelParams[i].mesh_points =
                    Math.Max(3, (int)(levelParams[i].mesh_points * (1 + 1.5 * mesh_quality)));
            }

            // stop generation at some level?
            if (stopLevel >= 0 && stopLevel <= Levels)
            {
                Levels = stopLevel;
                Leaves = 0;
            }

            scale_tree = Scale + levelParams[0].random.uniform(-ScaleV, ScaleV);
        }

        public float getShapeRatio(float ratio)
        {
            return getShapeRatio(ratio, Shape);
        }

        public float getShapeRatio(float ratio, int shape)
        {

            switch (shape)
            {
                //case CONICAL: return 0.2+0.8*ratio;
                // need real conical shape for lark, fir, etc.
                case CONICAL: return ratio; // FIXME: this would be better: 0.05+0.95*ratio; ?
                case SPHERICAL: return (float)(0.2 + 0.8 * Math.Sin(Math.PI * ratio));
                case HEMISPHERICAL: return (float)(0.2 + 0.8 * Math.Sin(0.5 * Math.PI * ratio));
                case CYLINDRICAL: return 1.0f;
                case TAPERED_CYLINDRICAL: return (float)(0.5 + 0.5 * ratio);
                case FLAME:
                    return ratio <= 0.7f ?
                            ratio / 0.7f :
                                (1 - ratio) / 0.3f;
                case INVERSE_CONICAL: return 1 - 0.8f * ratio;
                case TEND_FLAME:
                    return ratio <= 0.7f ?
                            0.5f + 0.5f * ratio / 0.7f :
                                0.5f + 0.5f * (1 - ratio) / 0.3f;
                case ENVELOPE:
                    if (ratio < 0 || ratio > 1)
                    {
                        return 0;
                    }
                    else if (ratio < (1 - PruneWidthPeak))
                    {
                        return (float)Math.Pow(ratio / (1 - PruneWidthPeak), PrunePowerHigh);
                    }
                    else
                    {
                        return (float)Math.Pow((1 - ratio) / (1 - PruneWidthPeak), PrunePowerLow);
                    }
                // tested in prepare() default: throw new ErrorParam("Shape must be between 0 and 8");
            }
            return 0; // shouldn't reach here
        }

        public void setParam(String name, String value)
        {
            CS_AbstractParam p = (CS_AbstractParam)paramDB[name];
            
            if (p != null)
            {
                p.setValue(value);
                raiseOnParamChanged("");
            }
            else
            {
                throw new Exception("Unknown parameter " + name + "!");
            }
        }

        /*
        public TreeMap getParamGroup(int level, String group)
        {
            TreeMap result = new TreeMap();
            for (Enumeration e = paramDB.elements(); e.hasMoreElements(); )
            {
                AbstractParam p = (AbstractParam)e.nextElement();
                if (p.getLevel() == level && p.getGroup().equals(group))
                {
                    result.put(new Integer(p.getOrder()), p);
                }
            }
            return result;
        }*/
        public SortedList<int, CS_AbstractParam> getParamGroup(int level, String group)
        {
            SortedList<int, CS_AbstractParam> result = new SortedList<int, CS_AbstractParam>();
            foreach (CS_AbstractParam p in paramDB.Values)
            {
                if (p.getLevel() == level && p.getGroup() == group)
                {
                    result.Add(p.getOrder(), p);
                }
            }


            return result;
        }

        // help methods for createing param-db

        int order;
        private void intParam(String name, int min, int max, int deflt,
                String group, String short_desc, String long_desc, String html_desc)
        {
            paramDB.Add(name, new CS_IntParam(name, min, max, deflt, group, CS_AbstractParam.GENERAL, order++, short_desc, long_desc, html_desc));
        }

        private void shapeParam(String name, int min, int max, int deflt,
                String group, String short_desc, String long_desc, string html)
        {
            paramDB.Add(name, new CS_ShapeParam(name, min, max, deflt, group, CS_AbstractParam.GENERAL, order++, short_desc, long_desc, html));
        }

        private void int4Param(String name, int min, int max,
                int deflt0, int deflt1, int deflt2, int deflt3,
                String group, String short_desc, String long_desc, string html_desc)
        {
            int[] deflt = { deflt0, deflt1, deflt2, deflt3 };
            order++;
            for (int i = 0; i < 4; i++)
            {
                String fullname = "" + i + name.Substring(1);
                paramDB.Add(fullname, new CS_IntParam(fullname, min, max, deflt[i], group, i,
                        order, short_desc, long_desc, html_desc));
            }
        }

        private void dblParam(String name, float min, float max, float deflt,
                String group, String short_desc, String long_desc, string html_desc)
        {
            paramDB.Add(name, new CS_FloatParam(name, min, max, deflt, group, CS_AbstractParam.GENERAL,
                    order++, short_desc, long_desc, html_desc));
        }

        private void dbl4Param(String name, float min, float max,
                float deflt0, float deflt1, float deflt2, float deflt3,
                String group, String short_desc, String long_desc, string html_desc)
        {
            float[] deflt = { deflt0, deflt1, deflt2, deflt3 };
            order++;
            for (int i = 0; i < 4; i++)
            {
                String fullname = "" + i + name.Substring(1);
                paramDB.Add(fullname, new CS_FloatParam(fullname, min, max, deflt[i], group, i,
                        order, short_desc, long_desc, html_desc));
            }
        }

        private void lshParam(String name, String deflt,
                String group, String short_desc, String long_desc, string html_desc)
        {
            paramDB.Add(name, new CS_LeafShapeParam(name, deflt, group, CS_AbstractParam.GENERAL,
                    order++, short_desc, long_desc, html_desc));
        }

        private void strParam(String name, String deflt,
                String group, String short_desc, String long_desc, string html_desc)
        {
            paramDB.Add(name, new CS_StringParam(name, deflt, group, CS_AbstractParam.GENERAL,
                    order++, short_desc, long_desc, html_desc));
        }

        private void registerParams()
        {
            order = 1;

            strParam("Species", "default",
                    "SHAPE", "The tree's species",
                    "<p><strong>Species</strong> is the kind of tree.</p>" +
                    "<p>It is used for declarations in the output file.</p>","");

            shapeParam("Shape", 0, 8, 0, "SHAPE", "General tree shape id",
                    "<p>The <strong>Shape</strong> can be one of:</p><ul>\n" +
                    "<li>0 - conical</li>\n" +
                    "<li>1 - spherical</li>\n" +
                    "<li>2 - hemispherical</li>\n" +
                    "<li>3 - cylindrical</li>\n" +
                    "<li>4 - tapered cylindrical</li>\n" +
                    "<li>5 - flame</li>\n" +
                    "<li>6 - inverse conical</li>\n" +
                    "<li>7 - tend flame</li>\n" +
                    "<li>8 - envelope - uses pruning envelope\n" +
                    "(see PruneWidth, PruneWidthPeak, PrunePowerLow, PrunePowerHigh)</li></ul></p>\n",
                    ""
            );

            intParam("Levels", 0, 9, 2, "SHAPE", "Levels of recursion",
                    "<p><strong>Levels</strong> are the levels of recursion when creating the\n" +
                    "stems of the tree.</p><ul>\n" +
                    "<li>Levels=1 means the tree consist only of the (may be splitting) trunk</li>\n" +
                    "<li>Levels=2 the tree consist of the trunk with one level of branches</li>\n" +
                    "<li>Levels>4 seldom necessary, the parameters of the forth level are used\n" +
                    "for all higher levels.</li></ul>\n" +
                    "<p>Leaves are considered to be one level above the last stem level, \n" +
                    "and uses it's down and rotation angles.</p>\n",
                    ""
            );

            dblParam("Scale", 0.000001f, float.PositiveInfinity, 10.0f, "SHAPE", "Average tree size in meters",
                    "<p><strong>Scale</strong> is the average tree size in meters.</p>\n" +
                    "<p>With Scale = 10.0 and ScaleV = 2.0 trees of this species\n" +
                    "reach from 8.0 to 12.0 meters.</p>\n" +
                    "<p>Note, that the trunk length can be different from the tree size.\n" +
                    "(See 0Length and 0LengthV)</p>\n",
                    ""
            );

            dblParam("ScaleV", 0.0f, float.PositiveInfinity, 0.0f, "SHAPE", "Variation of tree size in meters",
                    "<p><strong>ScaleV</strong> is the variation range of the tree size in meters.</p>\n" +
                    "<p>Scale = 10.0, ScaleV = 2.0 means trees of this species\n" +
                    "reach from 8.0 to 12.0 meters.\n" +
                    "(See Scale)</p>\n",
                    ""
            );

            dblParam("BaseSize", 0.0f, 1.0f, 0.25f, "SHAPE", "Fractional branchless area at tree base",
                    "<p><strong>BaseSize</strong> is the fractional branchless part of the trunk. E.g.</p>\n<ul>" +
                    "<li>BaseSize=&nbsp;&nbsp;0</code> means branches begin on the bottom of the tree,</li>\n" +
                    "<li>BaseSize=0.5</code> means half of the trunk is branchless,</li>\n" +
                    "<li>BaseSize=1.0</code> branches grow out from the peak of the trunk only.</li></ul>\n",
                    ""
            );

            intParam("0BaseSplits", 0, int.MaxValue, 0, "SHAPE",
                    "Stem splits at base of trunk",
                    "<p><strong>BaseSplits</strong> are the stem splits at the top of the first trunk segment.</p>\n" +
                    "<p>So with BaseSplits=2 you get a trunk splitting into three parts. Other then\n" +
                    "with 0SegSplits the clones are evenly distributed over\n" +
                    "the 360&deg;. So, if you want to use splitting, you should\n" +
                    "use BaseSplits for the first splitting to get a circular\n" +
                    "stem distribution (seen from top).</p>\n",
                    ""
            );

            //		dblParam("ZScale",0.000001,Double.POSITIVE_INFINITY,1.0,"SHAPE",
            //				"additional Z-scaling (not used)<br>",
            //				"<strong>ZScale</strong> and ZScaleV are not described in the Weber/Penn paper.<br>\n"+
            //				"so theire meaning is unclear and they aren't used at the moment\n"
            //		);
            //		
            //		dblParam("ZScaleV",0.0,Double.POSITIVE_INFINITY,0.0,"SHAPE",
            //				"additional Z-scaling variation (not used)<br>",
            //				"ZScale and <strong>ZScaleV</strong> are not described in the Weber/Penn paper.<br>\n"+
            //				"so theire meaning is unclear and they aren't used at the moment\n"
            //		);

            dblParam("Ratio", 0.000001f, float.PositiveInfinity, 0.05f, "TRUNK",
                    "Trunk radius/length ratio",
                    "<p><strong>Ratio</strong> is the radius/length ratio of the trunk.</p>\n" +
                    "<p>Ratio=0.05 means the trunk is 1/20 as thick as it is long,\n" +
                    "t.e. a 10m long trunk has a base radius of 50cm.</p>\n" +
                    "<p>Note, that the real base radius could be greater, when Flare\n" +
                    "and/or Lobes are used. (See Flare, Lobes, LobesDepth, RatioPower)</p>\n",
                    ""
            );

            dblParam("RatioPower", float.NegativeInfinity, float.PositiveInfinity, 1.0f,
                    "SHAPE", "Radius reduction",
                    "<p><strong>RatioPower</strong> is a reduction value for the radius of the\n" +
                    "substems.\n</p><ul>" +
                    "<li>RatioPower=1.0  means the radius decreases linearly with\n" +
                    "decreasing stem length</li>\n" +
                    "<li>RatioPower=2.0  means it decreases with the second power</li>\n" +
                    "<li>RatioPower=0.0  means radius is the same as parent radius\n" +
                    "(t.e. it doesn't depend of the length)</li>\n" +
                    "<li>RatioPower=-1.0 means the shorter the stem the thicker it is\n" +
                    "(radius = parent radius * 1 / length)</li></ul>\n" +
                    "<p>Note, that the radius of a stem cannot be greater then the parent radius at the stem offset.</p>\n" +
                    "<p>So with negative RatioPower you cannot create stems thicker than it's parent.</p>\n" +
                    "<p>Instead you can use it to make stems thinner, which are longer than it's parent.\n" +
                    "(See Ratio)</p>\n",
                    ""
            );

            dblParam("Flare", -1.0f, float.PositiveInfinity, 0.5f,
                    "TRUNK", "Exponential expansion at base of tree",
                    "<p><strong>Flare</strong> makes the trunk base thicker.</p><ul>\n" +
                    "<li>Flare = 0.0 means base radius is used at trunk base</li>\n" +
                    "<li>Flare = 1.0 means trunk base is twice as thick as it's base radius\n" +
                    "(See Ratio)</li></ul>\n" +
                    "<p>Note, that using Lobes make the trunk base thicker too.\n" +
                    "(See Lobes, LobeDepth)</p>\n",
                    ""
            );

            intParam("Lobes", 0, int.MaxValue, 0, "TRUNK",
                    "Sinusoidal cross-section variation",
                    "<p>With <strong>Lobes</strong> you define how much lobes (this are variations in it's\n" +
                    "cross-section) the trunk will have. This isn't supported for\n" +
                    "cones output, but for mesh only.\n" +
                    "(See LobeDepth too)</p>\n",
                    ""
            );

            dblParam("LobeDepth", 0, float.PositiveInfinity, 0,
                    "TRUNK", "Amplitude of cross-section variation",
                    "<p><strong>LobeDepth</strong> defines, how deep the lobes of the trunk will be.</p>\n" +
                    "<p>This is the amplitude of the sinusoidal cross-section variations.<br>\n" +
                    "(See Lobes)</p>\n",
                    ""
            );

            intParam("Leaves", int.MinValue, int.MaxValue, 0,
                    "LEAVES", "Number of leaves per stem",
                    "<p><strong>Leaves</strong> gives the maximal number of leaves per stem.</p>\n" +
                    "<p>Leaves grow only from stems of the last level. The actual number of leaves on a stem,\n" +
                    "depending on the stem offset and length, can be smaller than Leaves.</p>\n" +
                    "<p>When Leaves is negative, the leaves grow in a fan at\n" +
                    "the end of the stem.</p>\n",
                    ""
            );

            lshParam("LeafShape", "0", "LEAVES", "Leaf shape id",
                    "<p><strong>LeafShape</strong> is the shape of the leaf (\"0\" means oval shape).</p>\n" +
                    "<p>The length and width of the leaf are given by LeafScale and LeafScaleX.</p>\n" +

                    "<p>When creating a mesh at the moment you can use the following values:</p><ul>\n" +
                    "<li>\"disc\" - a surface consisting of 6 triangles approximating an oval shape</li>\n" +
                    "<li>\"sphere\" - an ikosaeder approximating a shperical shape,\n" +
                    "useful for making knots or seeds instead of leaves, or for high quality needles</li>\n" +
                    "<li>\"disc1\", \"disc2\", ... - a surface consisting of 1, 2, ... triangles approximating an oval shape\n" +
                    "lower values are useful for low quality needles or leaves, to reduce mesh size,\n" +
                    "values between 6 and 10 are quite good for big, round leaves.</li>\n" +
                    "<li>any other - same like disc</li></ul>\n" +

                    "<p>When using primitives output, the possible values of LeafShape references\n" +
                    "the declarations in arbaro.inc. At the moment there are:</p><ul>\n" +
                    "<li>\"disc\" the standard oval form of a leaf, defined\n" +
                    "as a unit circle of radius 0.5m. The real\n" +
                    "length and width are given by the LeafScale parameters.</li>\n" +
                    "<li>\"sphere\" a spherical form, you can use to\n" +
                    "simulate seeds on herbs or knots on branches like in the\n" +
                    "desert bush. You can use the sphere shape for needles too,\n" +
                    "thus they are visible from all sides</li>\n" +
                    "<li>\"palm\" a palm leaf, this are two disc halfs put together\n" +
                    "with an angle between them. So they are visible\n" +
                    "also from the side and the light effects are\n" +
                    "more typically, especialy for fan palms seen from small distances.</li>\n" +
                    "<li>any other - add your own leaf shape to the file arbaro.inc</li></ul>\n",
                    ""
            );

            dblParam("LeafScale", 0.000001f, float.PositiveInfinity, 0.2f,
                    "LEAVES", "Leaf length",
                    "<p><strong>LeafScale</strong> is the length of the leaf in meters.</p>\n" +
                    "<p>The unit leaf is scaled in z-direction (y-direction in Povray)\n" +
                    "by this factor. (See LeafShape, LeafScaleX)</p>\n",
                    ""
            );

            dblParam("LeafScaleX", 0.000001f, float.PositiveInfinity, 0.5f, "LEAVES",
                    "Fractional leaf width",
                    "<p><strong>LeafScaleX</strong> is the fractional width of the leaf relativly to it's length. So</p><ul>\n" +
                    "<li>LeafScaleX=0.5 means the leaf is half as wide as long</li>\n" +
                    "<li>LeafScaleX=1.0 means the leaf is like a circle</li></ul>\n" +
                    "<p>The unit leaf is scaled by LeafScale*LeafScaleX in x- and\n" +
                    "y-direction (x- and z-direction in Povray).\n" +
                    "So the spherical leaf is transformed to a needle 5cm long and\n" +
                    "1mm wide by LeafScale=0.05 and LeafScaleX=0.02.</p>\n",
                    ""
            );

            dblParam("LeafBend", 0, 1, 0.3f, "LEAVES", "Leaf orientation toward light",
                    "<p>With <strong>LeafBend</strong> you can influence, how much leaves are oriented\n" +
                    "outside and upwards.<br>Values near 0.5 are good. For low values the leaves\n" +
                    "are oriented to the stem, for high value to the light.</p>\n" +
                    "<p>For trees with long leaves like palms you should use lower values.</p>\n",
                    ""
            );

            dblParam("LeafStemLen", float.NegativeInfinity, float.PositiveInfinity, 0.5f,
                    "LEAVES", "Fractional leaf stem length",
                    "<p><strong>LeafStemLen</strong is the length of the (virtual) leaf stem.</p>\n" +
                    "<p>It's not drawn, so this is the distance between the stem\n" +
                    "axis and the leaf. For normal trees with many nearly circular\n" +
                    "leaves the default value of 0.5 (meaning the stem has half of the length\n" +
                    "of the leaf) is quite good. For other trees like palms with long leaves\n" +
                    "or some herbs you need a LeafStemLen near 0. Negative stem length is\n" +
                    "allowed for special cases.</p>",
                    ""
            );

            intParam("LeafDistrib", 0, 8, 4, "LEAVES", "Leaf distribution",
                    "<p><strong>LeafDistrib</strong> determines how leaves are distributed over\n" +
                    "the branches of the last but one stem level. It takes the same\n" +
                    "values like Shape, meaning 3 = even distribution, 0 = most leaves\n" +
                    "outside. Default is 4 (some inside, more outside).</p>",
                    ""
            );

            dblParam("LeafQuality", 0.000001f, 1.0f, 1.0f, "QUALITY", "Leaf quality/leaf count reduction",
                    "<p>With a <strong>LeafQuality</strong> less then 1.0 you can reduce the number of leaves\n" +
                    "to improve rendering speed and memory usage. The leaves are scaled\n" +
                    "with the same amount to get the same coverage.</p>\n" +
                    "<p>For trees in the background of the scene you will use a reduced\n" +
                    "LeafQuality around 0.9. Very small values would cause strange results.\n" +
                    "(See LeafScale)</p>",
                    ""
            );

            dblParam("Smooth", 0.0f, 1.0f, 0.5f, "QUALITY", "Smooth value for mesh creation",
                    "<p>Higher <strong>Smooth</strong> values creates meshes with more vertices and\n" +
                    "adds normal vectors to them for some or all branching levels.</p>\n" +
                    "<p>Normally you would specify this value on the command line or in\n" +
                    "the rendering dialog, but for some species a special default\n" +
                    "smooth value could be best. E.g. for shave-grass a low smooth value\n" +
                    "is preferable, because this herb has angular stems.</p>",
                    ""
            );

            dblParam("AttractionUp", float.NegativeInfinity, float.PositiveInfinity, 0.0f,
                    "SHAPE", "Upward/downward growth tendency",
                    "<p><strong>AttractionUp</strong> is the tendency of stems with level>=2 to grow upwards\n" +
                    "(downwards for negative values).</p>\n" +
                    "<p>A value of 1.0 for a horizontal stem means the last segment should point upwards.</p>\n" +
                    "<p>Greater values means earlier reaching of upward direction. Values of 10 and greater\n" +
                    "could cause overcorrection resulting in a snaking oscillation.</p>\n" +
                    "<p>As an example see the weeping willow, which has a negative AttractionUp value.</p>\n",
                    ""
            );

            dblParam("PruneRatio", 0.0f, 1.0f, 0.0f, "PRUNING",
                    "Fractional effect of pruning",
                    "<p>A <strong>PruneRatio</strong> of 1.0 means all branches are inside\n" +
                    "the envelope. 0.0 means no pruning.</p>\n",
                    ""
            );

            dblParam("PruneWidth", 0.0f, 1.0f, 0.5f, "PRUNING", "Width of envelope peak",
                    "<p><strong>PruneWidth</strong> is the fractional width of the pruning envelope at the\n" +
                    "peak. A value of 0.5 means the tree is half as wide as high.</p>\n" +
                    "<p>This parameter is used for the shape \"envelope\" too, even if PruneRatio is off.</p>\n",
                    ""
            );

            dblParam("PruneWidthPeak", 0.0f, 1.0f, 0.5f, "PRUNING", "Position of envelope peak",
                    "<p><strong>PruneWidthPeak</strong> is the fractional height of the envelope peak.</p>\n" +
                    "<p>A value of 0.5 means upper part and lower part of the envelope have the same height.</p>\n" +
                    "<p>This parameter is used for the shape \"envelope\" too, even if PruneRatio is off.</p>\n",
                    ""
            );

            dblParam("PrunePowerLow", 0.0f, float.PositiveInfinity, 0.5f, "PRUNING",
                    "Curvature of envelope",
                    "<p><strong>PrunePowerLow</strong> describes the envelope curve below the peak.</p>\n" +
                    "<p>A value of 1 means linear decreasing. Higher values means concave,\n" +
                    "lower values convex curve.</p>\n" +
                    "<p>This parameter is used for the shape \"envelope\" too, even if PruneRatio is off.</p>\n",
                    ""
            );

            dblParam("PrunePowerHigh", 0.0f, float.PositiveInfinity, 0.5f, "PRUNING",
                    "Curvature of envelope",
                    "<p><strong>PrunePowerHigh</strong> describes the envelope curve above the peak.</p>\n" +
                    "<p>A value of 1 means linear decreasing. Higher values means concave,\n" +
                    "lower values convex curve.</p>\n" +
                    "<p>This parameter is used for the shape \"envelope\" too, even if PruneRatio is off.</p>\n",
                    ""
            );

            dblParam("0Scale", 0.000001f, float.PositiveInfinity, 1.0f,
                    "TRUNK", "Extra trunk scaling",
                    "<p><strong>0Scale</strong> and 0ScaleV makes the trunk thicker.</p>\n" +
                    "<p>This parameters exists for the level 0 only. From the Weber/Penn paper it is\n" +
                    "not clear, why there are two trunk scaling parameters \n" +
                    "0Scale and Ratio. See Ratio, 0ScaleV, Scale, ScaleV.</p>\n" +
                    "<p>In this implementation 0Scale does not influence the trunk base radius\n" +
                    "but is applied finally to the stem radius formular. Thus the\n" +
                    "trunk radius could be influenced independently from the\n" +
                    "Ratio/RatioPower parameters and the periodic tapering (0Taper > 2.0)\n" +
                    "could be scaled, so that the sections are elongated spheres.</p>\n",
                    ""
            );

            dblParam("0ScaleV", 0.0f, float.PositiveInfinity, 0.0f, "TRUNK",
                    "Variation for extra trunk scaling",
                    "<p>0Scale and <strong>0ScaleV</strong> makes the trunk thicker. This parameters\n" +
                    "exists for the level 0 only. From the Weber/Penn paper it is\n" +
                    "not clear, why there are two trunk scaling parameters\n" +
                    "0Scale and Ratio. See Ratio, 0ScaleV, Scale, ScaleV.</p>\n" +
                    "<p>In this implementation 0ScaleV is used to perturb the\n" +
                    "mesh of the trunk. But use with care, because the mesh\n" +
                    "could got fissures when using too big values.</p>\n",
                    ""
            );

            dbl4Param("nLength", 0.0000001f, float.PositiveInfinity, 1.0f, 0.5f, 0.5f, 0.5f,
                    "LENTAPER", "Fractional trunk scaling",
                    "<p><strong>0Length</strong> and 0LengthV give the fractional length of the\n" +
                    "trunk. So with Scale=10 and 0Length=0.8 the length of the\n" +
                    "trunk will be 8m. Dont' confuse the height of the tree with\n" +
                    "the length of the trunk here.</p>\n" +
                    "<p><strong>nLength</strong> and nLengthV define the fractional length of a stem\n" +
                    "relating to the length of theire parent.</p>\n",
                    ""
            );

            dbl4Param("nLengthV", 0.0f, float.PositiveInfinity, 0.0f, 0.0f, 0.0f, 0.0f,
                    "LENTAPER", "variation of fractional trunk scaling",
                    "<p><strong>nLengthV</strong> is the variation of the length given by nLength.</p>\n",
                    ""
            );

            dbl4Param("nTaper", 0.0f, 2.99999999f, 1.0f, 1.0f, 1.0f, 1.0f,
                    "LENTAPER", "Cross-section scaling",
                    "<p><strong>nTaper</strong> is the tapering of the stem along its length.</p><ul>\n" +
                    "<li>0 - non-tapering cylinder</li>\n" +
                    "<li>1 - taper to a point (cone)</li>\n" +
                    "<li>2 - taper to a spherical end</li>\n" +
                    "<li>3 - periodic tapering (concatenated spheres)</li></ul>\n" +
                    "<p>You can use fractional values, to get intermediate results.</p>\n",
                    ""
            );

            dbl4Param("nSegSplits", 0, float.PositiveInfinity, 0, 0, 0, 0,
                    "SPLITTING", "Stem splits per segment",
                    "<p><strong>nSegSplits</strong> determines how much splits per segment occures.</p>\n" +
                    "<p>Normally you would use a value between 0.0 and 1.0. A value of\n" +
                    "0.5 means a split at every second segment. If you use splitting\n" +
                    "for the trunk you should use 0BaseSplits for the first split, \n" +
                    "otherwise the tree will tend to one side.</p>",
                    ""
            );

            dbl4Param("nSplitAngle", 0, 180, 0, 0, 0, 0, "SPLITTING",
                    "Splitting angle",
                    "<p><strong>nSplitAngle</strong> is the vertical splitting angle. A horizontal diverging\n" +
                    "angle will be added too, but this one you cannot influence with parameters.</p>\n" +
                    "<p>The declination of the splitting branches won't exceed the splitting angle.</p>\n",
                    ""
            );

            dbl4Param("nSplitAngleV", 0, 180, 0, 0, 0, 0, "SPLITTING",
                    "Splitting angle variation",
                    "<p><strong>nSplitAngleV</strong> is the variation of the splitting angle. See nSplitAngle.</p>\n",
                    ""
            );

            int4Param("nCurveRes", 1, int.MaxValue, 3, 3, 1, 1,
                    "CURVATURE", "Curvature resolution",
                    "<p><strong>nCurveRes</strong> determines how many segments the branches consist of.</p>\n" +
                    "<p>Normally you will use higher values for the first levels, and low\n" +
                    "values for the higher levels.</p>\n",
                    ""
            );

            dbl4Param("nCurve", float.NegativeInfinity, float.PositiveInfinity, 0, 0, 0, 0,
                    "CURVATURE", "Curving angle",
                    "<p><strong>nCurve</strong> is the angle the branches are declined over theire whole length.</p>\n" +
                    "<p>If nCurveBack is used, the curving angle is distributed only over the\n" +
                    "first half of the stem.</p>\n",
                    ""
            );

            dbl4Param("nCurveV", -90, float.PositiveInfinity, 0, 0, 0, 0,
                    "CURVATURE", "Curving angle variation",
                    "<p><strong>nCurveV</strong> is the variation of the curving angle. See nCurve, nCurveBack.</p>\n" +
                    "<p>A negative value means helical curvature</p>\n",
                    ""
            );

            dbl4Param("nCurveBack", float.NegativeInfinity, float.PositiveInfinity, 0, 0, 0, 0,
                    "CURVATURE", "Curving angle upper stem half",
                    "<p>Using <strong>nCurveBack</strong> you can give the stem an S-like shape.</p>\n" +
                    "<p>The first half of the stem the nCurve value is applied.</p>\n" +
                    "<p>The second half the nCurveBack value.</p>\n" +
                    "<p>It's also possible to give both parametera the same sign to\n" +
                    "get different curving over the stem length, instead of a S-shape</p>\n",
                    ""
            );

            dbl4Param("nDownAngle", -179.9999999f, 179.999999f, 0, 30, 30, 30,
                    "BRANCHING", "Angle from parent",
                    "<p><strong>nDownAngle</strong> is the angle between a stem and it's parent.</p>\n",
                    ""
            );

            dbl4Param("nDownAngleV", -179.9999999f, 179.9999999f, 0, 0, 0, 0,
                    "BRANCHING", "Down angle variation",
                    "<p><strong>nDownAngleV</strong> is the variation of the downangle. See nDownAngle.</p>\n" +
                    "<p>Using a negative value, the nDownAngleV is variated over the\n" +
                    "length of the stem, so that the lower branches have a bigger\n" +
                    "downangle then the higher branches.</p>\n",
                    ""
            );

            dbl4Param("nRotate", -360, 360, 0, 120, 120, 120,
                    "BRANCHING", "Spiraling angle",
                    "<p><strong>nRotate</strong> is the angle, the branches are rotating around the parent</p>\n" +
                    "<p>If nRotate is negative the branches are located on alternating\n" +
                    "sides of the parent.</p>\n",
                    ""
            );

            dbl4Param("nRotateV", -360, 360, 0, 0, 0, 0,
                    "BRANCHING", "Spiraling angle variation",
                    "<p><strong>nRotateV</strong> is the variation of nRotate.</p>\n",
                    ""
            );

            int4Param("nBranches", 0, int.MaxValue, 1, 10, 5, 5,
                    "BRANCHING", "Number of branches",
                    "<p><strong>nBranches</strong> is the maximal number of branches on a parent stem.</p>\n" +
                    "<p>The number of branches are reduced proportional to the\n" +
                    "relative length of theire parent.</p>\n",
                    ""
            );

            dbl4Param("nBranchDist", 0, 1, 0, 1, 1, 1,
                    "BRANCHING", "Branch distribution along the segment",
                    "<p><strong>nBranchDist</strong> is an additional parameter of Arbaro. It influences the\n" +
                    "distribution of branches over a segment of the parent stem.</p>\n" +
                    "<p>With 1.0 you get evenly distribution of branches like in the\n" +
                    "original model. With 0.0 all branches grow from the segments\n" +
                    "base like for conifers.</p>\n",
                    ""
            );


            //		outParam("OutFormat",MESH,CONES,MESH,
            //				"RENDER","the output file format",
            //				"<strong>OutFormat</strong> defines the format of the outputfile for rendering.<br>\n");
            //		
            //		intParam("RenderWidth",15,6000,600,
            //				"RENDER","the width of the rendered image",
            //				"<strong>RenderWidth</strong> is the width of the rendered image,<br>\n"+
            //				"if you render a scene with the tree from Arbaro.");
            //
            //		intParam("RenderHeight",20,8000,800,
            //				"RENDER","the height of the rendered image",
            //				"<strong>RenderHeight</strong> is the height of the rendered image,<br>\n"+
            //				"if you render a scene with the tree from Arbaro.");
            //		
            //		intParam("Seed",0,Integer.MAX_VALUE,13,
            //				"RENDER","the random seed",
            //				"<strong>Seed</strong> is the seed for initializing the random generator<br>\n"+
            //				"making the tree individual. So you can think of it as the tree's seed too.");

        }

        public void readFromXML(string filename)
        {
            try
            {              
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNodeList elemList = doc.GetElementsByTagName("species");
                setParam("Species", elemList[0].Attributes.GetNamedItem("name").Value);

                elemList = doc.GetElementsByTagName("param");
                foreach (XmlNode n in elemList) {
                    setParam(n.Attributes.GetNamedItem("name").Value, n.Attributes.GetNamedItem("value").Value);
                }             
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CS_AbstractParam getParam(String parname)
        {
            return (CS_AbstractParam)paramDB[parname];
        }

        public void raiseOnParamChanged(string p)
        {
            EventHandler<CS_ParamChangedArgs> handler = OnParamChanged;

            if (handler != null)
            {
                handler(null, new CS_ParamChangedArgs(p));
            }
        }
 

        /**
         * Enables or disables params depending on other
         * params values. 
         */
        public void enableDisable()
        {
            bool enable;

            // ############ general params ##############

            // disable Z-Scale parameters (they are not used)
            //		getParam("ZScale").setEnabled(false);
            //		getParam("ZScaleV").setEnabled(false);

            // enable RatioPower/Leaves if Levels>1
            enable = (((CS_IntParam)getParam("Levels")).intValue() > 1);
            getParam("RatioPower").setEnabled(enable);
            getParam("Leaves").setEnabled(enable);

            // enable leaf params if Leaves != 0
            enable = (((CS_IntParam)getParam("Leaves")).intValue() != 0 &&
                    ((CS_IntParam)getParam("Levels")).intValue() > 1);
            getParam("LeafShape").setEnabled(enable);
            getParam("LeafScale").setEnabled(enable);
            getParam("LeafScaleX").setEnabled(enable);
            getParam("LeafBend").setEnabled(enable);
            getParam("LeafDistrib").setEnabled(enable);
            getParam("LeafQuality").setEnabled(enable);
            getParam("LeafStemLen").setEnabled(enable);

            // enable Pruning parameters, if PruneRatio>0 or Shape=envelope
            enable = (((CS_IntParam)getParam("Shape")).intValue() == 8 ||
                    ((CS_FloatParam)getParam("PruneRatio")).doubleValue() > 0);
            getParam("PrunePowerHigh").setEnabled(enable);
            getParam("PrunePowerLow").setEnabled(enable);
            getParam("PruneWidth").setEnabled(enable);
            getParam("PruneWidthPeak").setEnabled(enable);

            // enable LobeDepth if Lobes>0
            enable = (((CS_IntParam)getParam("Lobes")).intValue() > 0);
            getParam("LobeDepth").setEnabled(enable);

            // enable AttractionUp if Levels>2
            enable = (((CS_IntParam)getParam("Levels")).intValue() > 2);
            getParam("AttractionUp").setEnabled(enable);

            // ############## disable unused levels ###########

            for (int i = 0; i < 4; i++)
            {

                enable = i < ((CS_IntParam)getParam("Levels")).intValue();

                getParam("" + i + "Length").setEnabled(enable);
                getParam("" + i + "LengthV").setEnabled(enable);
                getParam("" + i + "Taper").setEnabled(enable);

                getParam("" + i + "Curve").setEnabled(enable);
                getParam("" + i + "CurveV").setEnabled(enable);
                getParam("" + i + "CurveRes").setEnabled(enable);
                getParam("" + i + "CurveBack").setEnabled(enable);

                getParam("" + i + "SegSplits").setEnabled(enable);
                getParam("" + i + "SplitAngle").setEnabled(enable);
                getParam("" + i + "SplitAngleV").setEnabled(enable);

                getParam("" + i + "BranchDist").setEnabled(enable);
                getParam("" + i + "Branches").setEnabled(enable);

                // down and rotation angle of last level are
                // used for leaves
                enable = enable ||
                (((CS_IntParam)getParam("Leaves")).intValue() != 0 &&
                        i == ((CS_IntParam)getParam("Levels")).intValue());

                getParam("" + i + "DownAngle").setEnabled(enable);
                getParam("" + i + "DownAngleV").setEnabled(enable);
                getParam("" + i + "Rotate").setEnabled(enable);
                getParam("" + i + "RotateV").setEnabled(enable);
            }

            for (int i = 0; i < ((CS_IntParam)getParam("Levels")).intValue() && i < 4; i++)
            {

                // enable nSplitAngle/nSplitAngleV if nSegSplits>0
                enable = (((CS_FloatParam)getParam("" + i + "SegSplits")).doubleValue() > 0) ||
                (i == 0 && ((CS_IntParam)getParam("0BaseSplits")).intValue() > 0);
                getParam("" + i + "SplitAngle").setEnabled(enable);
                getParam("" + i + "SplitAngleV").setEnabled(enable);

                // enable Curving parameters only when CurveRes>1
                enable = (((CS_IntParam)getParam("" + i + "CurveRes")).intValue() > 1);
                getParam("" + i + "Curve").setEnabled(enable);
                getParam("" + i + "CurveV").setEnabled(enable);
                getParam("" + i + "CurveBack").setEnabled(enable);
            }

        }
    }
}
