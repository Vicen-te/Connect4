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
        public event IColumInteraction.Interaction OnInteraction;

        public void GetClassWithInterface()
        {
            TryGetComponent(out _script);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnGameTurnChange(BoardInfo boardInfo)
        {
            //Debug.Log(ActorName);
            
            // Execute AI script
            StartCoroutine(MakeInteraction(boardInfo));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MakeInteraction(BoardInfo boardInfo)
        {
            yield return new WaitForSeconds(0.2f);
            Column selectedColumn = _script.ExecuteAlgorithm(boardInfo); 
            
            // Invoke delegate
            OnInteraction?.Invoke(selectedColumn);
            yield return null;
        }
    }
}