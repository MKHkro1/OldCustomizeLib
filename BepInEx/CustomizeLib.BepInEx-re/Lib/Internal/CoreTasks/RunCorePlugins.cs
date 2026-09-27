using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks
{
    internal struct RunCorePlugins : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PostStart;

        readonly void IModTask.Do()
        {
            foreach (var callback in CorePlugin.OnLoadCallbacks) callback.Invoke();
        }
    }
}
