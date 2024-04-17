using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy.Tools
{
    public class Animation
    {
        private bool _loop;
        private Stopwatch _stopwatch = new Stopwatch();
        private List<int> _keyframes = new List<int>();

        public Animation(List<int> keyframes, bool loop = false)
        {
            keyframes.Sort();
            _keyframes = keyframes;
            _loop = loop;
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

        public bool IsRunning()
        {
            return _stopwatch.IsRunning;
        }

        public int GetKeyframe()
        {
            int keyframe = _keyframes.Where(k => k <= _stopwatch.ElapsedMilliseconds).Count();
            if(keyframe == _keyframes.Count)
            {
                if(_loop)
                {
                    _stopwatch.Restart();
                }
                else
                {
                    _stopwatch.Stop();
                }
            }
            return keyframe;
        }
    }
}
