using Board;
using UnityEngine;

namespace Core
{
    public abstract class ActorManager : MonoBehaviour
    {
        protected string ActorName => gameObject.name;
        [SerializeField] protected Color color;

        public Color GetColor()
        {
            return color;
        }

        public abstract void OnGameTurnChange(BoardInfo boardInfo);
    }
}
