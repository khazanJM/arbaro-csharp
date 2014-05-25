using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine.TreeClasses
{
    public class DXLeafMeshHelper
    {
        private float _length;
        private float _width;
        private float _stemLength;
        private int _leafType;

        public List<DXMEV> Vertices;
        public List<int> Indices;

        public DXLeafMeshHelper(float length, float width, float stemLength, string leafType, List<DXMEV> V, List<int> I)
        {
            _length = length;
            _width = width;
            _stemLength = stemLength;
            _leafType = 0;

            Vertices = V;
            Indices = I;

            if (leafType == "0") _leafType = 0;
            if (leafType == "1") _leafType = 1;
            if (leafType == "2") _leafType = 2;
            if (leafType == "3") _leafType = 3;
            if (leafType == "4") _leafType = 4;
            if (leafType == "5") _leafType = 5;
            if (leafType == "6") _leafType = 6;
            if (leafType == "7") _leafType = 7;
            if (leafType == "8") _leafType = 8;
            if (leafType == "9") _leafType = 9;



            MakeLeaf();
        }

        private void MakeLeaf()
        {
            if (_leafType == 0) MakeDiscPoints(5);
            else if (_leafType == 1) MakeDiscPoints(3);
            else if (_leafType == 2) MakeDiscPoints(4);
            else if (_leafType == 3) MakeDiscPoints(5);
            else if (_leafType == 4) MakeDiscPoints(6);
            else if (_leafType == 5) MakeDiscPoints(7);
        }


        private void MakeDiscPoints(int vCount)
        {
            for (int i = 0; i < vCount; i++)
            {
                float a = (float)(i * Math.PI * 2 / vCount);
                Vector4 p = new Vector4((float)Math.Sin(a), 0, (float)Math.Cos(a), 1);

                if (a < Math.PI)
                {
                    p.X -= leaffunc(a);
                }
                else if (a > Math.PI)
                {
                    p.X += leaffunc((float)(2 * Math.PI - a));
                }

                p.X *= _width;
                p.Y *= _width;
                p.Z = (_stemLength + p.Z+1) * _length;              

                DXMEV m = new DXMEV();
                m.P = p;
                Vertices.Add(m);
            }

            for (int i = 0; i < vCount - 2; i++)
            {
                Indices.Add(0); Indices.Add(i + 1); Indices.Add(i + 2);
            }
        }

        private float leaffunc(float angle)
        {
            return (float)(leaffuncaux(angle) - angle * leaffuncaux((float)Math.PI) / Math.PI);
        }

        private float leaffuncaux(float x)
        {
            return (float)(0.8 * Math.Log(x + 1) / Math.Log(1.2) - 1.0 * Math.Sin(x));
        }

        private void MakeLeaf1()
        {
        }
    }
}
