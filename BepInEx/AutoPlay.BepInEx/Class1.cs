using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using RhythmGame;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Core;

namespace AutoPlay.BepInEx
{
    [BepInPlugin("salmon.autoplay", "AutoPlay", "1.0.0")]
    public class Core : BasePlugin
    {
        public static bool isAutoPlay = false;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    /// <summary>
    /// AutoPlay功能补丁 - 零误差版
    /// </summary>
    [HarmonyPatch]
    public static class AutoPlayPatch
    {
        // 存储每个音符的AutoPlay状态
        private static readonly ConditionalWeakTable<FallingNote, AutoPlayState> _autoPlayStates = new ConditionalWeakTable<FallingNote, AutoPlayState>();

        // 判定区域范围
        private const float JUDGE_RANGE = 0.5f;

        // 时间容错
        private const float TIME_TOLERANCE = 0.05f;

        public class AutoPlayState
        {
            public bool hasAutoPlayed;
            public Coroutine holdCompleteCoroutine;
        }

        // 协程调度器
        private static MonoBehaviour _coroutineRunner;

        /// <summary>
        /// 设置协程运行器
        /// </summary>
        public static void SetCoroutineRunner(MonoBehaviour runner)
        {
            _coroutineRunner = runner;
        }

        /// <summary>
        /// 获取或创建音符的AutoPlay状态
        /// </summary>
        public static AutoPlayState GetState(FallingNote note)
        {
            if (!_autoPlayStates.TryGetValue(note, out var state))
            {
                state = new AutoPlayState();
                _autoPlayStates.Add(note, state);
            }
            return state;
        }

        /// <summary>
        /// 清理状态
        /// </summary>
        public static void CleanupState(FallingNote note)
        {
            if (note != null)
            {
                var state = GetState(note);
                if (state.holdCompleteCoroutine != null && _coroutineRunner != null)
                {
                    _coroutineRunner.StopCoroutine(state.holdCompleteCoroutine);
                }
                _autoPlayStates.Remove(note);
            }
        }

        /// <summary>
        /// 补丁FallingNote的Update方法
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FallingNote), "Update")]
        public static void Update_Postfix(FallingNote __instance)
        {
            if (!Core.isAutoPlay) return;
            if (GameAPP.theGameStatus != 0) return;
            if (RhythmGameManager.Instance == null) return;
            if (RhythmGameManager.Instance.isPaused) return;
            if (__instance.isClicked || __instance.hasMissed) return;
            if (__instance.noteType == NoteType.Hold && __instance.holdCompleted) return;

            float currentTime = RhythmGameManager.Instance.currentBGMTime;

            if (__instance.noteType == NoteType.Hold)
            {
                ProcessHoldNoteAutoPlay(__instance, currentTime);
            }
            else
            {
                ProcessNormalNoteAutoPlay(__instance, currentTime);
            }
        }

        /// <summary>
        /// 处理普通音符的AutoPlay
        /// </summary>
        private static void ProcessNormalNoteAutoPlay(FallingNote note, float currentTime)
        {
            var state = GetState(note);
            if (state.hasAutoPlayed) return;
            if (note.isClicked || note.hasMissed) return;

            float y = (note.targetTime - currentTime) * note.fallSpeed + note.targetY;
            float distanceToJudgeLine = Mathf.Abs(y - note.targetY);
            float timeDiff = currentTime - note.targetTime;

            bool shouldTrigger = false;

            if (distanceToJudgeLine <= JUDGE_RANGE)
            {
                shouldTrigger = true;
            }
            else if (Mathf.Abs(timeDiff) <= TIME_TOLERANCE)
            {
                shouldTrigger = true;
            }
            else if (y < note.targetY && !note.isClicked && !note.hasMissed)
            {
                shouldTrigger = true;
            }

            if (shouldTrigger)
            {
                state.hasAutoPlayed = true;
                ExecuteClick(note);
            }
        }

        /// <summary>
        /// 处理长按音符的AutoPlay
        /// </summary>
        private static void ProcessHoldNoteAutoPlay(FallingNote note, float currentTime)
        {
            var state = GetState(note);
            float endTime = note.targetTime + note.holdDuration;
            float timeToTarget = note.targetTime - currentTime;

            if (!state.hasAutoPlayed && !note.isHolding && !note.holdCompleted && !note.hasMissed)
            {
                float currentY = note.transform.position.y;
                float distanceToJudgeLine = Mathf.Abs(currentY - note.targetY);

                bool shouldStartHold = false;

                if (distanceToJudgeLine <= JUDGE_RANGE)
                {
                    shouldStartHold = true;
                }
                else if (Mathf.Abs(timeToTarget) <= TIME_TOLERANCE)
                {
                    shouldStartHold = true;
                }
                else if (currentY < note.targetY)
                {
                    shouldStartHold = true;
                }

                if (shouldStartHold)
                {
                    AutoPlayHoldStart(note);
                    state.hasAutoPlayed = true;
                }
            }

            if (note.isHolding && !note.holdCompleted)
            {
                if (RhythmGameManager.Instance.holdKeyPressed != null &&
                    note.trackIndex >= 0 &&
                    note.trackIndex < RhythmGameManager.Instance.holdKeyPressed.Length)
                {
                    if (!RhythmGameManager.Instance.holdKeyPressed[note.trackIndex])
                    {
                        RhythmGameManager.Instance.holdKeyPressed[note.trackIndex] = true;
                    }
                }

                // 节拍判定
                if (endTime > currentTime)
                {
                    if (note.lastBeatJudgeTime < 0f)
                        note.lastBeatJudgeTime = note.targetTime;

                    float nextBeat = note.lastBeatJudgeTime + note.beatDuration;
                    if (currentTime >= nextBeat)
                    {
                        RhythmGameManager.Instance.OnHoldBeatJudge(note, currentTime);
                        note.lastBeatJudgeTime = nextBeat;
                    }
                }
            }

            // 强制完成长按
            if (note.isHolding && !note.holdCompleted && currentTime >= endTime + 0.05f)
            {
                note.holdCompleted = true;
                RhythmGameManager.Instance.OnHoldNoteComplete(note, endTime);
                note.HideNote();
                CleanupState(note);
            }
        }

        /// <summary>
        /// 执行点击判定 - 强制完美判定
        /// </summary>
        private static void ExecuteClick(FallingNote note)
        {
            if (note == null) return;
            if (note.isClicked || note.hasMissed) return;

            note.isClicked = true;

            if (RhythmGameManager.Instance != null)
            {
                // 使用目标时间而非当前时间，强制完美判定
                RhythmGameManager.Instance.OnNoteClicked(note, note.targetTime);
            }

            if (note.noteType != NoteType.Hold)
            {
                note.HideNote();
                CleanupState(note);
            }
        }

        /// <summary>
        /// AutoPlay自动开始长按
        /// </summary>
        public static void AutoPlayHoldStart(FallingNote note)
        {
            if (note.isHolding || note.holdCompleted || note.hasMissed) return;

            note.isHolding = true;

            if (RhythmGameManager.Instance.holdKeyPressed != null &&
                note.trackIndex >= 0 &&
                note.trackIndex < RhythmGameManager.Instance.holdKeyPressed.Length)
            {
                RhythmGameManager.Instance.holdKeyPressed[note.trackIndex] = true;
            }
        }
    }

    /// <summary>
    /// 拦截手动点击和错误判定
    /// </summary>
    [HarmonyPatch]
    public static class ManualInputPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FallingNote), "OnClick")]
        public static bool OnClick_Prefix()
        {
            return !Core.isAutoPlay;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FallingNote), "OnHoldStart")]
        public static bool OnHoldStart_Prefix()
        {
            return !Core.isAutoPlay;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FallingNote), "OnMiss")]
        public static bool OnMiss_Prefix(FallingNote __instance)
        {
            if (!Core.isAutoPlay) return true;
            if (__instance.isClicked || __instance.hasMissed) return true;

            if (__instance.noteType == NoteType.Hold)
            {
                if (!__instance.isHolding && !__instance.holdCompleted)
                {
                    AutoPlayPatch.AutoPlayHoldStart(__instance);

                    if (AutoPlayPatch.GetState(__instance).holdCompleteCoroutine == null)
                    {
                        if (AutoPlayPatchExtensions.TryGetCoroutineRunner(out var runner))
                        {
                            var coroutine = DelayedHoldComplete(__instance, __instance.holdDuration);
                            AutoPlayPatch.GetState(__instance).holdCompleteCoroutine = runner.StartCoroutine(coroutine.WrapToIl2Cpp());
                        }
                        else
                        {
                            __instance.holdCompleted = true;
                            if (RhythmGameManager.Instance != null)
                            {
                                RhythmGameManager.Instance.OnHoldNoteComplete(__instance, __instance.targetTime + __instance.holdDuration);
                            }
                            __instance.HideNote();
                            AutoPlayPatch.CleanupState(__instance);
                        }
                    }
                }
                else if (__instance.isHolding && !__instance.holdCompleted)
                {
                    __instance.holdCompleted = true;
                    if (RhythmGameManager.Instance != null)
                    {
                        RhythmGameManager.Instance.OnHoldNoteComplete(__instance, __instance.targetTime + __instance.holdDuration);
                    }
                    __instance.HideNote();
                    AutoPlayPatch.CleanupState(__instance);
                }
            }
            else
            {
                if (!__instance.isClicked)
                {
                    __instance.isClicked = true;
                    if (RhythmGameManager.Instance != null)
                    {
                        RhythmGameManager.Instance.OnNoteClicked(__instance, __instance.targetTime);
                    }
                    __instance.HideNote();
                    AutoPlayPatch.CleanupState(__instance);
                }
            }

            return false;
        }

        private static IEnumerator DelayedHoldComplete(FallingNote note, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (note != null && !note.holdCompleted)
            {
                note.holdCompleted = true;
                if (RhythmGameManager.Instance != null)
                {
                    RhythmGameManager.Instance.OnHoldNoteComplete(note, note.targetTime + note.holdDuration);
                }
                note.HideNote();
                AutoPlayPatch.CleanupState(note);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FallingNote), "OnHoldFailed")]
        public static bool OnHoldFailed_Prefix(FallingNote __instance)
        {
            if (Core.isAutoPlay && __instance.isHolding && !__instance.holdCompleted)
            {
                __instance.holdCompleted = true;
                if (RhythmGameManager.Instance != null)
                {
                    RhythmGameManager.Instance.OnHoldNoteComplete(__instance, __instance.targetTime + __instance.holdDuration);
                }
                __instance.HideNote();
                AutoPlayPatch.CleanupState(__instance);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class AutoPlayPatchExtensions
    {
        public static bool TryGetCoroutineRunner(out MonoBehaviour runner)
        {
            runner = null;

            if (RhythmGameManager.Instance != null && RhythmGameManager.Instance is MonoBehaviour mb)
            {
                runner = mb;
                return true;
            }

            var fallback = UnityEngine.Object.FindObjectOfType<RhythmGameManager>();
            if (fallback != null)
            {
                runner = fallback;
                return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CheatKey))]
    public static class CheatKeyPatch
    {
        [HarmonyPatch(nameof(CheatKey.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(CheatKey __instance)
        {
            Action action = () =>
            {
                Core.isAutoPlay = !Core.isAutoPlay;
                InGameText.Instance.ShowText($"自动播放已{(Core.isAutoPlay ? "开启" : "关闭")}", 1f);
            };
            __instance.CheatKeys.Add("auto", action);
        }
    }
}
