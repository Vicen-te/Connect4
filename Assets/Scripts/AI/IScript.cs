using Board;

namespace AI
{
    /// <summary>
    /// Interface for AI algorithms that determines the next move based on the current board state and the player's turn.
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// Executes the algorithm to determine the next move based on the current board state and the player's turn.
        /// </summary>
        /// <param name="boardState">The current state of the game board.</param>
        /// <param name="turn">The current player's turn (either 0 or 1).</param>
        /// <returns>The index of the column where the next disc will be placed.</returns>
        int ExecuteAlgorithm(BoardState boardState, int turn);
    }
}