using System;
using UnityEngine;

namespace Core
{
    public class PlayerManager : ActorManager
    {
        public override void OnGameTurnChange()
        {
            base.OnGameTurnChange();
            // Turn column interaction on if it is the player's turn
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}