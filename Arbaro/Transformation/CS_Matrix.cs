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
        private Matrix _matrix;

        public CS_Matrix()
        {
            _matrix = Matrix.Identity;               
        }

        public static CS_Matrix RotY(float radAngle)
        {          
            CS_Matrix rmm = new CS_Matrix();
            rmm._matrix = Matrix.RotationY(radAngle);
            
            return rmm;
        }

        public static CS_Matrix RotX(float radAngle)
        {     
            CS_Matrix rmm = new CS_Matrix();
            rmm._matrix = Matrix.RotationX(-radAngle);
            
            return rmm;
        }

        public static CS_Matrix RotXZ(float radDelta, float radRho)
        {           
            Matrix rotZ = Matrix.RotationZ(-radRho);
            Matrix rotX = Matrix.RotationX(-radDelta);
        
            CS_Matrix rmm = new CS_Matrix();
            rmm._matrix = rotZ * rotX;

            return rmm;
        }

        public static CS_Matrix RotAxisZ(float radDelta, float radRho)
        {
            float a = (float)Math.Cos(radRho);
            float b = (float)Math.Sin(radRho);
                       
            CS_Matrix rmm = new CS_Matrix();
            rmm._matrix = Matrix.RotationAxis(new Vector3(a,b,0),-radDelta);

            return rmm;
        }

        public static CS_Matrix RotAxis(float radAngle, Vector3 axis)
        {           
            CS_Matrix rmm = new CS_Matrix();
            rmm._matrix = Matrix.RotationAxis(axis, -radAngle);

            return rmm;
        }

        public static CS_Matrix Prod(CS_Matrix a, CS_Matrix b)
        {
            CS_Matrix r = new CS_Matrix();
            r._matrix = a._matrix * b._matrix;
            return r;          
        }

        public static Vector3 Prod(CS_Matrix a, Vector3 b)
        {
            Matrix mt = a._matrix; mt.Transpose();
            Vector4 v4 = Vector3.Transform(b, mt);
            return new Vector3(v4.X, v4.Y, v4.Z);
        }        
    }
}
