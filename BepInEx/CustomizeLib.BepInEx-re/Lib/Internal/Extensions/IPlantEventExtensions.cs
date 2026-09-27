using CustomizeLib.BepInEx.Internal.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class IPlantEventExtensions
    {
        /// <inheritdoc cref="IPlantEvent.Register"/>
        internal static IPlantEvent Register(this IPlantEvent plantEvent) => plantEvent.Register();

        /// <inheritdoc cref="IPlantEvent.Unregister"/>
        internal static IPlantEvent Unregister(this IPlantEvent plantEvent) => plantEvent.Unregister();

        /// <summary>
        /// 获取 <paramref name="obj"/> 上所有绑定的 IPlantEvent
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="plantEvents">所有已绑定的 IPlantEvent</param>
        /// <returns>是否拥有已绑定元素</returns>
        internal static bool TryGetBindEvents(this object obj, out List<IPlantEvent> plantEvents)
        {
            var res = IPlantEvent.GetBindEvents(obj);
            if (obj is IPlantEvent plantEvent) res.Add(plantEvent);
            plantEvents = res;
            return res.Count > 0;
        }

        /// <inheritdoc cref="IPlantEvent.Bind(object)"/>
        internal static void Bind(this IPlantEvent plantEvent, object obj) =>
            plantEvent.Bind(obj);
    }
}
