using System;

namespace NPC
{
    public static class DialogueEvents
    {
        public static event Action<string, object> OnRaised;

        public static void Raise(string id, object data) => OnRaised?.Invoke(id, data);
        public static void Raise(string id) => OnRaised?.Invoke(id, null);
    }
}