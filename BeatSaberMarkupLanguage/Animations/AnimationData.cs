using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public record AnimationData(Texture2D atlas, Rect[] uvs, float[] delays, int width, int height);
}
