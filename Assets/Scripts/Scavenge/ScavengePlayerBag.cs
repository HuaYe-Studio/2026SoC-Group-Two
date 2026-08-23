namespace Scavenge
{
    public static class ScavengePlayerBag
    {
        private static BackpackSystemAdapter adapter;

        public static void EnsureWired()
        {
            if (adapter == null)
            {
                adapter = new BackpackSystemAdapter();
                if (adapter.HasBackpackSystem)
                    adapter.RebuildFromScene();
            }
        }

        public static Inventory Get()
        {
            var uiMgr = InventoryUIManager.Instance;
            if (uiMgr == null) return null;

            EnsureWired();
            if (adapter != null && adapter.HasBackpackSystem)
            {
                adapter.RebuildFromScene(); 
                uiMgr.SetPlayerInventory(adapter.Data);
                return adapter.Data;
            }

            return null;
        }

        public static bool RemoveRandomItem(out Item removed)
        {
            removed = null;
            EnsureWired();
            if (adapter == null || !adapter.HasBackpackSystem) return false;
            adapter.RebuildFromScene();
            return adapter.RemoveRandomItem(out removed);
        }

        public static void OnContainerOpened()
        {
            if (adapter == null || !adapter.HasBackpackSystem) return;
            adapter.RebuildFromScene();
            if (InventoryUIManager.Instance != null)
                InventoryUIManager.Instance.SetPlayerInventory(adapter.Data);
        }

        public static void OnContainerClosed()
        {
            if (adapter == null || !adapter.HasBackpackSystem) return;
            adapter.ApplyDataToScene();
        }
    }
}
