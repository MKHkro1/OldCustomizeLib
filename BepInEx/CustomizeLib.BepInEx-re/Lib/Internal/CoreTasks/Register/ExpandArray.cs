using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    // 扩容资源数组
    internal struct ExpandArray : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IModTask.Do()
        {

        }
    }
}
