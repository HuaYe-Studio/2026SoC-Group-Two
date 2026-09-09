using UnityEngine;
using UnityEngine.UI;

namespace Scavenge
{
    public static class ScavengeUI
    {
        private static Font builtinFont;

        
        public static Font BuiltinFont
        {
            get
            {
                if (builtinFont == null)
                    builtinFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                return builtinFont;
            }
        }

        public static Text CreateText(Transform parent, string name, string content, Color color, int fontSize,
            TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var text = go.GetComponent<Text>();
            text.font = BuiltinFont;
            text.text = content;
            text.color = color;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.fontStyle = style;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Outline AddOutline(Text text)
        {
            var outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            return outline;
        }
    }
}
