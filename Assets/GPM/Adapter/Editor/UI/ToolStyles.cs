#if UNITY_EDITOR

using UnityEngine;

namespace Gpm.Adapter.Tool
{
    public static class ToolStyles
    {
        public static readonly GUIStyle ErrorLabel;
        public static readonly GUIStyle DefaultButton;
        public static readonly GUIStyle DefaultToggle;
        public static readonly GUIStyle CopyrightBox;
        public static readonly GUIStyle CopyrightLabel;

        static ToolStyles()
        {
            ErrorLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                margin = new RectOffset(5, 5, 5, 5),
                fontSize = 12,
                richText = true
            };

            DefaultButton = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 10, 10)
            };
            
            DefaultToggle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 12
            };

            CopyrightBox = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.LowerCenter
            };

            CopyrightLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.gray },
                fontSize = 10
            };
        }
    }
}

#endif