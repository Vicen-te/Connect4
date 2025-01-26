using System.Collections.Generic;
using Interaction;

namespace Board
{
    /// <summary>
    /// This class serves as a data structure that holds the state of the game board.
    /// It provides access to board dimensions, columns, discs, and allows interaction with columns.
    /// </summary>
    public sealed class BoardInfo
    {
        // Board dimensions and capacity
        public readonly byte NumColumns;  //< Number of columns on the board
        public readonly byte NumRows;     //< Number of rows on the board
        public readonly ushort Capacity;  //< Total number of spaces (Capacity = Columns * Rows)

        // Lists that hold the board's columns and discs
        public readonly List<Disc> Discs;  //< List of discs on the board
        public readonly List<Column> Columns;  //< List of columns on the board

        // Delegate and reference for resetting discs
        private delegate void ResetDiscsLoader();
        private readonly ResetDiscsLoader resetDiscsLoader;  //< Holds the reset discs method

        /// <summary>
        /// Constructor that initializes the BoardInfo from a BoardLoader instance.
        /// </summary>
        /// <param name="boardLoader">The BoardLoader instance that provides the board data.</param>
        public BoardInfo(BoardLoader boardLoader)
        {
            // Initialize board dimensions, capacity, and lists from BoardLoader
            NumColumns = boardLoader.NumColumns;
            NumRows = boardLoader.NumRows;
            Capacity = boardLoader.Capacity;
            Discs = boardLoader.Discs;
            Columns = boardLoader.Columns;
            resetDiscsLoader = boardLoader.ResetDiscs;  //< Set the reset discs method reference
        }
        
        // Accessor Methods
        
        /// <summary>
        /// Gets the index of a given column in the list of columns.
        /// </summary>
        /// <param name="column">The column whose index is to be retrieved.</param>
        /// <returns>The index of the column in the list.</returns>
        public int GetColumnIndex(Column column) => Columns.FindIndex(element => element == column);
        
        /// <summary>
        /// Gets the disc at the specified index in the list of discs.
        /// </summary>
        /// <param name="index">The index of the disc in the list.</param>
        /// <returns>The disc at the specified index.</returns>
        public Disc GetDisc(int index) => Discs[index];
        
        
        // Mutator Methods
        
        /// <summary>
        /// Resets all discs by invoking the reset method stored in the resetDiscsLoader delegate.
        /// </summary>
        public void ResetDiscs() => resetDiscsLoader.Invoke();
        
        /// <summary>
        /// Enables the specified interaction method for all columns on the board.
        /// </summary>
        /// <param name="method">The interaction method to be triggered on column interaction.</param>
        public void EnableColumnInteractions(IColumInteraction.Interaction method)
        {
            // Loop through all columns and add the interaction method to their event
            for (byte columnIndex = 0; columnIndex < NumColumns; ++columnIndex)
            {
                Columns[columnIndex].OnInteraction += method;
            }
        }

        /// <summary>
        /// Disables the specified interaction method for all columns on the board.
        /// </summary>
        /// <param name="method">The interaction method to be removed from column interactions.</param>
        public void DisableColumnInteractions(IColumInteraction.Interaction method)
        {
            // Loop through all columns and remove the interaction method from their event
            for (byte columnIndex = 0; columnIndex < NumColumns; ++columnIndex)
            {
               Columns[columnIndex].OnInteraction -= method;
            }
        }
    }
}
