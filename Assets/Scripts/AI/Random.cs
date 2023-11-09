using Board;
using UnityEngine;

namespace AI
{
    public class Random : MonoBehaviour, IScript
    {
        public int ExecuteAlgorithm(BoardState boardState)
        {
            int random = UnityEngine.Random.Range(0, boardState.Columns);
            // Debug.Log($"Random Column: {random}");
            return random;
        }
    }
}