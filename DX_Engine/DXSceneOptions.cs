using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine
{
    public class DXSceneOptions
    {
        public enum LEVELS { LEVEL_0 = 0, LEVEL_1 = 1, LEVEL_2 = 2, LEVEL_3 = 3, LEAVES = 4};
        public bool[] LevelVisibility = { true, true, true, true, true };
    }
}
