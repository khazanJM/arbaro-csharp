using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
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
        public CS_Vector pos;
        public double dist; // distance from segment's base 
        public double rad;
        CS_SegmentImpl segment;
        public CS_SubsegmentImpl prev = null;
        public CS_SubsegmentImpl next = null;

        public override CS_Vector getPosition()
        {
            return pos;
        }

        public override double getRadius()
        {
            return rad;
        }

        public override CS_Transformation getTransformation()
        {
            return segment.transf.translate(pos.sub(segment.getLowerPosition()));
        }

        public override CS_Vector getZ()
        {
            return segment.transf.getZ();
        }

        public override double getDistance()
        {
            return segment.index * segment.length + dist;
        }

        public CS_SubsegmentImpl(CS_Vector p, double r, double h, CS_SegmentImpl segment)
        {
            pos = p;
            rad = r;
            dist = h;
            this.segment = segment;
        }

        public override CS_Vector[] getSectionPoints()
        {
            CS_Params par = segment.par;
            CS_LevelParams lpar = segment.lpar;

            int pt_cnt = lpar.mesh_points;
            CS_Vector[] points;
            CS_Transformation trf = getTransformation(); //segment.getTransformation().translate(pos.sub(segment.getLowerPosition()));

            // if radius = 0 create only one point
            if (rad < 0.000001)
            {
                points = new CS_Vector[1];
                points[0] = trf.apply(new CS_Vector(0, 0, 0));
            }
            else
            { //create pt_cnt points
                points = new CS_Vector[pt_cnt];
                //stem.DBG("MESH+LOBES: lobes: %d, depth: %f\n"%(self.tree.Lobes, self.tree.LobeDepth))

                for (int i = 0; i < pt_cnt; i++)
                {
                    double angle = i * 360.0 / pt_cnt;
                    // for Lobes ensure that points are near lobes extrema, but not exactly there
                    // otherwise there are to sharp corners at the extrema
                    if (lpar.level == 0 && par.Lobes != 0)
                    {
                        angle -= 10.0 / par.Lobes;
                    }

                    // create some point on the unit circle
                    CS_Vector pt = new CS_Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180), 0);
                    // scale it to stem radius
                    if (lpar.level == 0 && (par.Lobes != 0 || par._0ScaleV != 0))
                    {
                        double rad1 = rad * (1 +
                                par.random.uniform(-par._0ScaleV, par._0ScaleV) /
                                segment.getSubsegmentCount());
                        pt = pt.mul(rad1 * (1.0 + par.LobeDepth * Math.Cos(par.Lobes * angle * Math.PI / 180.0)));
                    }
                    else
                    {
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
