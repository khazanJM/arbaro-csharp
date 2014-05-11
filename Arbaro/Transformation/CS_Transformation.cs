using SharpDX;
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
    public class DX_Transformation
    {
        public static int X = 0;
        public static int Y = 1;
        public static int Z = 2;
        public static int T = 3;

        public static Vector3 X_AXIS = new Vector3(1, 0, 0);
        public static Vector3 Y_AXIS = new Vector3(0, 1, 0);
        public static Vector3 Z_AXIS = new Vector3(0, 0, 1);

        CS_Matrix _matrix;
        Matrix _matrix3;
        Vector3 _vector;

        public DX_Transformation()
        {
            _matrix = new CS_Matrix();
            _matrix3 = Matrix.Identity;
            _vector = new Vector3();
        }
       
        public DX_Transformation(CS_Matrix m, Matrix M, Vector3 v)
        {
            _matrix = m;
            _matrix3 = M;
            _vector = v;
        }
        
        public CS_Matrix matrix()
        {
            return _matrix;
        }

        public Matrix matrix3()
        {
            return _matrix3;
        }
      
        public Vector3 vector()
        {
            return _vector;
        }


        /**
         * @param T1 the transformation to multiply with
         * @return the product of two transformations, .i.e. the tranformation
         * resulting of the two transformations applied one after the other
         */
        public DX_Transformation prod(DX_Transformation T1)
        {           
            Matrix M = T1.matrix3() * _matrix3;
            return new DX_Transformation(_matrix.prod(T1.matrix()), M, _matrix.prod(T1.vector()) + _vector);
        }

        /**
         * Applies the transformation to a vector
         * 
         * @param v
         * @return resulting vector
         */
        public Vector3 apply(Vector3 v)
        {
            return _matrix.prod(v) + _vector;
        }

        /**
         * Returns the X-column of the rotation matrix. This
         * is the projection on to the x-axis
         * 
         * @return X-column of the rotation matrix
         */       
        public Vector3 getX3()
        {
            return _matrix.col(X);            
        }

        /**
         * Returns the Y-column of the rotation matrix. This
         * is the projection on to the y-axis
         * 
         * @return Y-column of the rotation matrix
         */       
        public Vector3 getY3()
        {
            return _matrix.col(Y);         
        }

        /**
         * Returns the Z-column of the rotation matrix. This
         * is the projection on to the z-axis
         * 
         * @return Z-column of the rotation matrix
         */
        public Vector3 getZ3()
        {
            return _matrix.col(Z);
        }

        /**
         * Returns the translation vector of the transformation.
         * (for convenience, same as vector()) 
         * 
         * @return translation vector of the transformation
         */      
        public Vector3 getT() { return _vector; }

        public String toString()
        {
            return "x: " + getX3() + ", y: " + getY3() + ", z: " + getZ3() + ", t: " + getT();
        }

        public DX_Transformation rotz(float angle)
        {
            // local rotation about z-axis
            float radAngle = (float)(angle * Math.PI / 180);
            CS_Matrix rm = new CS_Matrix((float)Math.Cos(radAngle), -(float)Math.Sin(radAngle), 0,
                    (float)Math.Sin(radAngle), (float)Math.Cos(radAngle), 0,
                    0, 0, 1);

            Matrix M = Matrix.RotationZ(radAngle);

            DX_Transformation transf = new DX_Transformation(_matrix.prod(rm), M*_matrix3 ,_vector);

            return transf;
        }

        public DX_Transformation roty(double angle)
        {
            // local rotation about z-axis
            float radAngle = (float)(angle * Math.PI / 180);
            CS_Matrix rm = new CS_Matrix((float)Math.Cos(radAngle), 0, -(float)Math.Sin(radAngle),
                    0, 1, 0,
                    (float)Math.Sin(radAngle), 0, (float)Math.Cos(radAngle));

            Matrix M = Matrix.RotationY(radAngle);
            return new DX_Transformation(_matrix.prod(rm), M*_matrix3, _vector);
        }

        public DX_Transformation rotx(double angle)
        {
            // local rotation about the x axis
            float radAngle = (float)(angle * Math.PI / 180);
            CS_Matrix rm = new CS_Matrix(1, 0, 0,
                    0, (float)Math.Cos(radAngle), -(float)Math.Sin(radAngle),
                    0, (float)Math.Sin(radAngle), (float)Math.Cos(radAngle));

            Matrix M = Matrix.RotationX(radAngle);

            return new DX_Transformation(_matrix.prod(rm), M*_matrix3, _vector);
        }

        public DX_Transformation rotxz(double delta, double rho)
        {
            // local rotation about the x and z axees - for the substems
            float radDelta = (float)(delta * Math.PI / 180);
            float radRho = (float)(rho * Math.PI / 180);
            float sir = (float)Math.Sin(radRho);
            float cor = (float)Math.Cos(radRho);
            float sid = (float)Math.Sin(radDelta);
            float cod = (float)Math.Cos(radDelta);

            CS_Matrix rm = new CS_Matrix(cor, -sir * cod, sir * sid,
                    sir, cor * cod, -cor * sid,
                    0, sid, cod);

            //TODO
            Matrix M = Matrix.Identity;
            return new DX_Transformation(_matrix.prod(rm), M*_matrix3,  _vector);
        }

        public DX_Transformation rotaxisz(double delta, double rho)
        {
            // local rotation away from the local z-axis 
            // about an angle delta using an axis given by rho 
            // - used for splitting and random rotations
            float radDelta = (float)(delta * Math.PI / 180);
            float radRho = (float)(rho * Math.PI / 180);

            float a = (float)Math.Cos(radRho);
            float b = (float)Math.Sin(radRho);
            float si = (float)Math.Sin(radDelta);
            float co = (float)Math.Cos(radDelta);

            CS_Matrix rm = new CS_Matrix((co + a * a * (1 - co)), (b * a * (1 - co)), (b * si),
                    (a * b * (1 - co)), (co + b * b * (1 - co)), (-a * si),
                    (-b * si), (a * si), (co));

            Matrix M = Matrix.Identity;
            // TODO
            return new DX_Transformation(_matrix.prod(rm), M*_matrix3, _vector);
        }

        public DX_Transformation translate(Vector3 v)
        {            
            return new DX_Transformation(_matrix, _matrix3, _vector+v);
        }
      
        public DX_Transformation rotaxis(double angle, Vector3 axis)
        {
            // rotation about an axis
            float radAngle = (float)(angle * Math.PI / 180);           
            axis.Normalize();
            float a = axis.X;
            float b = axis.Y;
            float c = axis.Z;
            float si = (float)Math.Sin(radAngle);
            float co = (float)Math.Cos(radAngle);

            CS_Matrix rm = new CS_Matrix(
                    (co + a * a * (1 - co)), (-c * si + b * a * (1 - co)), (b * si + c * a * (1 - co)),
                    (c * si + a * b * (1 - co)), (co + b * b * (1 - co)), (-a * si + c * b * (1 - co)),
                    (-b * si + a * c * (1 - co)), (a * si + b * c * (1 - co)), (co + c * c * (1 - co)));

            Matrix M = Matrix.RotationAxis(axis, radAngle);

            return new DX_Transformation(rm.prod(_matrix), _matrix3 * M, _vector);
        }

        public DX_Transformation inverse()
        {
            // get inverse transformation M+t -> M'-M'*t"
            CS_Matrix T1 = _matrix.transpose();
            Matrix M = Matrix.Transpose(_matrix3);
            return new DX_Transformation(T1, M, T1.prod(-_vector));
        }
    }
}
