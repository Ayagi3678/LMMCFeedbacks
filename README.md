# LMMCFeedbacks :)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
- LMMCFeedbacks is a feedback create tool that runs on the high-performance tweening library LitMotion.
- Feedback can be easily created on the inspector.
### Install for UPM
```
https://github.com/Ayagi3678/LMMCFeedbacks.git?path=/Assets/LMMCFeedbacks
```

> `Package Manager` -> `+` -> `Add package from git URL...`

### Must Dependencies (Install Together)
- [LitMotion](https://github.com/AnnulusGames/LitMotion)   : Tween
- [UniTask](https://github.com/Cysharp/UniTask)   : async/await

### Other Dependencies
- Cinemachine
# HOW TO USE
1.  Attach FeedbackPlayer to any GameObject. (`Add Component` -> `Feedback Player`)
2.  Add any Feedback to FeedbackPlayer.
3.  Expand Feedback and set the appropriate values. You can play to see how it works.
4.  Call FeedbackPlayer's Play() method at any time you want.

## More Settings
- Play Mode
  - `Concurrent` - Play Feedback in FeedbackPlayer at the same time.
  - `Sequencial` - Play Feedback in FeedbackPlayer one after another.
- Loop
  - Repeat FeedbackPlayer playback.
- Play On Start
  - Play FeedbackPlayer when call start method.
