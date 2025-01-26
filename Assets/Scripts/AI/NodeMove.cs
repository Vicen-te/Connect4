namespace AI
{
    /// <summary>
    /// Represents a move made by a player in the game.
    /// Stores the score of the move and the column where it was made.
    /// </summary>
    public struct NodeMove
    {
        /// <summary>
        /// The score of the move.
        /// </summary>
        public int Score;
        
        /// <summary>
        /// The column in which the move was made.
        /// </summary>
        public readonly int Column;

        
        /// <summary>
        /// Initializes a new instance of the NodeMove struct.
        /// </summary>
        /// <param name="value">The score of the move.</param>
        /// <param name="column">The column where the move was made.</param>
        public NodeMove(int value, int column)
        {
            Score = value;
            Column = column;
        }

        /// <summary>
        /// Inverts the score of the move.
        /// </summary>
        public void InverseScore()
        {
            Score = -Score;
        }
    }
}