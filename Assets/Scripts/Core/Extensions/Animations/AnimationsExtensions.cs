using UnityEngine;

namespace Core.Extensions.Animations
{
    public static class AnimationsExtensions
    {
        public static bool IsAnyPlaying(this Animation animation)
        {
            foreach (AnimationState state in animation)
            {
                if(!animation.IsPlaying(state.name)) continue;
                return true;
            }

            return false;
        }
    }
}