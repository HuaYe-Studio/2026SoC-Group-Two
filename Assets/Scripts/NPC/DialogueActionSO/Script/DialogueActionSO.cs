using System;
using UnityEngine;


namespace NPC
{
    //该类用于实现对话里的动作
    public abstract class DialogueActionSO : ScriptableObject
    {
        public abstract void Execute(DialogueContext context);
    }
    
}