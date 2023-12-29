using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    public class FeedbackPlayer : MonoBehaviour
    {
        public bool playOnAwake;
        public FeedbackPlayMode playMode;
        public bool loop;
        [Min(1)] public int loopCount = 1;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        [SerializeReference]public List<IFeedback> Feedbacks = new ();

        public readonly Action OnCompleted;

        private readonly List<UniTask> _feedbackTaskCaches = new();

        private void Start()
        {
            if(playOnAwake) Play();
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Play()
        {
            switch (playMode)
            {
                case FeedbackPlayMode.Concurrent:
                    PlayConcurrent().Forget();
                    break;
                case FeedbackPlayMode.Sequential:
                    PlaySequential().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Stop()
        {
            foreach (var feedback in Feedbacks)
            {
                feedback.Cancel();
            }
        }

        public async UniTask PlayConcurrent()
        {
            for (int i = 0; i < (loop?loopCount:1); i++)
            {
                _feedbackTaskCaches.Clear();
                foreach (var feedback in Feedbacks)
                {
                    if(!feedback.IsActive) continue;
                    if(feedback is not IFeedbackHold) _feedbackTaskCaches.Add(feedback.Create().ToUniTask(destroyCancellationToken));
                    else await feedback.Create().ToUniTask(destroyCancellationToken);
                }
                await UniTask.WhenAll(_feedbackTaskCaches);
            }
            OnCompleted?.Invoke();
        }
        public async UniTask PlaySequential()
        {
            for (int i = 0; i < (loop?loopCount:1); i++)
            {
                foreach (var feedback in Feedbacks)
                {
                    if(!feedback.IsActive) continue;
                    await feedback.Create().ToUniTask(destroyCancellationToken);
                }
            }
            OnCompleted?.Invoke();
        }
    }
}