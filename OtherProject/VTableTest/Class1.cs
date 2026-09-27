using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VTableTest
{
    public class Class1
    {

    }

    public static unsafe class VTableHelper
    {
        // 4.0 的 Il2CppInterop 里 UnityVersionHandler.Wrap 返回的是 INativeMethodInfoStruct（值类型），
        // 不是指针，所以返回类型跟着改成它；否则 CS0029。原上游这里是空方法体（CS0161）。
        public static INativeMethodInfoStruct ConvertMethodInfo(MethodInfo method, INativeClassStruct declaring)
        {
            var cls = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "UltimateSpring");
            var strc = UnityVersionHandler.Wrap((Il2CppClass*)cls);
            var me = ((VirtualInvokeData*)strc.VTable)[61];
            var info = UnityVersionHandler.Wrap(me.method);
            Console.WriteLine($"{Marshal.PtrToStringUTF8(info.Name)}, {me.methodPtr:X}");
            return info;
        }
    }
}
