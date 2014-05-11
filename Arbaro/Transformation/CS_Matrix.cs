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
    public class CS_Matrix
    {
        static int X = 0;
        static int Y = 1;
        static int Z = 2;

        private float[] data;

        public CS_Matrix()
        {
            data = new float[(Z + 1) * (Z + 1)];
            for (int r = X; r <= Z; r++)
            {
                for (int c = X; c <= Z; c++)
                {
                    data[r * 3 + c] = c == r ? 1 : 0;
                }
            }
        }

        public CS_Matrix(float xx, float xy, float xz,
                float yx, float yy, float yz,
                float zx, float zy, float zz)
        {
            data = new float[(Z + 1) * (Z + 1)];

            data[X * 3 + X] = xx;
            data[X * 3 + Y] = xy;
            data[X * 3 + Z] = xz;
            data[Y * 3 + X] = yx;
            data[Y * 3 + Y] = yy;
            data[Y * 3 + Z] = yz;
            data[Z * 3 + X] = zx;
            data[Z * 3 + Y] = zy;
            data[Z * 3 + Z] = zz;
        }

        public String toString()
        {
            return "x: " + row(X) + " y: " + row(Y) + " z: " + row(Z);
        }

        public Vector3 row(int r)
        {
            return new Vector3(data[r * 3 + X], data[r * 3 + Y], data[r * 3 + Z]);
        }

        public Vector3 col(int c)
        {
            return new Vector3(data[X * 3 + c], data[Y * 3 + c], data[Z * 3 + c]);
        }

        public float get(int r, int c)
        {
            return data[r * 3 + c];
        }

        public void set(int r, int c, float value)
        {
            data[r * 3 + c] = value;
        }

        public CS_Matrix transpose()
        {
            CS_Matrix T = new CS_Matrix();
            for (int r = X; r <= Z; r++)
            {
                for (int c = X; c <= Z; c++)
                {
                    T.set(r, c, data[c * 3 + r]);
                }
            }
            return T;
        }

        public CS_Matrix mul(float factor)
        {
            // scales the matrix with a factor
            CS_Matrix R = new CS_Matrix();

            for (int r = X; r <= Z; r++)
            {
                for (int c = X; c <= Z; c++)
                {
                    R.set(r, c, data[r * 3 + c] * factor);
                }
            }
            return R;
        }

        public CS_Matrix prod(CS_Matrix M)
        {
            //returns the matrix product
            CS_Matrix R = new CS_Matrix();

            for (int r = X; r <= Z; r++)
            {
                for (int c = X; c <= Z; c++)
                {
                    R.set(r, c, Vector3.Dot(row(r), M.col(c)));
                }
            }

            return R;
        }

        /**
         * Adds the matrix to another
         * 
         * @param M the matrix to be added
         * @return the sum of the two matrices
         */
        public CS_Matrix add(CS_Matrix M)
        {
            CS_Matrix R = new CS_Matrix();

            for (int r = X; r <= Z; r++)
            {
                for (int c = X; c <= Z; c++)
                {
                    R.set(r, c, data[r * 3 + c] + M.get(r, c));
                }
            }
            return R;
        }

        /**
         * Multiplies the matrix with a vector
         * 
         * @param v the vector
         * @return The product of the matrix and the vector
         */
  
        public Vector3 prod(Vector3 v)
        {
            return new Vector3(Vector3.Dot(row(X), v), Vector3.Dot(row(Y), v), Vector3.Dot(row(Z), v));
        }

        /**
         * Divids the matrix by a value
         * 
         * @param factor the divisor
         * @return The matrix divided by the value
         */
        public CS_Matrix div(float factor)
        {
            return mul(1 / factor);
        }

        /**
         * Substracts a matrix
         * 
         * @param M the matrix to be subtracted
         * @return The result of subtracting another matrix
         */
        public CS_Matrix sub(CS_Matrix M)
        {
            return add(M.mul(-1));
        }
    }
}
