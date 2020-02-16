using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Event
{
    public delegate void EventCallback(object param);

    class EventDispatcher
    {
        static Dictionary<int, Dictionary<object, EventCallback>> eventMap = new Dictionary<int, Dictionary<object, EventCallback>>();
        public static void DispatchEvent(int eventId, object param)
        {
            var objMap = eventMap[eventId];
            foreach (var callBack in objMap.Values)
            {
                callBack(param);
            }
        }

        public static void RegisterEvent(object obj, int eventId, EventCallback callback)
        {
            if (!eventMap.ContainsKey(eventId))
            {
                eventMap[eventId] = new Dictionary<object, EventCallback>();
            }

            eventMap[eventId][obj] = callback;
        }
    }
}
