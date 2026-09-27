using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    internal struct RegTypeData : ILoadTask
    {
        readonly void IModTask.Do()
        {
            foreach (var (pt, tags) in RegData.Instance.TypeExtraMgr.CustomPlantTypeDatas)
                foreach (var tag in tags)
                    ProcessItem(pt, tag);

            foreach (var (pt, tags) in RegData.Instance.TypeExtraMgr.CustomZombieTypeDatas)
                foreach (var tag in tags)
                    ProcessItem(pt, tag);
        }

        private static void ProcessItem<T>(T item, string tag)
        {
            TryAddItemTo(item, typeof(TypeData), tag);
            TryAddItemTo(item, typeof(TypeMgr), tag);
        }

        private static void TryAddItemTo<T>(T item, Type type, string tag)
        {
            var prop = type.GetProperty(tag, InternalTools.TypeTools_DefaultFlag);
            if (prop != null && prop.CanRead && prop.CanWrite)
            {
                var obj = prop.GetValue(null);
                if (obj is Il2CppSystem.Collections.Generic.HashSet<T> set)
                {
                    set.Add(item);
                    prop.SetValue(null, set); // 写回
                }
            }
        }
    }
}
