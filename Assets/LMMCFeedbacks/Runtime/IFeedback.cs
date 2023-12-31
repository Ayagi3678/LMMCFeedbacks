using LitMotion;
using LMMCFeedbacks.Runtime;

public interface IFeedback
{
    public bool IsActive { get; set; }
    public string Name { get; }
    public FeedbackOption Options { get; }
    public MotionHandle Handle { get; }
    public MotionHandle Create();

    public void Complete();
}