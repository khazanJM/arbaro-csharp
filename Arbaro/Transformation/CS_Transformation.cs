using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.Transformation
{
    public class CS_Transformation
    {
        public static int X = 0;
        public static int Y = 1;
        public static int Z = 2;
        public static int T = 3;

        CS_Matrix _matrix;
        CS_Vector _vector;

        public CS_Transformation()
        {
            _matrix = new CS_Matrix();
            _vector = new CS_Vector();
        }

        public CS_Transformation(CS_Matrix m, CS_Vector v)
        {
            _matrix = m;
            _vector = v;
        }

        public CS_Matrix matrix()
        {
            return _matrix;
        }

        public CS_Vector vector()
        {
            return _vector;
        }

        /**
         * @param T1 the transformation to multiply with
         * @return the product of two transformations, .i.e. the tranformation
         * resulting of the two transformations applied one after the other
         */
        public CS_Transformation prod(CS_Transformation T1)
        {
            return new CS_Transformation(_matrix.prod(T1.matrix()),_matrix.prod(T1.vector()).add(_vector));
        }

        /**
         * Applies the transformation to a vector
         * 
         * @param v
         * @return resulting vector
         */
        public CS_Vector apply(CS_Vector v)
        {
            return _matrix.prod(v).add(_vector);
        }

        /**
         * Returns the X-column of the rotation matrix. This
         * is the projection on to the x-axis
         * 
         * @return X-column of the rotation matrix
         */
        public CS_Vector getX()
        {
            return _matrix.col(X);
        }

        /**
         * Returns the Y-column of the rotation matrix. This
         * is the projection on to the y-axis
         * 
         * @return Y-column of the rotation matrix
         */
        public CS_Vector getY()
        {
            return _matrix.col(Y);
        }

        /**
         * Returns the Z-column of the rotation matrix. This
         * is the projection on to the z-axis
         * 
         * @return Z-column of the rotation matrix
         */
        public CS_Vector getZ()
        {
            return _matrix.col(Z);
        }

        /**
         * Returns the translation vector of the transformation.
         * (for convenience, same as vector()) 
         * 
         * @return translation vector of the transformation
         */
        public CS_Vector getT()
        {
            return _vector;
        }

        /*	
         ostream& operator << (ostream &os, const Transformation &trf) {
         os << "(X:" << trf[X] << "; Y:" << trf[Y] << "; Z:" << trf[Z] 
         << "; T:" << trf[T] << ")";
         return os;
         }
         */






        public String toString()
        {
            return "x: " + getX() + ", y: " + getY() + ", z: " + getZ() + ", t: " + getT();
        }


        //    public String povray() {
        //	NumberFormat fmt = FloatFormat.getInstance();
        //	return "matrix <" + fmt.format(matrix.get(X,X)) + "," 
        //	    + fmt.format(matrix.get(X,Z)) + "," 
        //	    + fmt.format(matrix.get(X,Y)) + ","
        //	    + fmt.format(matrix.get(Z,X)) + "," 
        //	    + fmt.format(matrix.get(Z,Z)) + "," 
        //	    + fmt.format(matrix.get(Z,Y)) + ","
        //	    + fmt.format(matrix.get(Y,X)) + "," 
        //	    + fmt.format(matrix.get(Y,Z)) + "," 
        //	    + fmt.format(matrix.get(Y,Y)) + ","
        //	    + fmt.format(vector.getX())   + "," 
        //	    + fmt.format(vector.getZ())   + "," 
        //	    + fmt.format(vector.getY()) + ">";
        //    }

        public CS_Transformation rotz(double angle)
        {
            // local rotation about z-axis
            double radAngle = angle * Math.PI / 180;
            CS_Matrix rm = new CS_Matrix(Math.Cos(radAngle), -Math.Sin(radAngle), 0,
                    Math.Sin(radAngle), Math.Cos(radAngle), 0,
                    0, 0, 1);
            return new CS_Transformation(_matrix.prod(rm), _vector);
        }

        public CS_Transformation roty(double angle)
        {
            // local rotation about z-axis
            double radAngle = angle * Math.PI / 180;
            CS_Matrix rm = new CS_Matrix(Math.Cos(radAngle), 0, -Math.Sin(radAngle),
                    0, 1, 0,
                    Math.Sin(radAngle), 0, Math.Cos(radAngle));
            return new CS_Transformation(_matrix.prod(rm), _vector);
        }

        public CS_Transformation rotx(double angle)
        {
            // local rotation about the x axis
            double radAngle = angle * Math.PI / 180;
            CS_Matrix rm = new CS_Matrix(1, 0, 0,
                    0, Math.Cos(radAngle), -Math.Sin(radAngle),
                    0, Math.Sin(radAngle), Math.Cos(radAngle));
            return new CS_Transformation(_matrix.prod(rm), _vector);
        }

        public CS_Transformation rotxz(double delta, double rho)
        {
            // local rotation about the x and z axees - for the substems
            double radDelta = delta * Math.PI / 180;
            double radRho = rho * Math.PI / 180;
            double sir = Math.Sin(radRho);
            double cor = Math.Cos(radRho);
            double sid = Math.Sin(radDelta);
            double cod = Math.Cos(radDelta);

            CS_Matrix rm = new CS_Matrix(cor, -sir * cod, sir * sid,
                    sir, cor * cod, -cor * sid,
                    0, sid, cod);
            return new CS_Transformation(_matrix.prod(rm), _vector);
        }

        public CS_Transformation rotaxisz(double delta, double rho)
        {
            // local rotation away from the local z-axis 
            // about an angle delta using an axis given by rho 
            // - used for splitting and random rotations
            double radDelta = delta * Math.PI / 180;
            double radRho = rho * Math.PI / 180;

            double a = Math.Cos(radRho);
            double b = Math.Sin(radRho);
            double si = Math.Sin(radDelta);
            double co = Math.Cos(radDelta);

            CS_Matrix rm = new CS_Matrix((co + a * a * (1 - co)), (b * a * (1 - co)), (b * si),
                    (a * b * (1 - co)), (co + b * b * (1 - co)), (-a * si),
                    (-b * si), (a * si), (co));
            return new CS_Transformation(_matrix.prod(rm), _vector);
        }

        public CS_Transformation translate(CS_Vector v)
        {
            return new CS_Transformation(_matrix, _vector.add(v));
        }

        public CS_Transformation rotaxis(double angle, CS_Vector axis)
        {
            // rotation about an axis
            double radAngle = angle * Math.PI / 180;
            CS_Vector normAxis = axis.normalize();
            double a = normAxis.getX();
            double b = normAxis.getY();
            double c = normAxis.getZ();
            double si = Math.Sin(radAngle);
            double co = Math.Cos(radAngle);

            CS_Matrix rm = new CS_Matrix(
                    (co + a * a * (1 - co)), (-c * si + b * a * (1 - co)), (b * si + c * a * (1 - co)),
                    (c * si + a * b * (1 - co)), (co + b * b * (1 - co)), (-a * si + c * b * (1 - co)),
                    (-b * si + a * c * (1 - co)), (a * si + b * c * (1 - co)), (co + c * c * (1 - co)));
            return new CS_Transformation(rm.prod(_matrix), _vector);
        }

        public CS_Transformation inverse()
        {
            // get inverse transformation M+t -> M'-M'*t"
            CS_Matrix T1 = _matrix.transpose();
            return new CS_Transformation(T1, T1.prod(_vector.mul(-1)));
        }
    }
}
