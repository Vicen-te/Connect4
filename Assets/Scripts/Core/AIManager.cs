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
    public class AIManager : ActorManager, IColumInteraction
    { 
        private IScript _script;
        private float _waitSeconds = 0.2f;
        public event IColumInteraction.Interaction OnInteraction;
        private readonly Average average = new();

        public void GetClassWithInterface()
        {
            TryGetComponent(out _script);
        }

        public void SetWaitSeconds(float waitSeconds)
        {
            _waitSeconds = waitSeconds;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            //Debug.Log(ActorName);
            
            // Execute AI script
            StartCoroutine(MakeInteraction(boardInfo, boardState, turn));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MakeInteraction(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            yield return new WaitForSeconds(_waitSeconds);
            
            // Start StopWatch
            Stopwatch stopWatch = Stopwatch.StartNew();
            
            int selectedColumn = _script.ExecuteAlgorithm(boardState, turn);
            
            // Stop and debug elapsed time
            stopWatch.Stop();
            TimeSpan timeSpan = stopWatch.Elapsed;
            average.Add(timeSpan.TotalMilliseconds);
                
            // Debug
            Debug.Log($"{ActorName}:\nElapsed time: {timeSpan.TotalMilliseconds}, Media: {average.Value}");
            
            // Invoke delegate
            OnInteraction?.Invoke(boardInfo.Columns[selectedColumn]);
            yield return null;
        }
    }
}