using UnityEngine;

namespace Game.Controllers
{
    public class SineMotion
    {
        public Vector3 GetSineOffset(float amplitude, float frequency, float time)
        {
            float sineY = Mathf.Sin(time * frequency) * amplitude;
            return new Vector3(0, sineY, 0);
        }
    }
}
