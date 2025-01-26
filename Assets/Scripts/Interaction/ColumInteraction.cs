namespace Interaction
{
    /// <summary>
    /// Interface that defines interaction behavior for columns in the game.
    /// It includes a delegate to notify when a column is interacted with.
    /// </summary>
    public interface IColumInteraction
    {
        /// <summary>
        /// Delegate to handle column interactions.
        /// The interaction involves a specific column being selected or interacted with by a player or AI.
        /// </summary>
        /// <param name="column">The column that is being interacted with.</param>
        public delegate void Interaction(Column column);
    }
}