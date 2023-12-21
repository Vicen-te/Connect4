using Board;
using Core.Actor;
using UnityEngine;

namespace Core
{
    public class PlayerManager : ActorManager
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnGameTurnChange(BoardInfo boardInfo, BoardState boardState, int turn)
        {
            Debug.Log(ActorName);
        }
    }
}