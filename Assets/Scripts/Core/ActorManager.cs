using Board;
using UnityEngine;

namespace Core
{
    namespace Actor
    {
        public readonly struct ActorTurn
        {
            public readonly int Opponent;
            public readonly int AI;
            public ActorTurn(int opponent, int ai)
            {
                Opponent = opponent;
                AI = ai;
            }
            public ActorTurn(int ai)
            {
                Opponent = ai == 0 ? 1 : 0;
                AI = ai;
            }
        }
        
        public abstract class ActorManager : MonoBehaviour
        {
            protected string ActorName => gameObject.name;
            [SerializeField] protected Color color;

            public Color GetColor()
            {
                return color;
            }

            public abstract void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn);
        }
    }
}
