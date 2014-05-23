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

        // rotation part of the transform
        Matrix _matrix;    

        // translation part of the transform
        Vector3 _vector;

        public DX_Transformation()
        {
            _matrix = Matrix.Identity;     
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
            Matrix p = _matrix * T1._matrix;
            
            Matrix mt = _matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(T1._vector, mt);
            Vector3 v3 = new Vector3(v4.X, v4.Y, v4.Z);

            return new DX_Transformation(p, v3 + _vector);
        }

        /**
         * Applies the transformation to a vector
         * 
         * @param v
         * @return resulting vector
         */
        public Vector3 apply(Vector3 v)
        {
            Matrix mt = _matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(v, mt);
            Vector3 v3 = new Vector3(v4.X, v4.Y, v4.Z);
            
            return v3 + _vector;
        }

        public Vector4 apply(Vector4 v)
        {
            Matrix mt = _matrix; mt.Transpose();       
            Vector4 v4 = Vector4.Transform(v, mt);
            
            return new Vector4(v4.X+_vector.X, v4.Y+_vector.Y, v4.Z+_vector.Z, 1);
        }

        /**
         * Returns the Z-column of the rotation matrix. This
         * is the projection on to the z-axis
         * 
         * @return Z-column of the rotation matrix
         */
        public Vector3 getZ3()
        {
            Matrix mt = _matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(new Vector3(0,0,1), mt);
            Vector3 v3 = new Vector3(v4.X, v4.Y, v4.Z);

            return v3;    
        }

        /**
         * Returns the translation vector of the transformation.
         * (for convenience, same as vector()) 
         * 
         * @return translation vector of the transformation
         */      
        public Vector3 getT() { return _vector; }

        

        public DX_Transformation roty(double angle)
        {
            // local rotation about z-axis
            float radAngle = (float)(angle * Math.PI / 180);            
            Matrix rm = Matrix.RotationY(radAngle);
            
            return new DX_Transformation(_matrix* rm, _vector);
        }

        public DX_Transformation rotx(double angle)
        {
            // local rotation about the x axis
            float radAngle = (float)(angle * Math.PI / 180);
            Matrix rm = Matrix.RotationX(-radAngle);           

            return new DX_Transformation(_matrix* rm, _vector);
        }

        private Matrix RotXZ(float radDelta, float radRho) 
        {
            Matrix rotZ = Matrix.RotationZ(-radRho);
            Matrix rotX = Matrix.RotationX(-radDelta);
         
            return rotZ * rotX;      
        }

        public DX_Transformation rotxz(double delta, double rho)
        {
            // local rotation about the x and z axees - for the substems
            float radDelta = (float)(delta * Math.PI / 180);
            float radRho = (float)(rho * Math.PI / 180);          
            Matrix rm = RotXZ(radDelta, radRho);

            return new DX_Transformation(_matrix * rm, _vector);
        }

        private Matrix RotAxisZ(float radDelta, float radRho)
        {
            float a = (float)Math.Cos(radRho);
            float b = (float)Math.Sin(radRho);

            return Matrix.RotationAxis(new Vector3(a, b, 0), -radDelta);
        }

        public DX_Transformation rotaxisz(double delta, double rho)
        {
            // local rotation away from the local z-axis 
            // about an angle delta using an axis given by rho 
            // - used for splitting and random rotations
            float radDelta = (float)(delta * Math.PI / 180);
            float radRho = (float)(rho * Math.PI / 180);
            Matrix rm = RotAxisZ(radDelta, radRho);

            return new DX_Transformation(_matrix* rm, _vector);
        }

        public DX_Transformation translate(Vector3 v)
        {            
            return new DX_Transformation(_matrix, _vector+v);
        }
      
        public DX_Transformation rotaxis(double angle, Vector3 axis)
        {
            // rotation about an axis
            float radAngle = (float)(angle * Math.PI / 180);
            Matrix rm = Matrix.RotationAxis(axis, -radAngle);
          
            return new DX_Transformation(rm* _matrix, _vector);
        }



        //
        //          Used by CS_LeafImpl
        //


        /**
        * Returns the X-column of the rotation matrix. This
        * is the projection on to the x-axis
        * 
        * @return X-column of the rotation matrix
        */
        
        public Vector3 getX3()
        {
            Matrix mt = _matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(new Vector3(1, 0, 0), mt);
            Vector3 v3 = new Vector3(v4.X, v4.Y, v4.Z);

            return v3;   
        }

        /**
         * Returns the Y-column of the rotation matrix. This
         * is the projection on to the y-axis
         * 
         * @return Y-column of the rotation matrix
         */
        
        public Vector3 getY3()
        {
            Matrix mt = _matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(new Vector3(0, 1, 0), mt);
            Vector3 v3 = new Vector3(v4.X, v4.Y, v4.Z);

            return v3;           
        }

    }
}
