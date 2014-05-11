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
    public abstract class CS_StemSection
    {
        /**
 * 
 * @return the midpoint of the section
 */
        public abstract Vector3 getPosition();

        /**
         * 
         * @return the radius of the section (point distance from
         * midpoint can vary when lobes are used)
         */
        public abstract double getRadius();

        /**
         * @return relative distance from stem origin
         */
        public abstract double getDistance();

        /**
         * @return the transformation for the section, giving it's
         * position vector and rotation matrix
         */
        public abstract DX_Transformation getTransformation();

        /**
         * 
         * @return the z-direction vector, it is orthogonal to the
         * section layer
         */
        public abstract Vector3 getZ();

        /**
         * 
         * @return the vertex points of the section. It's number
         * is influenced by the stem level and smooth value. It's distance
         * from midpoint can vary about the radius (when lobes are used).
         */
        public abstract Vector3[] getSectionPoints();
    }
}
