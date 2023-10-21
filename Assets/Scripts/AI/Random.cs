using Board;
using Interaction;
using UnityEngine;

namespace AI
{
    public class Random : MonoBehaviour, IScript
    {
        public Column ExecuteAlgorithm(BoardInfo boardInfo)
        {
            int random = UnityEngine.Random.Range(0, boardInfo.Columns.Count);
            return boardInfo.Columns[random];
        }
    }
}