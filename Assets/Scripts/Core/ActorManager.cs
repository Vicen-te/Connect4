using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Core
{
    public abstract class ActorManager : MonoBehaviour
    {
        protected String ActorName => gameObject.name;
        [SerializeField] protected Color color;

        public Color GetColor()
        {
            return color;
        }
        
        public virtual void OnGameTurnChange()
        {
            // throw new System.NotImplementedException();
            
            Debug.Log(ActorName);
        }
    }
}
