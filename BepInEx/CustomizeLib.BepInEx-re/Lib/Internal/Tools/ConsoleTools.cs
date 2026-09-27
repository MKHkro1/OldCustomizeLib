using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Tools
{
    internal static class ConsoleTools
    {
        /// <summary>
        /// 设置控制台输出编码
        /// </summary>
        /// <param name="encoding">编码</param>
        internal static void SetOutputEncoding(Encoding encoding)
        {
            if (Application.platform != RuntimePlatform.Android)
                Console.OutputEncoding = encoding;
        }

        /// <summary>
        /// 设置控制台输出编码 (UTF8)
        /// </summary>
        internal static void SetOutputEncoding() => SetOutputEncoding(Encoding.UTF8);
    }
}
