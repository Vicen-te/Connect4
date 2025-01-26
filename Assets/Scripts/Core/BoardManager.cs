using Board;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// BoardManager handles the setup and initialization of the game board
    /// </summary>
    public sealed class BoardManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the BoardLoader component responsible for building the board
        /// </summary>
        [SerializeField] private BoardLoader boardLoader;
        
        /// <summary>
        /// Sets up the scene by creating the game board and returning board information.
        /// </summary>
        /// <returns>A BoardInfo object containing information about the board.</returns>
        public BoardInfo SetupScene()
        {
            // Build the board using the BoardLoader
            boardLoader.BuildBoard();
            
            // Create and return a reference object that holds board information
            return new BoardInfo(boardLoader);
        }
    }
}