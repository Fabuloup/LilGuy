using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy.Tools
{
    public class LilGuyKeyFrame
    {
        public string text { get; set; } = "";
        public int blurRadius { get; set; } = 0;

        public LilGuyKeyFrame() { }

        public LilGuyKeyFrame(LilGuyKeyFrame original)
        {
            this.text = original.text;
            this.blurRadius = original.blurRadius;
        }
    }

    public class LilGuyAnimation : Animation
    {
        private List<LilGuyKeyFrame> _keyframesValue = new List<LilGuyKeyFrame>();

        public LilGuyAnimation(Dictionary<int, LilGuyKeyFrame> keyframes, bool loop = false) : base(keyframes.Select(kv => kv.Key).ToList(), loop)
        {
            _keyframesValue = keyframes.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }

        public new LilGuyKeyFrame GetKeyframe()
        {
            double preciseKeyframe = base.GetPreciseKeyframe();
            int keyframe = (int)Math.Truncate(preciseKeyframe);
            double precision = preciseKeyframe - keyframe;
            LilGuyKeyFrame keyframeValue = new LilGuyKeyFrame(_keyframesValue[keyframe - 1]);
            LilGuyKeyFrame previousKeyframeValue = _keyframesValue[_keyframesValue.Count() - 1];
            if ((int)Math.Truncate(preciseKeyframe) > 1)
            {
                previousKeyframeValue = _keyframesValue[keyframe-2];
            }

            keyframeValue.blurRadius = (int)Double.Round(Double.Lerp(previousKeyframeValue.blurRadius, keyframeValue.blurRadius, precision));

            return keyframeValue;
        }

        public new LilGuyKeyFrame GetPreciseKeyframe()
        {
            return GetKeyframe();
        }
    }
}
