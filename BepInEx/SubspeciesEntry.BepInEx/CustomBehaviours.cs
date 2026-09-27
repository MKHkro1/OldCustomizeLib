using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubspeciesEntry.BepInEx
{
    public static class CustomBehaviours
    {
        public static void Init()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Timer>();
            ClassInjector.RegisterTypeInIl2Cpp<ComponentRunner>();
        }
    }

    public class Timer : MonoBehaviour
    {
        public List<Timers> timers { get; set; } = new();

        public void Update()
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                if (!timer.canReplace)
                {
                    timer.Update();
                    timers[i] = timer;
                }
            }
        }

        public void OnDestroy()
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                if (!timer.canReplace)
                {
                    timer.action?.Invoke();
                }
            }
        }

        public void AddTimer(float time, Action action)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                if (!timer.canReplace) continue;
                timer.time = time;
                timer.action = action;
                timer.OnStart();
                timers[i] = timer;
                return;
            }
            timers.Add(new Timers
            {
                time = time,
                action = action,
                canReplace = false
            });
        }

        public struct Timers
        {
            public float time = 0f;
            public Action? action;
            public bool canReplace = false;

            public void OnStart()
            {
                canReplace = false;
            }

            public void Update()
            {
                time -= Time.deltaTime;
                if (time <= 0f && action != null)
                {
                    action.Invoke();
                    canReplace = true;
                }
            }

            public Timers() { }
        }
    }

    public class ComponentRunner : MonoBehaviour
    {
        public Action? OnAwake;
        public Action? OnStart;
        public Action? OnUpdate;
        public Action? OnFixedUpdate;
        public Action? OnDestroyAction;

        public void Awake() => OnAwake?.Invoke();
        public void Start() => OnStart?.Invoke();
        public void Update() => OnUpdate?.Invoke();
        public void FixedUpdate() => OnFixedUpdate?.Invoke();
        public void OnDestroy() => OnDestroyAction?.Invoke();
    }
}
