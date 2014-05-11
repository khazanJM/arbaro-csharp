using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Ported from original Arbaro software
//  This should be replaced with the SharpDX implementation
//

namespace Arbaro2.Arbaro.Transformation
{
    public class CS_Vector
    {
        static int X = 0;
        static int Y = 1;
        static int Z = 2;

        public static CS_Vector X_AXIS = new CS_Vector(1, 0, 0);
        public static CS_Vector Y_AXIS = new CS_Vector(0, 1, 0);
        public static CS_Vector Z_AXIS = new CS_Vector(0, 0, 1);

        private float[] coord = { 0, 0, 0 };

        public CS_Vector()
        {
            coord = new float[Z + 1];           
        }

        public CS_Vector(float x, float y, float z)
        {
            coord = new float[Z + 1];
            coord[X] = x;
            coord[Y] = y;
            coord[Z] = z;            
        }

        public bool equals(CS_Vector v)
        {
            return this.sub(v).abs() < 0.0000001;        
        }

        public float abs()
        {
            //returns the length of the vector
            return (float)Math.Sqrt(coord[X] * coord[X] + coord[Y] * coord[Y] + coord[Z] * coord[Z]);
        }

        // I'm too lazy to port the string format of Arbaro
        public String toString()
        {
            return "<" + coord[X] + "," + coord[Y] + "," + coord[Z] + ">";
        }

        public CS_Vector normalize()
        {
            float abs = this.abs();
            return new CS_Vector(coord[X] / abs, coord[Y] / abs, coord[Z] / abs);
        }

        public float getX()
        {
            return coord[X];
        }

        public float getY()
        {
            return coord[Y];
        }

        public float getZ()
        {
            return coord[Z];
        }

        public CS_Vector mul(float factor)
        {
            // scales the vector
            return new CS_Vector(coord[X] * factor, coord[Y] * factor, coord[Z] * factor);
        }

        public float prod(CS_Vector v)
        {
            // inner product of two vectors
            return coord[X] * v.getX() + coord[Y] * v.getY() + coord[Z] * v.getZ();
        }

        public CS_Vector div(float factor)
        {
            return this.mul(1 / factor);
        }

        public CS_Vector add(CS_Vector v)
        {
            return new CS_Vector(coord[X] + v.getX(), coord[Y] + v.getY(), coord[Z] + v.getZ());
        }

        public CS_Vector sub(CS_Vector v)
        {
            return this.add(v.mul(-1));
        }

        /**
         * Returns the angle of a 2-dimensional vector (u,v) with the u-axis 
         *
         * @param v v-coordinate of the vector
         * @param u u-coordinate of the vector
         * @return a value from (-180..180)
         */
        static public float atan2(float v, float u)
        {
            return (float)(Math.Atan2(v, u) * 180 / Math.PI);
        }

        public void setMaxCoord(CS_Vector v)
        {
            if (v.getX() > coord[X]) coord[X] = v.getX();
            if (v.getY() > coord[Y]) coord[Y] = v.getY();
            if (v.getZ() > coord[Z]) coord[Z] = v.getZ();
        }

        public void setMinCoord(CS_Vector v)
        {
            if (v.getX() < coord[X]) coord[X] = v.getX();
            if (v.getY() < coord[Y]) coord[Y] = v.getY();
            if (v.getZ() < coord[Z]) coord[Z] = v.getZ();
        }
    }
}
