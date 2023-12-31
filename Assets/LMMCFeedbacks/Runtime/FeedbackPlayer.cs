using System;
using System.Collections.Generic;
using System.Threading;
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
        public FeedbackPlayerOption options;

        private readonly List<UniTask> _feedbackTaskCaches = new();

        public readonly Action OnCompleted;

        private CancellationTokenSource _playCancellationTokenSource;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        [SerializeReference] public List<IFeedback> Feedbacks = new();

        private void Start()
        {
            if (playOnAwake) Play();
        }

        private void OnDisable()
        {
            _playCancellationTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Play()
        {
            _playCancellationTokenSource = new CancellationTokenSource();
            switch (playMode)
            {
                case FeedbackPlayMode.Concurrent:
                    PlayConcurrent(_playCancellationTokenSource.Token).Forget();
                    break;
                case FeedbackPlayMode.Sequential:
                    PlaySequential(_playCancellationTokenSource.Token).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Stop()
        {
            _playCancellationTokenSource?.Cancel();
            foreach (var feedback in Feedbacks) feedback.Complete();
        }

        public void Initialize()
        {
            Stop();
            foreach (var feedback in Feedbacks)
                if (feedback is IFeedbackInitializable initializable)
                    initializable.Initialize();
        }

        public void InitialSetup()
        {
            foreach (var feedback in Feedbacks)
                if (feedback is IFeedbackInitializable initializable)
                    initializable.InitialSetup();
        }

        public async UniTask PlayConcurrent(CancellationToken cancellationToken)
        {
            for (var i = 0; i < (options.loop ? options.loopCount : 1); i++)
            {
                _feedbackTaskCaches.Clear();
                foreach (var feedback in Feedbacks)
                {
                    if (!feedback.IsActive) continue;
                    if (feedback is not IFeedbackHold)
                        _feedbackTaskCaches.Add(feedback.Create().ToUniTask(cancellationToken));
                    else await feedback.Create().ToUniTask(cancellationToken);
                }

                await UniTask.WhenAll(_feedbackTaskCaches);
            }

            if (options.initializeOnComplete) Initialize();
            OnCompleted?.Invoke();
        }

        public async UniTask PlaySequential(CancellationToken cancellationToken)
        {
            for (var i = 0; i < (options.loop ? options.loopCount : 1); i++)
                foreach (var feedback in Feedbacks)
                {
                    if (!feedback.IsActive) continue;
                    await feedback.Create().ToUniTask(cancellationToken);
                }

            if (options.initializeOnComplete) Initialize();
            OnCompleted?.Invoke();
        }
    }
}