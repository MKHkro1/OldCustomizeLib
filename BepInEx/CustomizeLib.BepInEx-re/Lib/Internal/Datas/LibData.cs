using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Datas
{
    // 存储lib自己的数据
    internal struct LibData
    {
        #region 基础成员
        internal static LibData Instance;

        public LibData() => Instance = this;
        #endregion

        internal ManualLogSource? Logger { get; set; } = null;
        internal Dictionary<string, AssetBundle> LoadedAB { get; set; } = [];
    }
}
