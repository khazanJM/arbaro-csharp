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

            MakeLeaf();
        }

        private void MakeLeaf()
        {
            if (_leafType == 0) MakeDiscPoints(6);
            else if (_leafType == 1) MakeLeaf1();
        }


        private void MakeDiscPoints(int vCount)
        {
            for (int i = 0; i < vCount; i++)
            {
                float a = (float)(3 * Math.PI / 2f + i * Math.PI * 2 / vCount);
                Vector4 p = 0.5f * new Vector4(_width * (float)Math.Cos(a), 0, _length * (_stemLength + 1 + (float)Math.Sin(a)), 2);
                DXMEV m = new DXMEV();
                m.P = p;
                Vertices.Add(m);
            }

            for (int i = 0; i < vCount - 2; i++)
            {
                Indices.Add(0); Indices.Add(i + 1); Indices.Add(i + 2);
            }
        }


        private void MakeLeaf1()
        {
        }
    }
}
