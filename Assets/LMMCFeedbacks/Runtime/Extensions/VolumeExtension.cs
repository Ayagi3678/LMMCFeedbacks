using UnityEngine.Rendering;

namespace LMMCFeedbacks.Extensions
{
    public static class VolumeExtension
    {
        public static T TryGetVolumeComponent<T>(this Volume volume) where T : VolumeComponent
        {
            if (volume.profile.TryGet(out T t)) return t;
            var component = volume.profile.Add(typeof(T));
            return component as T;
        }
        public static void EnableVolumeParameter(this VolumeComponent volumeComponent,VolumeParameter volumeParameter)
        {
            volumeComponent.active= true;
            volumeParameter.overrideState = true;
        }
        public static void EnableVolumeParameter(this VolumeParameter volumeParameter)
        {
            volumeParameter.overrideState = true;
        }
        public static void EnableVolumeparameterAll(this VolumeComponent volumeComponent)
        {
            volumeComponent.active = true;
            for (var i = 0; i < volumeComponent.parameters.Count; i++)
            {
                var t = volumeComponent.parameters[i];
                t.overrideState = true;
            }
        }
    }
}