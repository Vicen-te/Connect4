using Board;
using Core.Actor;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the player's actions and turn in the game.
    /// This class extends `ActorManager` to handle player-specific behavior.
    /// </summary>
    public class PlayerManager : ActorManager
    {
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Called when the game turn changes. Logs the player's name (actor) for debugging purposes.
        /// </summary>
        /// <param name="boardInfo">Information about the current board.</param>
        /// <param name="boardState">The current state of the board.</param>
        /// <param name="turn">The current turn of the game (AI or opponent).</param>
        public override void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            // Log the player's name (actor) when their turn changes
            Debug.Log(ActorName);
        }
    }
}