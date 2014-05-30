using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  Ported from original Arbaro software

namespace Arbaro2.Arbaro.Tree
{
    public class CS_SegmentImpl : CS_StemSection
    {
	public CS_Params par;
	public CS_LevelParams lpar;
	public int index;
	public DX_Transformation transf;
    public float rad1;
    public float rad2;
    public float length;

	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableSegment#getLength()
	 */
    public float getLength() { return length; }
	/* (non-Javadoc)
	 * @see net.sourceforge.arbaro.tree.TraversableSegment#getTransformation()
	 */
	public override DX_Transformation getTransformation() { return transf; }

	CS_StemImpl stem;
	
	// FIXME: use Enumeration instead of making this public
	public List<CS_SubsegmentImpl> subsegments;

    public CS_SegmentImpl(/*Params params, LevelParams lparams,*/
            CS_StemImpl stm, int inx, DX_Transformation trf,
            float r1, float r2)
    {
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
		Vector3 dir = getUpperPosition()-getLowerPosition();
		for (int i=1; i<cnt+1; i++) {
            float pos = i * length / cnt;
			// System.err.println("SUBSEG:stem_radius");
            float rad = stem.stemRadius(index * length + pos);
			// System.err.println("SUBSEG: pos: "+ pos+" rad: "+rad+" inx: "+index+" len: "+length);
			
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition() + dir * (pos/length),rad, pos, this));
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
		Vector3 dir = getUpperPosition() -getLowerPosition();
		for (int i=1; i<cnt; i++) {
            float pos = (float)(length - length / Math.Pow(2, i));
            float rad = stem.stemRadius(index * length + pos);
			//stem.DBG("FLARE: pos: %f, rad: %f\n"%(pos,rad))
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition() + dir*(pos/length),rad, pos, this));
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
		Vector3 dir = getUpperPosition() -getLowerPosition();
		//addSubsegment(new SubsegmentImpl(getLowerPosition(),rad1,0,this));
		for (int i=cnt-1; i>=0; i--) {
            float pos = (float)(length / Math.Pow(2, i));
            float rad = stem.stemRadius(index * length + pos);
			//self.stem.DBG("FLARE: pos: %f, rad: %f\n"%(pos,rad))
			addSubsegment(new CS_SubsegmentImpl(getLowerPosition() +dir*(pos/length),rad, pos,this));
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
        float angle = (float)(Math.Abs(lpar.nCurveV) / 180 * Math.PI);
		// this is the radius of the helix
        float rad = (float)(Math.Sqrt(1.0 / (Math.Cos(angle) * Math.Cos(angle)) - 1) * length / Math.PI / 2.0);
		
        /*
		if (Console.debug())
			stem.DBG("Segment.make_helix angle: "+angle+" len: "+length+" rad: "+rad);
		*/

		//self.stem.DBG("HELIX: rad: %f, len: %f\n" % (rad,len))
		for (int i=1; i<cnt+1; i++) {
			Vector3 pos = new Vector3(rad*(float)Math.Cos(2*Math.PI*i/cnt)-rad, rad*(float)Math.Sin(2*Math.PI*i/cnt), i*length/cnt);
			//self.stem.DBG("HELIX: pos: %s\n" % (str(pos)))
			// this is the stem radius
            float srad = stem.stemRadius(index * length + i * length / cnt);
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

    public DX_Transformation substemPosition(DX_Transformation trf, float where)
    {
		if (lpar.nCurveV>=0) { // normal segment 
			return trf.translate(transf.getZ3() * (where*length));
		} else { // helix
			// get index of the subsegment
			int i = (int)(where*(subsegments.Count-1));
			// interpolate position
            Vector3 p1 = ((CS_SubsegmentImpl)subsegments[i]).pos;
            Vector3 p2 = ((CS_SubsegmentImpl)subsegments[i + 1]).pos;
            Vector3 pos = p1 + (p2 -p1)*(where - i / (subsegments.Count - 1));
			return trf.translate(pos - getLowerPosition());
		}
	}
	
	/**
	 * Position at the beginning of the segment
	 * 
	 * @return beginning point of the segment
	 */
    public Vector3 getLowerPosition()
    {
		// self.stem.DBG("segmenttr0: %s, t: %s\n"%(self.transf_pred,self.transf_pred.t()))
		return transf.getT();
	}

    public override Vector3 getPosition()
    {
		// self.stem.DBG("segmenttr0: %s, t: %s\n"%(self.transf_pred,self.transf_pred.t()))
		return transf.getT();
	}
	
	/**
	 * Position of the end of the segment
	 * 
	 * @return end point of the segment
	 */
    public Vector3 getUpperPosition()
    {
		//self.stem.DBG("segmenttr1: %s, t: %s\n"%(self.transf,self.transf.t()))
		return transf.getT() + transf.getZ3()*length;
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

    public override Vector3 getZ()
    {
		return transf.getZ3();
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


    // return points on the low part of the section 
    // I don't understand how the author is generating the upper section of the last
    // segment in the stem... Moreover using this function to create up and low part
    // can result in inconsistencies due to the random radius
    // the upper part of a segment would not match the lower part of its successor
    public override Vector3[] getSectionPoints(bool lower_section = true)
    {
		int pt_cnt = lpar.mesh_points;
        Vector3[] points;
        DX_Transformation trf = getTransformation(); //segment.getTransformation().translate(pos.sub(segment.getLowerPosition()));
        float rad = this.rad1;
        if (!lower_section) rad = rad2;
		
		// if radius = 0 create only one point -> that's a problem for me KhazanJM
		if (false /*rad<-0.00001*/) {
            points = new Vector3[1];
            points[0] = trf.apply(new Vector3(0, 0, 0));
		} 
        else { //create pt_cnt points
            points = new Vector3[pt_cnt];
			//stem.DBG("MESH+LOBES: lobes: %d, depth: %f\n"%(self.tree.Lobes, self.tree.LobeDepth))
			
			for (int i=0; i<pt_cnt; i++) {
                float angle = i * 360.0f / pt_cnt;
				// for Lobes ensure that points are near lobes extrema, but not exactly there
				// otherwise there are to sharp corners at the extrema
				if (lpar.level==0 && par.Lobes != 0) {
					angle -= 10.0f/par.Lobes;
				}
				
				// create some point on the unit circle
                Vector3 pt = new Vector3((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180), 0);

				// scale it to stem radius
				if (lpar.level==0 && (par.Lobes != 0 || par._0ScaleV !=0)) {
					float rad1 = rad * (1 + 
							par.random.uniform(-par._0ScaleV,par._0ScaleV)/
							getSubsegmentCount());
					pt = pt * ((float)(rad1*(1.0+par.LobeDepth*Math.Cos(par.Lobes*angle*Math.PI/180.0)))); 
				} else {
					pt = pt * rad; // faster - no radius calculations
				}
				// apply transformation to it
				// (for the first trunk segment transformation shouldn't be applied to
				// the lower meshpoints, otherwise there would be a gap between 
				// ground and trunk)
				// FIXME: for helical stems may be/may be not a random rotation 
				// should applied additionally?
				
				pt = trf.apply(pt);
                if (!lower_section) pt += (getUpperPosition()-getLowerPosition());
				points[i] = pt;
			}
		}
		
		return points;
	}
    }
}
