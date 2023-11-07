using System.Collections;
using AI;
using Board;
using UnityEngine;
using Interaction;

namespace Core
{
    public class AIManager : ActorManager, IColumInteraction
    { 
        private IScript _script;
        private float _waitSeconds = 0.2f;
        public event IColumInteraction.Interaction OnInteraction;

        public void GetClassWithInterface()
        {
            TryGetComponent(out _script);
        }

        public void SetWaitSeconds(float waitSeconds)
        {
            _waitSeconds = waitSeconds;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState)
        {
            //Debug.Log(ActorName);
            
            // Execute AI script
            StartCoroutine(MakeInteraction(boardInfo, boardState));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MakeInteraction(BoardInfo boardInfo, BoardState boardState)
        {
            yield return new WaitForSeconds(_waitSeconds);
            int selectedColumn = _script.ExecuteAlgorithm(boardState); 
            
            // Invoke delegate
            OnInteraction?.Invoke(boardInfo.Columns[selectedColumn]);
            yield return null;
        }
    }
}