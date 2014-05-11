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

        Matrix _matrix;      
        Vector3 _vector;

        public DX_Transformation()
        {
            _matrix = new Matrix();         
            _vector = new Vector3();
        }

        public DX_Transformation(Matrix m, Vector3 v)
        {
            _matrix = m;
            _vector = v;
        }

        public Matrix matrix()
        {
            return _matrix;
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
            Vector4 t = Vector3.Transform(T1.vector(), _matrix);
            return new DX_Transformation(T1.matrix() * _matrix, new Vector3(t.X, t.Y, t.Z) + _vector);
        }

        /**
         * Applies the transformation to a vector
         * 
         * @param v
         * @return resulting vector
         */
        public Vector3 apply(Vector3 v)
        {
            Vector4 t = Vector3.Transform(v, _matrix);
            return new Vector3(t.X, t.Y, t.Z) + _vector;
        }

        /**
         * Returns the X-column of the rotation matrix. This
         * is the projection on to the x-axis
         * 
         * @return X-column of the rotation matrix
         */       
        public Vector3 getX3()
        {
            return new Vector3(_matrix.Column1.X, _matrix.Column1.Y, _matrix.Column1.Z);
            //return _matrix.col(X);            
        }

        /**
         * Returns the Y-column of the rotation matrix. This
         * is the projection on to the y-axis
         * 
         * @return Y-column of the rotation matrix
         */       
        public Vector3 getY3()
        {
            return new Vector3(_matrix.Column2.X, _matrix.Column2.Y, _matrix.Column2.Z);
            //return _matrix.col(Y);         
        }

        /**
         * Returns the Z-column of the rotation matrix. This
         * is the projection on to the z-axis
         * 
         * @return Z-column of the rotation matrix
         */
        public Vector3 getZ3()
        {
            return new Vector3(_matrix.Column3.X, _matrix.Column3.Y, _matrix.Column3.Z);
            //return _matrix.col(Z);
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
            Matrix rm = Matrix.RotationZ(radAngle);
            return new DX_Transformation(rm * _matrix, _vector);
        }

        public DX_Transformation roty(double angle)
        {
            // local rotation about z-axis
            float radAngle = (float)(angle * Math.PI / 180);
            Matrix rm = Matrix.RotationY(radAngle);
            return new DX_Transformation(rm*_matrix, _vector);
        }

        public DX_Transformation rotx(double angle)
        {
            // local rotation about the x axis
            float radAngle = (float)(angle * Math.PI / 180);
            Matrix rm = Matrix.RotationX(radAngle);
            return new DX_Transformation(rm * _matrix, _vector);
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

            
            Matrix rm = new Matrix(cor, -sir * cod, sir * sid, 0,
                    sir, cor * cod, -cor * sid, 0,
                    0, sid, cod, 0,
                    0, 0, 0, 1);
             
            // TODO
            return new DX_Transformation(rm * _matrix, _vector);
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
          
            Matrix rm = new Matrix((co + a * a * (1 - co)), (b * a * (1 - co)), (b * si), 0,
                    (a * b * (1 - co)), (co + b * b * (1 - co)), (-a * si), 0, 
                    (-b * si), (a * si), (co), 0,
                    0, 0, 0, 1);
                        
            return new DX_Transformation(rm * _matrix, _vector);
        }

        public DX_Transformation translate(Vector3 v)
        {            
            return new DX_Transformation(_matrix, _vector+v);
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

            Matrix rm = Matrix.RotationAxis(axis, radAngle);
            return new DX_Transformation(_matrix * rm, _vector);
        }

        public DX_Transformation inverse()
        {
            // get inverse transformation M+t -> M'-M'*t"
            Matrix T1 = Matrix.Transpose(_matrix);
            Vector4 t = Vector3.Transform(-_vector, T1);
            return new DX_Transformation(T1, new Vector3(t.X, t.Y, t.Z));
        }
    }
}
