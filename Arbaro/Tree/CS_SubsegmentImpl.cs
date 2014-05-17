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
    public class CS_SubsegmentImpl : CS_StemSection
    {
        // a Segment can have one or more Subsegments
        public Vector3 pos;
        public float dist; // distance from segment's base 
        public float rad;
        CS_SegmentImpl segment;
        public CS_SubsegmentImpl prev = null;
        public CS_SubsegmentImpl next = null;

        public override Vector3 getPosition()
        {
            return pos;
        }

        public override double getRadius()
        {
            return rad;
        }    

        public override DX_Transformation getTransformation()
        {
            return segment.transf.translate(pos - segment.getLowerPosition());
        }

        public override Vector3 getZ()
        {
            return segment.transf.getZ3();
        }

        public override double getDistance()
        {
            return segment.index * segment.length + dist;
        }

        public CS_SubsegmentImpl(Vector3 p, float r, float h, CS_SegmentImpl segment)
        {
            pos = p;
            rad = r;
            dist = h;
            this.segment = segment;
        }

        public override Vector3[] getSectionPoints()
        {
            CS_Params par = segment.par;
            CS_LevelParams lpar = segment.lpar;

            int pt_cnt = lpar.mesh_points;
            Vector3[] points;
            DX_Transformation trf = getTransformation(); //segment.getTransformation().translate(pos.sub(segment.getLowerPosition()));

            // if radius = 0 create only one point
            if (rad < 0.000001)
            {
                points = new Vector3[1];
                points[0] = trf.apply(new Vector3(0, 0, 0));
            }
            else
            { //create pt_cnt points
                points = new Vector3[pt_cnt];
                //stem.DBG("MESH+LOBES: lobes: %d, depth: %f\n"%(self.tree.Lobes, self.tree.LobeDepth))

                for (int i = 0; i < pt_cnt; i++)
                {
                    float angle = i * 360.0f / pt_cnt;
                    // for Lobes ensure that points are near lobes extrema, but not exactly there
                    // otherwise there are to sharp corners at the extrema
                    if (lpar.level == 0 && par.Lobes != 0)
                    {
                        angle -= 10.0f / par.Lobes;
                    }

                    // create some point on the unit circle
                    Vector3 pt = new Vector3((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180), 0);
                    // scale it to stem radius
                    if (lpar.level == 0 && (par.Lobes != 0 || par._0ScaleV != 0))
                    {
                        float rad1 = rad * (1 +
                                par.random.uniform(-par._0ScaleV, par._0ScaleV) /
                                segment.getSubsegmentCount());
                        pt = pt * (rad1 * (1.0f + par.LobeDepth * (float)Math.Cos(par.Lobes * angle * Math.PI / 180.0)));
                    }
                    else
                    {
                        pt = pt * rad; // faster - no radius calculations
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
