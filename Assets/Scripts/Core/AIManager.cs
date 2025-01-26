using System;
using System.Collections;
using System.Diagnostics;
using AI;
using Board;
using Core.Actor;
using UnityEngine;
using Interaction;
using Debug = UnityEngine.Debug;

namespace Core
{
    /// <summary>
    /// Manages the AI actor's behavior during the game. 
    /// It handles the AI's turn, executes algorithms, and interacts with the game board.
    /// </summary>
    public class AIManager : ActorManager, IColumInteraction
    { 
        /// <summary>
        /// Reference to the script that contains the AI algorithm.
        /// </summary>
        private IScript _script;

        /// <summary>
        /// Time to wait before executing the AI's move.
        /// </summary>
        private float _waitSeconds = 0.2f;

        /// <summary>
        /// Event to trigger column interaction.
        /// </summary>
        public event IColumInteraction.Interaction OnInteraction;

        /// <summary>
        /// Stores average time of AI turn executions.
        /// </summary>
        private readonly Average _average = new();

        /// <summary>
        /// Attempts to retrieve the component implementing the AI algorithm.
        /// </summary>
        public void GetClassWithInterface()
        {
            TryGetComponent(out _script);
        }

        /// <summary>
        /// Sets the time to wait before AI executes its turn.
        /// </summary>
        /// <param name="waitSeconds">Time in seconds to wait before executing the move.</param>
        public void SetWaitSeconds(float waitSeconds)
        {
            _waitSeconds = waitSeconds;
        }
        
        /// <summary>
        /// Called when the game turn changes. The AI executes its algorithm to make a move.
        /// </summary>
        /// <param name="boardInfo">Information about the current board.</param>
        /// <param name="boardState">The current state of the board.</param>
        /// <param name="turn">The current turn of the game (AI or opponent).</param>
        public override void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            // Start a coroutine to handle the AI's turn and execute its move
            StartCoroutine(MakeInteraction(boardInfo, boardState, turn));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Coroutine that executes the AI move after waiting for a specified time.
        /// Measures the time taken for the move to be selected and logs performance.
        /// </summary>
        /// <param name="boardInfo">Information about the current board.</param>
        /// <param name="boardState">The current state of the board.</param>
        /// <param name="turn">The current turn of the game (AI or opponent).</param>
        private IEnumerator MakeInteraction(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            // Wait for the specified delay before AI moves
            yield return new WaitForSeconds(_waitSeconds);
            
            // Start a stopwatch to measure execution time of AI move
            Stopwatch stopWatch = Stopwatch.StartNew();
            
            // Execute the AI's algorithm to select a column to play in
            int selectedColumn = _script.ExecuteAlgorithm(boardState, turn);
            
            // Stop the stopwatch and calculate the elapsed time
            stopWatch.Stop();
            TimeSpan timeSpan = stopWatch.Elapsed;
            
            // Log the average time of AI's move execution
            _average.Add(timeSpan.TotalMilliseconds);
                
            // Debug log: elapsed time and average execution time
            Debug.Log($"{ActorName}:\nElapsed time: {timeSpan.TotalMilliseconds} milliseconds, Media: {_average.Value}");
            
            // Trigger the interaction event to indicate the AI's move
            OnInteraction?.Invoke(boardInfo.Columns[selectedColumn]);
            yield return null;
        }
    }
}
