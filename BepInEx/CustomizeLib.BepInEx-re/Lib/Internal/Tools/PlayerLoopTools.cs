using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Test;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;

namespace CustomizeLib.BepInEx.Internal.Tools
{
    internal static class PlayerLoopTools
    {
        /// <summary>
        /// 获取 <paramref name="root"/> 中符合 <paramref name="target"/> 的节点的索引
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="target">目标节点</param>
        /// <returns>索引</returns>
        internal static List<int> GetIndex(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target)
        {
            var res = new List<int>();

            // 使用 BFS 遍历树形结构
            var queue = new Queue<(PlayerLoopSystem node, List<int> idx)>();
            queue.Enqueue((root, [])); // 根节点入队

            ValueTuple<PlayerLoopSystem, List<int>> tuple; // 当前节点
            PlayerLoopSystem current;
            List<int> idx;

            while (queue.Count > 0)
            {
                tuple = queue.Dequeue();
                (current, idx) = tuple; // 析构

                if (target.Invoke(current))
                {
                    res = idx;
                    break;
                }

                if (current.subSystemList != null)
                {
                    for (int i = 0; i < current.subSystemList.Length; i++)
                        queue.Enqueue((current.subSystemList[i], idx.Concat(i.ToIEnumerable()).ToList())); // 添加当前子节点的索引
                }
            }

            return res;
        }

        /// <summary>
        /// 访问在 <paramref name="root"/> 下索引为 <paramref name="idx"/> 的节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="idx">索引列表</param>
        /// <returns>指定节点</returns>
        internal static PlayerLoopSystem AccessIndex(PlayerLoopSystem root, List<int> idx)
        {
            var current = root;
            for (int i = 0;i < idx.Count; i++)
            {
                if (current.subSystemList == null) return current;
                current = current.subSystemList[idx[i]];
            }
            return current;
        }
        
        internal static void SetIndex(ref PlayerLoopSystem root, List<int> idx, PlayerLoopSystem value)
        {
            var stack = new Stack<PlayerLoopSystem>();
            var current = root;
            for (int i = 0; i < idx.Count; i++)
            {
                if (current.subSystemList == null) break;
                stack.Push(current);
                current = current.subSystemList[idx[i]];
            }
            current = value;
            for (int i = idx.Count - 1; i >= 0; i--)
            {
                var pop = stack.Pop();
                pop.subSystemList[idx[i]] = current;
                current = pop;
            }
            root = current;
        }

        /// <summary>
        /// 在 <paramref name="root"/> 中符合 <paramref name="target"/> 的 <see cref="PlayerLoopSystem"/> 前插入 <paramref name="cons"/>
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="cons">新节点的构造函数</param>
        /// <returns>修改后的 <see cref="PlayerLoopSystem"/></returns>
        /// <exception cref="ArgumentException">若目标节点为根节点时抛出</exception>
        internal static PlayerLoopSystem InsertSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons)
        {
            var newRoot = root;
            var targetIdx = GetIndex(newRoot, target);
            if (targetIdx.Count <= 0) throw new ArgumentException($"The target can't be a root node in PlayerLoopSystem");
            var parent = AccessIndex(newRoot, targetIdx.GetRange(0, targetIdx.Count - 1));

            var oldSub = parent.subSystemList;
            var newSub = new PlayerLoopSystem[oldSub.Length + 1];
            newSub[targetIdx[^1]] = cons.Invoke(); // 取倒数第一个 (指定节点) 的获取到的索引

            // 复制旧的system (指定索引之前)
            for (int i = 0; i < targetIdx[^1]; i++)
                newSub[i] = oldSub[i];
            // 指定位已经赋值过了
            // 复制旧的system (指定索引之后)
            for (int i = targetIdx[^1] + 1; i < newSub.Length; i++)
                newSub[i] = oldSub[i - 1];

            parent.subSystemList = newSub;
            SetIndex(ref newRoot, targetIdx.GetRange(0, targetIdx.Count - 1), parent);
            return newRoot;
        }

        /// <summary>
        /// 在 <paramref name="root"/> 中符合 <paramref name="target"/> 的 <see cref="PlayerLoopSystem"/> 后追加 <paramref name="cons"/>
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="cons">新节点的构造函数</param>
        /// <returns>修改后的 <see cref="PlayerLoopSystem"/></returns>
        /// <exception cref="ArgumentException">若目标节点为根节点时抛出</exception>
        internal static PlayerLoopSystem AppendSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons)
        {
            var newRoot = root;
            var targetIdx = GetIndex(newRoot, target);
            if (targetIdx.Count <= 0) throw new ArgumentException($"The target can't be a root node in PlayerLoopSystem");
            var parent = AccessIndex(newRoot, targetIdx.GetRange(0, targetIdx.Count - 1));

            var oldSub = parent.subSystemList;
            var newSub = new PlayerLoopSystem[oldSub.Length + 1];
            newSub[targetIdx[^1] + 1] = cons.Invoke(); // 取倒数第一个 (指定节点) 的获取到的索引

            // 复制旧的system (指定索引及之前)
            for (int i = 0; i <= targetIdx[^1]; i++)
                newSub[i] = oldSub[i];
            // 指定位已经赋值过了
            // 复制旧的system (指定索引之后)
            for (int i = targetIdx[^1] + 2; i < newSub.Length; i++)
                newSub[i] = oldSub[i - 1];

            parent.subSystemList = newSub;
            SetIndex(ref newRoot, targetIdx.GetRange(0, targetIdx.Count - 1), parent);
            return newRoot;
        }

        /// <summary>
        /// 根据 <paramref name="targetType"/> 判断 <paramref name="system"/> 是否符合条件
        /// </summary>
        /// <param name="system">判定节点</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>是否符合</returns>
        internal static bool CheckByType(PlayerLoopSystem system, Type targetType) =>
            system.type != null && system.type == Il2CppType.From(targetType);

        /// <summary>
        /// 构造 <see cref="PlayerLoopSystem"/>
        /// </summary>
        /// <param name="type">type 成员</param>
        /// <param name="updateDelegate">updateDelegate 成员</param>
        /// <returns>构造的 <see cref="PlayerLoopSystem"/></returns>
        internal static PlayerLoopSystem ConsLoopSystem(Type type, Action updateDelegate) =>
            new()
            {
                type = Il2CppType.From(type),
                updateDelegate = updateDelegate
            };

        /// <summary>
        /// 构造 <see cref="PlayerLoopSystem"/>
        /// </summary>
        /// <param name="type">type 成员</param>
        /// <param name="updateDelegate">updateDelegate 成员</param>
        /// <param name="subs">subSystemList 成员</param>
        /// <returns>构造的 <see cref="PlayerLoopSystem"/></returns>
        internal static PlayerLoopSystem ConsLoopSystem(Type type, Action updateDelegate, IEnumerable<PlayerLoopSystem> subs) =>
            new()
            {
                type = Il2CppType.From(type),
                updateDelegate = updateDelegate,
                subSystemList = subs.ToArray()
            };
    }
}
