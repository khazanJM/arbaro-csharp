using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine
{
    public class DXConfigClass
    {
        public int FormDefaultWidth = 1024;
        public int FormDefaultHeight = 768;

        public bool VSync = false;

        public float ScreenDepth = 1000.0f;
        public float ScreenNear = 0.1f;
        public float FOV = (float)(45 * Math.PI / 180);

        public int MSAA_SampleCount = 4;
        public int MSAA_SampleDesc = 0;

        public Color4 BackgroundColor = new Color4(new Vector4(110.0f / 255.0f, 126 / 255.0f, 142.0f / 255.0f, 1f));

        // Create config from config file
        public DXConfigClass(string configFilePath)
        {              
        }
    }
}
