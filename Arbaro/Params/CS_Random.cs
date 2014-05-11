using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Ported from original Arbaro software
//

namespace Arbaro2.Arbaro.Params
{
    public class CS_Random
    {
        private Random _r;

        public CS_Random(int seed)
        {

            _r = new Random(seed);
        }

        public float uniform(float low, float high)
        {
            return low + (float)_r.NextDouble() * (high - low);
        }

        public int getState()
        {
            // the original random generator doesn't provide an interface
            // to read, and reset it's state, so this is a hack here, to make
            // this possible. The random generator is reseeded here with a seed
            // got from the generator, this seed are returned as state.
            int state = _r.Next();
            _r = new Random(state);
            return state;
        }

        public void setState(int state)
        {
            _r = new Random(state);
        }

        public int nextInt() { return _r.Next(); }
    }
}
