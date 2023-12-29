#if USE_CINEMACHINE
using System;
using Cinemachine;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class CinemachineImpulseFeedback : IFeedback, IFeedbackTagColor ,IFeedbackSceneRepaint,IFeedbackWaringMessageBox
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private CinemachineImpulseReferenceMode referenceMode;
        [SerializeField] private bool velocityRandom = true;
        [SerializeField] [DisplayIf(nameof(velocityRandom),false)] private Vector3 velocity;
        [SerializeField] [DisplayIf(nameof(referenceMode),0)] private CinemachineImpulseSource impulseSource;
        [SerializeField] [DisplayIf(nameof(referenceMode),1)] [NoCopy]private CinemachineImpulseDefinition impulseDefinition = new();

        public bool IsActive { get; set; } = true;

        public string Name => "Camera/Cinemachine Impulse";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            Handle = LMotion.Create(0, 0, 0).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithOnComplete(() =>
                {
                    CinemachineImpulseManager.Instance.IgnoreTimeScale = options.ignoreTimeScale;
                    if(velocityRandom) velocity = UnityEngine.Random.insideUnitSphere;
                    switch (referenceMode)
                    {
                        case CinemachineImpulseReferenceMode.ImpulseSource:
                            impulseSource.GenerateImpulse(velocity);
                            break;
                        case CinemachineImpulseReferenceMode.ImpulseDefinition:
                            if (Camera.main != null)
                                impulseDefinition.CreateEvent(Camera.main.transform.position, velocity);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                })
#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;
        public string WarningMessage => "This feedback can't play in editor";
    }
}
#endif //USE_CINEMACHINE