using Board;
using UnityEngine;

namespace Core
{
    namespace Actor
    {
        /// <summary>
        /// Represents the turn of an actor (AI or opponent) in the game.
        /// Stores the AI's turn and calculates the opponent's turn based on it.
        /// </summary>
        public readonly struct ActorTurn
        {
            /// <summary>
            /// Represents the opponent's player ID.
            /// </summary>
            public readonly int Opponent;

            /// <summary>
            /// Represents the AI player's ID.
            /// </summary>
            public readonly int AI;
            
            /// <summary>
            /// Initializes a new instance of the ActorTurn struct, 
            /// calculating the opponent's player ID based on the AI's turn.
            /// </summary>
            /// <param name="ai">The ID of the AI player (0 or 1).</param>
            public ActorTurn(int ai)
            {
                // Assign the opponent's ID based on the AI's turn. If AI is 0, opponent is 1, and vice versa.
                Opponent = ai == 0 ? 1 : 0;
                AI = ai;
            }
        }

        /// <summary>
        /// Abstract class that handles the management of an actor's turn in the game.
        /// Derived classes must implement the logic for handling changes in the game turn.
        /// </summary>
        public abstract class ActorManager : MonoBehaviour
        {
            /// <summary>
            /// Gets the name of the actor from the GameObject this script is attached to.
            /// </summary>
            protected string ActorName => gameObject.name;

            /// <summary>
            /// The color associated with the actor, typically representing the player or team.
            /// </summary>
            [SerializeField] protected Color color;

            /// <summary>
            /// Gets the color of the actor.
            /// </summary>
            /// <returns>The color of the actor.</returns>
            public Color GetColor()
            {
                return color;
            }

            /// <summary>
            /// Abstract method to handle game turn changes.
            /// </summary>
            /// <param name="boardInfo">Information about the board state.</param>
            /// <param name="boardState">The current state of the board.</param>
            /// <param name="turn">The current turn in the game.</param>
            public abstract void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn);
        }
    }
}
