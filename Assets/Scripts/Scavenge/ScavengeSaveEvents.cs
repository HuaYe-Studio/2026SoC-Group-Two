using System;
using UnityEngine;

namespace Scavenge
{
    public static class ScavengeSaveEvents
    {
        public static event Action OnWorldSaved;

        public static void NotifyWorldSaved()
        {
            Debug.Log("[Scavenge] 拾荒点刷新");
            OnWorldSaved?.Invoke();
        }
    }
}
