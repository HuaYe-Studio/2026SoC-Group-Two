using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace NPC
{
        [CreateAssetMenu(menuName = "Dialogue/Node")]
        public class DialogueNodeSO : ScriptableObject
        {
                public DialogueLine[] lines;
                public DialogueOption[] options;
        }
        
        [Serializable]
        public class DialogueLine
        {
                public string speakerName;
                public Sprite npcSprite;
                [TextArea(3, 5)] public string text;
        }

        public class DialogueContext
        {
                public GameObject Initiator;
                public GameObject Target;
        }
        
        [Serializable]
        public class DialogueOption
        {
                public string optionText;
                public DialogueNodeSO nextNode;
                public DialogueActionSO[] onSelect; 
                public DialogueConditionSO[] conditions;
        }
}