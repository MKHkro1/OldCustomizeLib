using CustomizeLib.BepInEx.Internal;
using CustomizeLib.BepInEx.Internal.Events;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    #region 事件
    public interface IModTask
    {
        public const int Low = 0;
        public const int Default = 1;
        public const int High = 2;

        /// <summary>
        /// 加载顺序, 值小的优先
        /// </summary>
        public int GetOrder() => Default;

        /// <summary>
        /// 当触发时
        /// </summary>
        public void Do();

        protected static IEnumerable<T> GetAllTask<T>() where T : IModTask
        {
            return InternalTools.TypeTools_GetAllDerivedTypes<T>(InternalTools.TypeTools_GetAllAssemblies(), false).Select(t =>
            {
                return (T)Activator.CreateInstance(t)!;
            }).OrderBy(task => task.GetOrder());
        }

        protected static void DoAll<T>() where T : IModTask
        {
            foreach (var task in GetAllTask<T>()) task.Do();
        }
    }

    public interface ILoadTask : IModTask
    {
        /// <summary>
        /// 执行所有 <see cref="ILoadTask"/>
        /// </summary>
        internal static void RunAll() => DoAll<ILoadTask>();
    }

    public interface IGameAppEvent : IModTask
    {
        public GameAppEvent Filter => GameAppEvent.Default;


        internal static void ManualRunAll(GameAppEvent call)
        {
            foreach (var task in GetAllTask<IGameAppEvent>())
                if ((task.Filter & call) != 0) task.Do();
        }

        [Flags]
        public enum GameAppEvent
        {
            Default = 0,
            PreAwake = 1,
            PostAwake = 2,
            PreStart = 4,
            PostStart = 8,
            PreLoadResources = 16,
            PostLoadResources = 32,
            All = PreAwake | PostAwake | PreStart | PostStart | PreLoadResources | PostLoadResources
        }
    }
    #endregion
}
