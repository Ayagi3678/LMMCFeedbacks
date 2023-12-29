using UnityEngine;

namespace LMMCFeedbacks.Runtime
{
    public static class FeedbackStyling
    {
        public static Color VolumeFeedbackColor => Color.Lerp(Color.cyan, Color.green, .5f);
        public static Color GraphicFeedbackColor => Color.Lerp(Color.magenta, Color.red, .5f);
        public static Color UIFeedbackColor => Color.Lerp(Color.yellow, Color.green, .5f);
        public static Color AudioFeedbackColor => Color.yellow;
        public static Color ParticlesFeedbackColor => Color.Lerp(Color.cyan, Color.blue, .7f);
        public static Color ObjectFeedbackColor => Color.gray;
        public static Color CameraFeedbackColor => Color.red;
        public static Color RendererFeedbackColor => Color.Lerp(Color.yellow, Color.red, .5f);
        public static Color TransformFeedbackColor => Color.cyan;
        public static Color RectTransformFeedbackColor => Color.Lerp(Color.cyan, Color.blue, .3f);
        public static Color EtcFeedbackColor => Color.white;
    }
}