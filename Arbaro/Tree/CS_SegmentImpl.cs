using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.Arbaro.Tree
{
    public class CS_SegmentImpl : CS_StemSection
    {
	public CS_Params par;
	public CS_LevelParams lpar;
	public int index;
	public CS_Transformation transf;
	public double rad1;
	public double rad2;
	public double length;

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableSegment#getLength()
	 */
	public double getLength() { return length; }
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableSegment#getTransformation()
	 */
	public override CS_Transformation getTransformation() { return transf; }

	CS_StemImpl stem;
	
	// FIXME: use Enumeration instead of making this public
	public List<CS_SubsegmentImpl> subsegments;

    public CS_SegmentImpl(/*Params params, LevelParams lparams,*/
            CS_StemImpl stm, int inx, CS_Transformation trf, 
			double r1, double r2) {
		index = inx;
		transf = trf; 
		rad1 = r1;
		rad2 = r2;
		stem = stm;

		par = stem.par;
		lpar = stem.lpar;
		length = stem.segmentLength;
		
		// FIXME: rad1 and rad2 could be calculated only when output occurs (?)
		// or here in the constructor ?
		// FIXME: inialize subsegs with a better estimation of size
        subsegments = new List<CS_SubsegmentImpl>(10);
	}

    public void addSubsegment(CS_SubsegmentImpl ss)
    {
		if (subsegments.Count > 0) {
            CS_SubsegmentImpl p = ((CS_SubsegmentImpl)subsegments[(subsegments.Count - 1)]); 
			p.next = ss;
			ss.prev = p; 
		}
		subsegments.Add(ss);
	}
	

	void minMaxTest() {
		stem.minMaxTest(getUpperPosition());
		stem.minMaxTest(getLowerPosition());
	}
	
	/**
	 * Makes the segments from subsegments 
	 */
	
	public void make() {
		// FIXME: numbers for cnt should correspond to Smooth value
		// helical stem
		if (lpar.nCurveV<0) { 
			makeHelix(10);
		}
		
		// spherical end
		else if (lpar.nTaper > 1 && lpar.nTaper <=2 && isLastStemSegment()) {
			makeSphericalEnd(10);
		}
		
		// periodic tapering
		else if (lpar.nTaper>2) {
			makeSubsegments(20);
		}
		
		// trunk flare
		// FIXME: if nCurveRes[0] > 10 this division into several
		// subsegs should be extended over more then one segments?
		else if (lpar.level==0 && par.Flare!=0 && index==0) {
			
            /*
			if (Console.debug())
				stem.DBG("Segment.make() - flare");
			*/
			makeFlare(10);
			
		} else {
			makeSubsegments(1);
		}
		
		// FIXME: for helical stems maybe this test
		// should be made for all subsegments
		minMaxTest();
	}
	
	/**
	 * Creates susbsegments for the segment
	 * 
	 * @param cnt the number of subsegments
	 */
	
	private void makeSubsegments(int cnt) {
		CS_Vector dir = getUpperPosition().sub(getLowerPosition());
		for (int i=1; i<cnt+1; i++) {
			double pos = i*length/cnt;
			// System.err.println("SUBSEG:stem_radius");
			double rad = stem.stemRadius(index*length + pos);
			// System.err.println("SUBSEG: pos: "+ pos+" rad: "+rad+" inx: "+index+" len: "+length);
			
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition().add(dir.mul(pos/length)),rad, pos, this));
		}
	}
	
	/**
	 * Make a subsegments for a segment with spherical end
	 * (last stem segment), subsegment lengths decrements near
	 * the end to get a smooth surface
	 * 
	 * @param cnt the number of subsegments
	 */
	
	private void makeSphericalEnd(int cnt) {
		CS_Vector dir = getUpperPosition().sub(getLowerPosition());
		for (int i=1; i<cnt; i++) {
			double pos = length-length/Math.Pow(2,i);
			double rad = stem.stemRadius(index*length + pos);
			//stem.DBG("FLARE: pos: %f, rad: %f\n"%(pos,rad))
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition().add(dir.mul(pos/length)),rad, pos, this));
		}
		addSubsegment(new CS_SubsegmentImpl(getUpperPosition(),rad2,length, this));
	}
	
	/**
	 * Make subsegments for a segment with flare
	 * (first trunk segment). Subsegment lengths are decrementing
	 * near the base of teh segment to get a smooth surface
	 * 
	 * @param cnt the number of subsegments
	 */
	
	private void makeFlare(int cnt) {
		CS_Vector dir = getUpperPosition().sub(getLowerPosition());
		//addSubsegment(new SubsegmentImpl(getLowerPosition(),rad1,0,this));
		for (int i=cnt-1; i>=0; i--) {
			double pos = length/Math.Pow(2,i);
			double rad = stem.stemRadius(index*length+pos);
			//self.stem.DBG("FLARE: pos: %f, rad: %f\n"%(pos,rad))
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition().add(dir.mul(pos/length)),rad, pos,this));
		}
	}
	
	/**
	 * Make subsegments for a segment with helical curving.
	 * They curve around with 360° from base to top of the
	 * segment
	 * 
	 * @param cnt the number of subsegments, should be higher
	 *        when a smooth curve is needed.
	 */
	
	private void makeHelix(int cnt) {
		double angle = Math.Abs(lpar.nCurveV)/180*Math.PI;
		// this is the radius of the helix
		double rad = Math.Sqrt(1.0/(Math.Cos(angle)*Math.Cos(angle)) - 1)*length/Math.PI/2.0;
		
        /*
		if (Console.debug())
			stem.DBG("Segment.make_helix angle: "+angle+" len: "+length+" rad: "+rad);
		*/

		//self.stem.DBG("HELIX: rad: %f, len: %f\n" % (rad,len))
		for (int i=1; i<cnt+1; i++) {
			CS_Vector pos = new CS_Vector(rad*Math.Cos(2*Math.PI*i/cnt)-rad,
					rad*Math.Sin(2*Math.PI*i/cnt),
					i*length/cnt);
			//self.stem.DBG("HELIX: pos: %s\n" % (str(pos)))
			// this is the stem radius
			double srad = stem.stemRadius(index*length + i*length/cnt);
			addSubsegment(new CS_SubsegmentImpl(transf.apply(pos), srad, i*length/cnt,this));
		}
	}
	
	/**
	 * Calcs the position of a substem in the segment given 
	 * a relativ position where in 0..1 - needed esp. for helical stems,
	 * because the substems doesn't grow from the axis of the segement
	 *
	 * @param trf the transformation of the substem
	 * @param where the offset, where the substem spreads out
	 * @return the new transformation of the substem (shifted from
	 *        the axis of the segment to the axis of the subsegment)
	 */

    public CS_Transformation substemPosition(CS_Transformation trf, double where)
    {
		if (lpar.nCurveV>=0) { // normal segment 
			return trf.translate(transf.getZ().mul(where*length));
		} else { // helix
			// get index of the subsegment
			int i = (int)(where*(subsegments.Count-1));
			// interpolate position
            CS_Vector p1 = ((CS_SubsegmentImpl)subsegments[i]).pos;
            CS_Vector p2 = ((CS_SubsegmentImpl)subsegments[i + 1]).pos;
            CS_Vector pos = p1.add(p2.sub(p1).mul(where - i / (subsegments.Count - 1)));
			return trf.translate(pos.sub(getLowerPosition()));
		}
	}
	
	/**
	 * Position at the beginning of the segment
	 * 
	 * @return beginning point of the segment
	 */
    public CS_Vector getLowerPosition()
    {
		// self.stem.DBG("segmenttr0: %s, t: %s\n"%(self.transf_pred,self.transf_pred.t()))
		return transf.getT();
	}

    public override CS_Vector getPosition()
    {
		// self.stem.DBG("segmenttr0: %s, t: %s\n"%(self.transf_pred,self.transf_pred.t()))
		return transf.getT();
	}
	
	/**
	 * Position of the end of the segment
	 * 
	 * @return end point of the segment
	 */
    public CS_Vector getUpperPosition()
    {
		//self.stem.DBG("segmenttr1: %s, t: %s\n"%(self.transf,self.transf.t()))
		return transf.getT().add(transf.getZ().mul(length));
	}
	
	public int getIndex() {
		return index;
	}
	
	public int getSubsegmentCount() {
		return subsegments.Count;
	}
	
	public double getLowerRadius() {
		return rad1;
	}
	
	public override  double getRadius() {
		return rad1;
	}
	
	public override double getDistance() {
		return index*length;
	}

    public override CS_Vector getZ()
    {
		return transf.getZ();
	}


	public double getUpperRadius() {
		return rad2;
	}

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableSegment#isLastStemSegment()
	 */
	public bool isLastStemSegment() {
		// return index == lpar.nCurveRes-1;
		
		// use segmentCount, not segments.size, because clones
		// has less segments, but index starts from where the
		// clone grows out and ends with segmentCount
		return index == stem.segmentCount-1;
	}


    public override CS_Vector[] getSectionPoints()
    {
		int pt_cnt = lpar.mesh_points;
        CS_Vector[] points;
        CS_Transformation trf = getTransformation(); //segment.getTransformation().translate(pos.sub(segment.getLowerPosition()));
		double rad = this.rad1;
		
		// if radius = 0 create only one point
		if (rad<0.000001) {
            points = new CS_Vector[1];
            points[0] = trf.apply(new CS_Vector(0, 0, 0));
		} else { //create pt_cnt points
            points = new CS_Vector[pt_cnt];
			//stem.DBG("MESH+LOBES: lobes: %d, depth: %f\n"%(self.tree.Lobes, self.tree.LobeDepth))
			
			for (int i=0; i<pt_cnt; i++) {
				double angle = i*360.0/pt_cnt;
				// for Lobes ensure that points are near lobes extrema, but not exactly there
				// otherwise there are to sharp corners at the extrema
				if (lpar.level==0 && par.Lobes != 0) {
					angle -= 10.0/par.Lobes;
				}
				
				// create some point on the unit circle
                CS_Vector pt = new CS_Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180), 0);

				// scale it to stem radius
				if (lpar.level==0 && (par.Lobes != 0 || par._0ScaleV !=0)) {
					double rad1 = rad * (1 + 
							par.random.uniform(-par._0ScaleV,par._0ScaleV)/
							getSubsegmentCount());
					pt = pt.mul(rad1*(1.0+par.LobeDepth*Math.Cos(par.Lobes*angle*Math.PI/180.0))); 
				} else {
					pt = pt.mul(rad); // faster - no radius calculations
				}
				// apply transformation to it
				// (for the first trunk segment transformation shouldn't be applied to
				// the lower meshpoints, otherwise there would be a gap between 
				// ground and trunk)
				// FIXME: for helical stems may be/may be not a random rotation 
				// should applied additionally?
				
				pt = trf.apply(pt);
				points[i] = pt;
			}
		}
		
		return points;
	}
    }
}
