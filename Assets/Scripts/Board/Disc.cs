using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Board
{
    public class Disc : Circle
    {
        public bool Visibility => spriteRenderer.enabled;
    
        public void SetVisibility(bool visibility)
        {
            spriteRenderer.enabled = visibility;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
    
        private void Animation()
        {
            // Lerp from Top to Bottom
        }  

        // Start is called before the first frame update
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}
