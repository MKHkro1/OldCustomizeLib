using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Mod
{
    internal static class ModImpl
    {
        /// <summary>
        /// 获取嵌入dll里的ab包
        /// </summary>
        /// <param name="assembly">要获取ab包的dll</param>
        /// <param name="name">名称</param>
        /// <returns>ab包</returns>
        /// <exception cref="ArgumentException"></exception>
        public static AssetBundle GetAssetBundle(Assembly assembly, string name)
        {
            try
            {
                if (LibData.Instance.LoadedAB.TryGetValue(name, out var assetBundle))
                {
                    Logger.LogInfo($"Successfully load AssetBundle {name}.");
                    return assetBundle;
                }
                using Stream stream =
                    assembly.GetManifestResourceStream(assembly.FullName!.Split(",")[0] + "." + name) ??
                    assembly.GetManifestResourceStream(name)!;
                using MemoryStream memStream = new();
                stream.CopyTo(memStream);
                var ab = AssetBundle.LoadFromMemory(memStream.ToArray());
                ArgumentNullException.ThrowIfNull(ab);
                Logger.LogInfo($"Successfully load AssetBundle {name}.");
                LibData.Instance.LoadedAB.Add(name, ab);
                return ab;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to load {name} \n{e}");
            }
        }
    }
}
