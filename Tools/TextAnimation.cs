using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy
{
    public class TextAnimation : Animation
    {
        private List<string> _keyframesValue = new List<string>();

        public TextAnimation(Dictionary<int, string> keyframes) : base(keyframes.Select(kv => kv.Key).ToList())
        {
            _keyframesValue = keyframes.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }

        public new string GetKeyframe()
        {
            return _keyframesValue[base.GetKeyframe()-1];
        }
    }
}
