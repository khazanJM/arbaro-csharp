using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_TreeImpl : CS_Tree
    {
	public CS_Params csparams;
	int seed = 13;
	public override int getSeed() { return seed; }
	
	Object progress;
	
	long stemCount;
	long leafCount;
	public override long getStemCount() { return stemCount; }
	public override long getLeafCount() { return leafCount; }
	
	public void setStemCount(long cnt) { stemCount=cnt; }
	public void setLeafCount(long cnt) { leafCount=cnt; }

	// the trunks (one for trees, many for bushes)
	List<CS_StemImpl> trunks;
	double trunk_rotangle = 0;
	
	//Progress progress;
	
	CS_Vector maxPoint;
	CS_Vector minPoint;
    public override CS_Vector getMaxPoint() { return maxPoint; }
    public override CS_Vector getMinPoint() { return minPoint; }
	public override double getHeight() { return maxPoint.getZ(); }
	public override double getWidth() {
		return Math.Sqrt(Math.Max(
				 minPoint.getX()*minPoint.getX()
				+minPoint.getY()*minPoint.getY(),
				 maxPoint.getX()*maxPoint.getX()
				+maxPoint.getY()*maxPoint.getY())); 
	}
	
	/**
	 * Creates a new tree object 
	 */
	public CS_TreeImpl(int seed, CS_Params csparams) {
		this.csparams = csparams;
		this.seed = seed;
        trunks = new List<CS_StemImpl>();
		//newProgress();
	}
	
	/**
	 * Creates a new tree object copying the parameters
	 * from an other tree
	 * 
	 * @param other the other tree, from wich parameters are taken
	 */
	public CS_TreeImpl(CS_TreeImpl other) {
		csparams = new CS_Params(other.csparams);
		trunks = new List<CS_StemImpl>();
	}
	
	public void clear() {
        trunks = new List<CS_StemImpl>();
		//newProgress();
	}
	
	/**
	 * Generates the tree. The following collaboration diagram
	 * shows the recursion trough the make process:
	 * <p>
	 * <img src="doc-files/Tree-2.png" />
	 * <p> 
	 * 
	 * @throws Exception
	 */
	
	public void make(Object progress) {
		this.progress = progress;
		
		setupGenProgress();
		csparams.prepare(seed);
		maxPoint = new CS_Vector(-Double.MaxValue,-Double.MaxValue,-Double.MaxValue);
		minPoint = new CS_Vector(Double.MaxValue,Double.MaxValue,Double.MaxValue);
		
		Console.WriteLine("Tree species: " + csparams.Species + ", Seed: " + seed);
		Console.WriteLine("making " + csparams.Species + "(" + seed + ") ");
		
		// create the trunk and all its stems and leaves
		CS_Transformation transf = new CS_Transformation();
		CS_Transformation trf;
		double angle;
		double dist;
		CS_LevelParams lpar = csparams.getLevelParams(0);
		for (int i=0; i<lpar.nBranches; i++) {
			trf = trunkDirection(transf,lpar);
			angle = lpar.var(360);
			dist = lpar.var(lpar.nBranchDist);
			trf = trf.translate(new CS_Vector(dist*Math.Sin(angle),
					dist*Math.Cos(angle),0));
			CS_StemImpl trunk = new CS_StemImpl(this,null,0,trf,0);
			trunks.Add(trunk);
			trunk.index=0;
			trunk.make();
		}
		
		
		// set leafCount and stemCount for the tree
		if (csparams.Leaves==0) setLeafCount(0);
		else {
			CS_LeafCounter leafCounter = new CS_LeafCounter();
			traverseTree(leafCounter);
			setLeafCount(leafCounter.getLeafCount());
		}
		CS_StemCounter stemCounter = new CS_StemCounter();
		traverseTree(stemCounter);
		setStemCount(stemCounter.getStemCount());
		
		// making finished
		//Console.progressChar();

        // TODO
		//progress.endPhase();
	}
	

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableTree#traverseTree(net.sourceforge.arbaro.tree.TreeTraversal)
	 */
	
	CS_Transformation trunkDirection(CS_Transformation trf, CS_LevelParams lpar) {
		
		// get rotation angle
		double rotangle;
		if (lpar.nRotate>=0) { // rotating trunk
			trunk_rotangle = (trunk_rotangle + lpar.nRotate+lpar.var(lpar.nRotateV)+360) % 360;
			rotangle = trunk_rotangle;
		} else { // alternating trunks
			if (Math.Abs(trunk_rotangle) != 1) trunk_rotangle = 1;
			trunk_rotangle = -trunk_rotangle;
			rotangle = trunk_rotangle * (180+lpar.nRotate+lpar.var(lpar.nRotateV));
		}
		
		// get downangle
		double downangle;
		downangle = lpar.nDownAngle+lpar.var(lpar.nDownAngleV);
		
		return trf.rotxz(downangle,rotangle);
	}
	
	
	public override bool traverseTree(CS_TreeTraversal traversal) {
	    if (traversal.enterTree(this))  // enter this tree?
        {
            //TODO

            /*
             Enumeration stems = trunks.elements();
             while (stems.hasMoreElements())
                if (! ((CS_Stem)stems.nextElement()).traverseTree(traversal))
                        break;
             */
        }

        return traversal.leaveTree(this);
	}

	public void minMaxTest(CS_Vector pt) {
		maxPoint.setMaxCoord(pt);
		minPoint.setMinCoord(pt);
	}
	
	/*
	 void Tree::dump() const {
	 cout << "TREE:\n";
	 // trunk.dump();
	  }
	  */
	
	
	/**
	 * Writes out the parameters to an XML definition file
	 * 
	 * @param out The output stream
	 */
	
	public override void paramsToXML(StreamWriter swout)  {
		csparams.toXML(swout);
	}
	
	/**
	 * Returns the species name of the tree
	 * 
	 * @return the species name
	 */
	public override String getSpecies() {
		return csparams.getSpecies();
	}
	
	public override int getLevels() {
		return csparams.Levels;
	}
	
	public override String getLeafShape() {
		return csparams.LeafShape;
	}
	
	public override double getLeafWidth() {
		return csparams.LeafScale*csparams.LeafScaleX/Math.Sqrt(csparams.LeafQuality);
	}
	
	public override double getLeafLength() {
		return csparams.LeafScale/Math.Sqrt(csparams.LeafQuality);
	}
	
	public override double getLeafStemLength() {
		return csparams.LeafStemLen;
	}
	
	public override String getVertexInfo(int level) {
		return "vertices/section: " 
			+ csparams.getLevelParams(level).mesh_points + ", smooth: " 
			+ (csparams.smooth_mesh_level>=level? "yes" : "no");
	}

	
	/**
	 * Sets the maximum for the progress while generating the tree 
	 */
	
	public void setupGenProgress() {
		if (progress != null) {
			// max progress = trunks * trunk segments * (first level branches + 1) 
			long maxGenProgress = 
				((CS_IntParam)csparams.getParam("0Branches")).intValue()
				* ((CS_IntParam)csparams.getParam("0CurveRes")).intValue()
				* (((CS_IntParam)csparams.getParam("1Branches")).intValue()+1);
			
            // TODO
			//progress.beginPhase("Creating tree structure",maxGenProgress);
		}
	}
	
	/**
	 * Sets (i.e. calcs) the progress for the process of making the tree
	 * object.
	 */
	long genProgress;
	
	 // TODO ! synchronized
	 //public synchronized void updateGenProgress() {
        public void updateGenProgress() {
		try {
			// how much of 0Branches*0CurveRes*(1Branches+1) are created yet
			long sum = 0;
			for (int i=0; i<trunks.Count; i++) {
				CS_StemImpl trunk = ((CS_StemImpl)trunks[i]);
				if (trunk.substems != null) {
					sum += trunk.segments.Count * (trunk.substems.Count+1);
				} else {
					sum += trunk.segments.Count;
				}
			}
			
            // TODO Progress
            /*
			if (sum-genProgress > progress.getMaxProgress()/100) {
				genProgress = sum;
				progress.setProgress(genProgress);
			}*/
		} catch (Exception e) {
			Console.WriteLine(e.Message);
		}
	}

	
    }
}
