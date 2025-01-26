using System.Collections;
using System.Collections.Generic;
using AI.Algorithms.MTD;
using Board;
using Core.Actor;
using Interaction;
using UI;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the overall game flow, including turns, actors, and interactions with the board.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>Total number of actors (players and AI).</summary>
        private int _totalActors = 0;

        /// <summary>Reference to the BoardManager, responsible for managing the board setup and visuals.</summary>
        [SerializeField] private BoardManager boardManager;
        
        /// <summary>The index of the player who starts the game (0-based).</summary>
        [SerializeField] private int initialPlayerIndex = 0;
        
        /// <summary>Contains information about the current state of the board.</summary>
        private BoardInfo _boardInfo;
        
        /// <summary>Tracks the logical state of the board, including disc positions.</summary>
        private BoardState _boardState;

        /// <summary>Total number of automated plays (used in simulations).</summary>
        [SerializeField] private int maxAutomatedPlays = 100;
        
        /// <summary>Remaining number of automated plays.</summary>
        private int _remainingAutomatedPlays;

        /// <summary>Tracks the index of the current turn (0-based).</summary>
        private int _currentTurnIndex;
    
        /// <summary>The actor currently taking their turn.</summary>
        private ActorManager _currentActor;
        
        /// <summary>List of all actors in the game (players and AI).</summary>
        [SerializeField] private List<ActorManager> actors;
        
        /// <summary>Delay in seconds between AI interactions.</summary>
        [Header("AI"), SerializeField] private float aiInteractionDelay = 0.2f;
        
        /// <summary>Delay in seconds between AI turns.</summary>
        [SerializeField] private float aiTurnDelay = 1f;

        /// <summary>Reference to the UIManager, which updates the game UI.</summary>
        [Header("UI"), SerializeField] private UIManager uiManager;
        
        /// <summary>Determines whether animations are enabled for disc placement.</summary>
        [SerializeField] private bool enableDiscAnimations;

        
        /// <summary>
        /// Called when the game starts. Initializes the board, actors, and other game settings.
        /// </summary>
        private void Start()
        {
            int aiCount = 0;
            
            // Initialize AI actors and count them
            foreach (ActorManager actorManager in actors)
            {
                if (actorManager is not AIManager aiManager) continue;
                aiManager.GetClassWithInterface();
                aiManager.OnInteraction += HandleMoveCompletion;
                aiManager.SetWaitSeconds(aiInteractionDelay);
                ++aiCount;
            }
            
            _totalActors = actors.Count;

            // Validate the number of AI actors
            if (aiCount > 2 || (_totalActors > 2 && aiCount > 0))
            {
                Debug.LogError("Only up to two AI players are supported.");
                return;
            }
            
            // Set up the board and initialize the game
            _boardInfo = boardManager.SetupScene();
            _remainingAutomatedPlays = maxAutomatedPlays;
            InitializeGame();
        }

        /// <summary>
        /// Initializes the game state, setting up turns and starting values.
        /// </summary>
        private void InitializeGame()
        {
            // Subtract 1 to set the first player correctly (startPlayer - 1)
            _currentTurnIndex = initialPlayerIndex-1;
            
            // Create a new board state based on the current board info
            _boardState = new BoardState(_boardInfo);
        
            // Change turn to the next player
            ChangeTurn();
            
            // Update the control interactions based on the current actor's type (player or AI)
            UpdateActorControls();
            
            // Initialize UI with current board state and automated plays remaining
            uiManager.Initialize(_boardState, maxAutomatedPlays);
        }
    
        /// <summary>
        /// Changes the turn to the next actor in the list and notifies them.
        /// </summary>
        private void ChangeTurn()
        {
            // Move to the next actor, looping back to the first actor when necessary
            _currentTurnIndex = (_currentTurnIndex + 1) % _totalActors;
            
            // Set the current actor based on the updated turn index
            _currentActor = actors[_currentTurnIndex];
            
            // Notify the actor that it's their turn and pass the current board state
            _currentActor.OnGameTurnChange(_boardInfo, _boardState, _currentTurnIndex);
        }

        /// <summary>
        /// Updates the board interaction controls based on the current actor's type.
        /// Player has control over columns, while AI does not.
        /// </summary>
        private void UpdateActorControls()
        {
            // Check if the current actor is a player (AI doesn't need column interactions)
            bool isPlayerTurn = _currentActor is PlayerManager;
            UpdateColumnInteractions(isPlayerTurn);
        }
        
        /// <summary>
        /// Enables or disables column interactions on the board depending on the player's or AI's turn.
        /// </summary>
        /// <param name="enable">True to enable column interactions (for players), false for AI turns.</param>
        private void UpdateColumnInteractions(bool enable)
        {
            if (enable)
                _boardInfo.EnableColumnInteractions(HandleMoveCompletion);
            else
                _boardInfo.DisableColumnInteractions(HandleMoveCompletion);
        }

        /// <summary>
        /// Handles the completion of a move in a specific column and updates the board.
        /// This is triggered once a disc is placed.
        /// </summary>
        /// <param name="column">The column where the move was made.</param>
        private void HandleMoveCompletion(Column column)
        {
            // Get the index of the column where the move was made
            int columnIndex = _boardInfo.GetColumnIndex(column);
            
            // Check if the column is full (if so, retry the turn)
            if (!_boardState.IsColumnEmpty(columnIndex))
            {
                _currentTurnIndex -= 1;
                ChangeTurn();
                return;
            }
            
            // Get the next available disc index and place the disc in the column
            int discIndex = _boardState.GetNextAvailableDiscIndex(columnIndex);
            _boardState.PlaceDisc(discIndex, _currentTurnIndex);
            
            // Get the disc object and update its properties
            Disc disc = _boardInfo.GetDisc(discIndex);
            disc.SetVisibility(true);
            
            // Start disc animation if enabled
            if(enableDiscAnimations) 
                disc.StartAnimation();
            
            // Set the color of the disc to the current player's color
            disc.SetColor(_currentActor.GetColor());
            
            // Update the UI with the current board state
            uiManager.UpdateDiscs(_boardState);
            
            // Disable further column interactions until the next turn
            UpdateColumnInteractions(false);
            
            // Check if there's a winner or a draw 
            if (_boardState.HasWinner(_currentTurnIndex))
            {
                uiManager.RegisterWin(_currentTurnIndex, maxAutomatedPlays);
                _boardState.PrintResult($"Win {maxAutomatedPlays-_remainingAutomatedPlays} - Turn {_currentTurnIndex}");
                RestartGame();
            }
            else if (_boardState.Draw())
            {
                uiManager.RegisterDraw(maxAutomatedPlays);
                _boardState.PrintResult($"Draw");
                RestartGame();
            }
            else
            {
                // Change to the next turn and update controls
                ChangeTurn();
                UpdateActorControls();
            }
        }

        /// <summary>
        /// Restarts the game for the next round or ends it if no plays remain.
        /// </summary>
        private void RestartGame()
        {
            // Decrease the remaining automated plays counter
            --_remainingAutomatedPlays;
            
            // Print the result of the game
            _boardState.LogResult();

            // If no more plays remain, exit the game
            if (_remainingAutomatedPlays == 0)
            {
                Debug.Log("Exiting...");
                return;
            }

            // Reinitialize AI (if any) for the next round
            foreach (ActorManager actor in actors)
            {
                if (actor.TryGetComponent(out Mtd mtd))
                {
                    mtd.Initialize(_boardState.Capacity);
                }
            }

            // Start a coroutine to reset the board for the next round
            StartCoroutine(ResetBoard());
        }

        /// <summary>
        /// Resets the board and starts a new game round.
        /// </summary>
        private IEnumerator ResetBoard()
        {
            // Wait for a short delay before resetting the board
            yield return new WaitForSeconds(aiTurnDelay);
            _boardInfo.ResetDiscs();
            InitializeGame();
        }
    }
}
