using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Events
{
    #region 基础部分
    /// <summary>
    /// 植物事件
    /// </summary>
    public partial interface IPlantEvent
    {
        public void OnUpdate(Trigger trigger) { }
        public void OnFixedUpdate(Trigger trigger) { }
        public void DieEvent(Trigger trigger, Plant.DieReason reason = Plant.DieReason.Default) { }
        public void DieEventMustExecute(Trigger trigger, Plant.DieReason reason = Plant.DieReason.Default) { }
    }
    #endregion

    #region 处理部分
    public partial interface IPlantEvent
    {
        internal static int CleanBindMod { get; set; } = 20; // 20次绑定清理一次字典

        /// <summary>
        /// 所有已注册的 Event
        /// </summary>
        internal static List<IPlantEvent> RegisterEvents { get; set; } = [];
        /// <summary>
        /// 绑定 IPlantEvent与对象
        /// </summary>
        internal static Dictionary<object, List<IPlantEvent>> BindingObjs { get; set; } = [];
        /// <summary>
        /// 对应Event触发的事件
        /// </summary>
        internal static Dictionary<EventType, Action<Trigger, object[]>> EventMaps { get; set; } = new()
        {
            [EventType.OnUpdate] = (trigger, _) => Execute(trigger, EventType.OnUpdate, [], []),
            [EventType.OnFixedUpdate] = (trigger, _) => Execute(trigger, EventType.OnFixedUpdate, [], []) 
        };
        /// <summary>
        /// 对应Event触发的事件 (仅对单实例生效)
        /// </summary>
        internal static Dictionary<EventType, Action<IPlantEvent, Trigger, object[]>> EventSingleMaps { get; set; } = new()
        {
            [EventType.DieEvent] = (plantEvent, trigger, arr) => ExecuteSingle(plantEvent, trigger, EventType.DieEvent, [typeof(Plant.DieReason)], arr),
            [EventType.DieEventMustExecute] = (plantEvent, trigger, arr) => ExecuteSingle(plantEvent, trigger, EventType.DieEventMustExecute, [typeof(Plant.DieReason)], arr)
        };
        /// <summary>
        /// 缓存的某一类型的所有事件方法
        /// </summary>
        internal static Dictionary<Type, Dictionary<EventType, MethodInfo?>> CachedTypeMethods { get; set; } = [];
        /// <summary>
        /// 所有EventType列表
        /// </summary>
        internal static Lazy<List<EventType>> EventTypes { get; set; } = new(() => [.. Enum.GetValues<EventType>()]);
        /// <summary>
        /// EventType的名字列表
        /// </summary>
        internal static Lazy<List<string>> EventTypesName { get; set; } = new(() =>
        {
            var res = new List<string>();
            foreach (var type in EventTypes.Value)
                res.Add(type.ToString());
            return res;
        });
        /// <summary>
        /// 根据 string 反查 EventType
        /// </summary>
        internal static Lazy<Dictionary<string, EventType>> EventTypeNamesReverse { get; set; } = new(() =>
            EventTypesName.Value.ToDictionary(str => str, str => Enum.Parse<EventType>(str)));
        /// <summary>
        /// 缓存的方法与其触发器
        /// </summary>
        internal static Dictionary<MethodInfo, HashSet<Trigger>> CachedMethodTriggers { get; set; } = [];

        #region 静态方法
        /// <summary>
        /// 清理已绑定的对象
        /// </summary>
        private static void CleanBind()
        {
            var cache = BindingObjs.ToList();
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                if (cache[i].Key == null) cache.RemoveAt(i);
                else cache[i] = new(cache[i].Key, [.. cache[i].Value.Where(i => i != null)]);
            }
            BindingObjs = cache.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// 获取 <paramref name="obj"/> 上已绑定的 IPlantEvent
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>已绑定的 IPlantEvent</returns>
        internal static List<IPlantEvent> GetBindEvents(object obj) =>
            BindingObjs.GetValueOrDefault(obj, []);
        #endregion

        /// <summary>
        /// 添加到列表
        /// </summary>
        public IPlantEvent Register()
        {
            if (RegisterEvents.Contains(this)) Logger.LogWarning($"You are registering a {GetType()} instance that has already been registered. Are you sure?");
            RegisterEvents.Add(this);
            return this;
        }

        /// <summary>
        /// 从列表中移除
        /// </summary>
        public IPlantEvent Unregister()
        {
            RegisterEvents.Remove(this);
            return this;
        }

        /// <summary>
        /// 将本 IPlantEvent 与 <paramref name="obj"/> 绑定
        /// </summary>
        /// <param name="obj">绑定对象</param>
        public void Bind(object obj)
        {
            if (obj == null) return;
            if (BindingObjs.Count % CleanBindMod == 0) CleanBind();
            var list = BindingObjs.GetValueOrDefault(obj, []);
            list.Add(this);
            BindingObjs[obj] = list;
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="type">类型</param>
        /// <param name="extraArgs">额外参数类型</param>
        /// <param name="args">额外参数</param>
        internal static void Execute(Trigger trigger, EventType type, Type[] extraArgs, object[] args)
        {
            var cacheList = RegisterEvents;
            for (int i = 0; i < cacheList.Count; i++)
            {
                // cacheList[i] 为 null
                if (cacheList[i] == null)
                {
                    cacheList.RemoveAt(i);
                    continue;
                }
                // cacheList[i] 为 MonoBehaviour且未启用, 跳过但不移除 (可能还会启用)
                if (cacheList[i] is MonoBehaviour behaviour && !behaviour.isActiveAndEnabled) continue;
                ExecuteSingle(cacheList[i], trigger, type, extraArgs, args);
            }
            RegisterEvents = cacheList;
        }

        /// <summary>
        /// 执行事件 (单例)
        /// </summary>
        /// <param name="plantEvent">目标单例</param>
        /// <param name="trigger">触发器</param>
        /// <param name="type">类型</param>
        /// <param name="extraArgs">额外参数类型</param>
        /// <param name="args">额外参数</param>
        internal static void ExecuteSingle(IPlantEvent plantEvent, Trigger trigger, EventType type, Type[] extraArgs, object[] args) 
        {
            if (plantEvent == null) return;
            var cachedEventType = plantEvent.GetType();
            InitCachedMethod(cachedEventType);

            // 搜索第一个参数为 Trigger, 且后续参数符合 extraArgs 的方法
            var method = CachedTypeMethods[cachedEventType][type];
            if (method == null) return;
            if (!CheckTrigger(method, trigger)) return;
            method.Invoke(plantEvent, [trigger, ..args]);
        }

        /// <summary>
        /// 初始化缓存方法
        /// </summary>
        /// <param name="type">类型</param>
        private static void InitCachedMethod(Type type)
        {
            if (CachedTypeMethods.ContainsKey(type)) return;

            var value = new Dictionary<EventType, MethodInfo?>();

            // 初始化类里已有代码的 dictionary
            foreach (var method in type.FindMembers(MemberTypes.Method, InternalTools.TypeTools_DefaultDeclaredOnly, // 所有方法且仅在本类中定义
                (m, criteria) => ((List<string>)criteria!).Any(str => m.Name.EndsWith(str)), EventTypesName.Value).  // 所有方法结尾名称包含了 EventType 里任意一枚举值的 ToString
                Cast<MethodInfo>()) // 转为 MethodInfo
            {
                var methodName = method.Name[(method.Name.LastIndexOf('.') + 1)..]; // 从最后一个.(不含)到末尾
                if (!EventTypeNamesReverse.Value.TryGetValue(methodName, out var eventType)) continue; // 获取不到对应的就返回
                value.Add(eventType, method);
            }

            // 为代码里没有定义的 event 赋值，防止读取时提示键不存在
            foreach (var eventType in EventTypes.Value)
                if (!value.ContainsKey(eventType)) value[eventType] = null;

            CachedTypeMethods[type] = value;
        }

        /// <summary>
        /// 检查方法是否匹配此触发器
        /// </summary>
        /// <param name="info">方法</param>
        /// <param name="trigger">触发器</param>
        /// <returns>是否匹配</returns>
        internal static bool CheckTrigger(MethodInfo info, Trigger trigger)
        {
            // 如果不存在，则进行缓存
            if (!CachedMethodTriggers.TryGetValue(info, out var value))
            {
                var attrs = info.GetCustomAttributes<TriggerAttribute>(false);
                HashSet<Trigger> set = [.. attrs.Select(attr => attr.Trigger)];
                if (set.Count <= 0) set = [Trigger.Pre, Trigger.Post]; // 默认所有触发
                CachedMethodTriggers[info] = value = set;
            }
            return value.Contains(trigger);
        }

        internal enum EventType
        {
            OnUpdate,
            OnFixedUpdate,
            DieEvent,
            DieEventMustExecute
        }
    }
    #endregion

    #region 工具类
    public enum Trigger
    {
        Pre,
        Post
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerAttribute(Trigger trigger = Trigger.Post) : Attribute
    {
        internal Trigger Trigger { get; set; } = trigger;
    }
    #endregion
}
