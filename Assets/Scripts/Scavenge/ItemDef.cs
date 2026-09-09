using UnityEngine;

namespace Scavenge
{
    /// <summary>扫雷物品配置（队友的 Item 类不可序列化，用此 ScriptableObject 在 Inspector 配置，运行时 ToItem() 转成 Item）</summary>
    [CreateAssetMenu(fileName = "ScavengeItemDef", menuName = "Scavenge/ItemDef")]
    public class ItemDef : ScriptableObject
    {
        [Header("物品")]
        [Tooltip("物品名")]
        public string itemName = "物品";
        [Tooltip("物品图标")]
        public Sprite icon;
        [Tooltip("占据格宽")]
        public int boundWidth = 1;
        [Tooltip("占据格高")]
        public int boundHeight = 1;

        public Item ToItem()
        {
            return new Item
            {
                itemName = itemName,
                icon = icon,
                boundWidth = boundWidth,
                boundHeight = boundHeight
            };
        }
    }
}
