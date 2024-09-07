using UnityEngine;

namespace BeatSaberMarkupLanguage.Animations
{
    public record AnimationData(Texture2D Atlas, Rect[] Uvs, float[] Delays, int Width, int Height);
}
