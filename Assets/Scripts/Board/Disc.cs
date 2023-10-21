using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class Disc : Circle
    {
        private int _actorOwner;
        public int ActorOwner => _actorOwner;
        public bool Visibility => spriteRenderer.enabled;

        private float _startYPosition;
        private float _lastYPosition;

        public void SetStartPosition(float startYPosition)
        {
            _startYPosition = startYPosition;
        }
        
        public void SetLastPosition(float lastYPosition)
        {
            _lastYPosition = lastYPosition;
        }
        
        public void SetVisibility(bool visibility)
        {
            spriteRenderer.enabled = visibility;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        public void SetActorOwner(int newActorOwner)
        {
            _actorOwner = newActorOwner;
        }

        public void StartAnimation()
        {
            StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            float elapsedTime = 0;
            const float waitTime = 0.2f;
            
            while (elapsedTime < waitTime)
            {
                // Lerp from Top to Bottom
                transform.position = 
                    new Vector2(
                            transform.position.x, 
                            Mathf.Lerp(_startYPosition, _lastYPosition, elapsedTime/waitTime)
                        );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector2(transform.position.x, _lastYPosition);
            yield return null;
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
