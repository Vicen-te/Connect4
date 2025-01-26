using Board;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Manages the user interface elements related to the game, including displaying the number of wins, draws, and discs used.
    /// Provides methods to update UI elements based on game statistics and track game progress.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        // UI elements to display the game stats (Wins, Discs, Draws)
        [Header("Wins")]
        [SerializeField] private TextMeshProUGUI wins; //< Text to display total wins
        [SerializeField] private TextMeshProUGUI firstWins; //< Text to display wins by the first player
        [SerializeField] private TextMeshProUGUI secondWins; //< Text to display wins by the second player
    
        [Header("Discs")]
        [SerializeField] private TextMeshProUGUI discs; //< Text to display current discs used
        [SerializeField] private TextMeshProUGUI firstDiscs; //< Text to display discs used by the first player
        [SerializeField] private TextMeshProUGUI secondDiscs; //< Text to display discs used by the second player

        [Header("Draws")]
        [SerializeField] private TextMeshProUGUI draws; //< Text to display the total draws

        // Variables to store the count of wins, draws, and discs
        private int drawsValue;
        private int winsValue;
        private int _firstWinsValue;
        private int _secondWinsValue;

        /// <summary>
        /// Initializes the UI with the board state and total plays.
        /// </summary>
        /// <param name="boardState">The current state of the game board.</param>
        /// <param name="totalPlays">Total number of plays to track statistics.</param>
        public void Initialize(BoardState boardState, int totalPlays)
        {
            UpdateDraws(totalPlays); //< Update the draw count
            UpdateWins(totalPlays);  //< Update the wins count
            UpdateFirstWins();       //< Update the wins for the first player
            UpdateSecondWins();      //< Update the wins for the second player
            UpdateDiscs(boardState); //< Update the number of discs in use
        }

        /// <summary>
        /// Updates the display of the discs used and their distribution between the players.
        /// </summary>
        /// <param name="boardState">The current state of the game board.</param>
        public void UpdateDiscs(BoardState boardState)
        {
            discs.text = $"Discs - {boardState.DiscInUse}/{boardState.Capacity}"; //< Total discs in use and capacity
            firstDiscs.text = $"First: {boardState.OpponentDisc}"; //< Discs used by the first player
            secondDiscs.text = $"Second: {boardState.AIDisc}"; //< Discs used by the second player
        }

        /// <summary>
        /// Registers a draw, incrementing the draw count and updating the UI.
        /// </summary>
        /// <param name="totalPlays">Total number of plays to track statistics.</param>
        public void RegisterDraw(int totalPlays)
        {
            ++drawsValue;            //< Increment draw count
            UpdateDraws(totalPlays); //< Update the draw UI
        }

        /// <summary>
        /// Updates the display of the draws.
        /// </summary>
        /// <param name="totalPlays">Total number of plays to track statistics.</param>
        private void UpdateDraws(int totalPlays)
        {
            draws.text = $"Draws - {drawsValue}/{totalPlays}"; //< Display current draws and total plays
        }

        /// <summary>
        /// Registers a win for a player (first or second) and updates the UI accordingly.
        /// </summary>
        /// <param name="index">The index of the player (0 for the first player, 1 for the second).</param>
        /// <param name="totalPlays">Total number of plays to track statistics.</param>
        public void RegisterWin(int index, int totalPlays)
        {
            if (index == 0) //< If the first player won
            {
                ++_firstWinsValue; //< Increment first player's win count
                UpdateFirstWins(); //< Update the UI for first player wins
            }
            else // If the second player won
            {
                ++_secondWinsValue; //< Increment second player's win count
                UpdateSecondWins(); //< Update the UI for second player wins
            }
        
            ++winsValue; //< Increment total wins count
            UpdateWins(totalPlays); //< Update the total wins UI
        }

        /// <summary>
        /// Updates the display of the total wins.
        /// </summary>
        /// <param name="totalPlays">Total number of plays to track statistics.</param>
        private void UpdateWins(int totalPlays)
        {
            wins.text = $"Wins - {winsValue}/{totalPlays}"; //< Display total wins and total plays
        }

        /// <summary>
        /// Updates the display of the first player's wins.
        /// </summary>
        private void UpdateFirstWins()
        {
            firstWins.text = $"First: {_firstWinsValue}"; //< Display first player's wins
        }

        /// <summary>
        /// Updates the display of the second player's wins.
        /// </summary>
        private void UpdateSecondWins()
        {
            secondWins.text = $"Second: {_secondWinsValue}"; //< Display second player's wins
        }
    }
}
