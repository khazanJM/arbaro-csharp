using Arbaro2.Arbaro.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public class CS_TreeGeneratorImpl : CS_TreeGenerator
    {
	CS_Params csparams;

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#makeTree(net.sourceforge.arbaro.export.Progress)
	 */
	public override CS_Tree makeTree(Object progress) {
		CS_TreeImpl tree = new CS_TreeImpl(seed, csparams);
		tree.make(progress);
		
		return tree;
	}
	
	private int seed = 13;

	public CS_TreeGeneratorImpl() {
		csparams = new CS_Params();
	}
	
	public CS_TreeGeneratorImpl(CS_Params csparams) {
		this.csparams = csparams;
	}

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#setSeed(int)
	 */
	public override void setSeed(int seed) {
		this.seed = seed;
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#getSeed()
	 */
	public override int getSeed() {
		return seed;
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#getParams()
	 */
	public override CS_Params getParams() {
		return csparams;
	}
	
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#setParam(java.lang.String, java.lang.String)
	 */
	public override void setParam(String csparam, String value) {
		csparams.setParam(csparam,value);
	}
	
	// TODO: not used at the moment, may be the GUI
	// should get a TreeGenerator as a ParamContainer
	// and tree maker, and not work directly with Params
	// class
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#getParam(java.lang.String)
	 */
	public override CS_AbstractParam getParam(String csparam) {
		return csparams.getParam(csparam);
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#getParamGroup(int, java.lang.String)
	 */
	// TODO: not used at the moment, may be the GUI
	// should get a TreeGenerator as a ParamContainer
	// and tree maker, and not work directly with Params
	// class
	public override SortedList<int, CS_AbstractParam> getParamGroup(int level, String group) {
		return csparams.getParamGroup(level,group);
	}

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#writeParamsToXML(java.io.PrintWriter)
	 */
	// TODO: not used at the moment, may be the GUI
	// should get a TreeGenerator as a ParamContainer
	// and tree maker, and not work directly with Params
	// class
	public override void writeParamsToXML(StreamWriter swout) {
		csparams.toXML(swout);
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#clearParams()
	 */
	public override void clearParams() {
		csparams.clearParams();
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#readParamsFromXML(java.io.InputStream)
	 */
	public override void readParamsFromXML(string filename) {
		csparams.readFromXML(filename);
	}
	
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TreeGenerator#readParamsFromCfg(java.io.InputStream)
	 */
        /*
	public void readParamsFromCfg(InputStream is) {
		params.readFromCfg(is);
	}
*/
    }
}
