using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class Space : Circle
    {
        public bool Free { get; private set; }

        private Space()
        {
            Free = true;
        }
    
        // Start is called before the first frame update
        private void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}