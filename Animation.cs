using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy
{
    public class Animation
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private List<int> _keyframes = new List<int>();

        public Animation(List<int> keyframes)
        {
            keyframes.Sort();
            _keyframes = keyframes;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public int GetKeyframe()
        {
            int keyframe = _keyframes.Where(k => k < _stopwatch.ElapsedMilliseconds).Count();
            if(keyframe == _keyframes.Count)
            {
                _stopwatch.Restart();
            }
            return keyframe;
        }
    }
}
