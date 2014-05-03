using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine
{
    public class DXConfig
    {
        // MSAA
        public int MSAA_SampleCount = 4;
        public int MSAA_SampleDesc = 0;

        // VSync
        public bool VSync = true;

        // Background color
        public Color BackgroundColor = new Color(110, 126, 142, 255);       

        // Create config from config file
        public DXConfig(string configFilePath)
        {              
        }
    }
}
