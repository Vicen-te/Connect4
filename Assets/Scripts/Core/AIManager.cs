using System;
using AI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public class AIManager : ActorManager
    { 
        [SerializeField]
        private Object script; // works
        public AIScript Script => script as AIScript;

        public override void OnGameTurnChange()
        {
            base.OnGameTurnChange();
            // Turn column off if it is the AI's turn
        }
    }
}