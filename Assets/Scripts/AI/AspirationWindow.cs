using Board;
using UnityEngine;

namespace AI
{
    public class AspirationWindow : MonoBehaviour, IScript
    {
        private NegaMax _negaMax;
        public int depth = 6;
        public int windowRange = 100;
        private int previousScore = int.MinValue;

        private void Start()
        {
            _negaMax = gameObject.AddComponent<NegaMax>();
            _negaMax.depth = depth;
            TryGetComponent(out _negaMax);
        }
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            Node startNode = new Node(boardState, (int)Actor.Player);
            int alpha = int.MinValue+1, beta = int.MaxValue;
            
            NodeMove result = AspirationWindowAlgorithm(startNode, alpha, beta);
            Debug.Log($"Final AWA:\n value: {-result.Score}, column: {result.Column}");
            
            return result.Column;
        }

        private NodeMove AspirationWindowAlgorithm(Node startNode, int alpha, int beta)
        {
            NodeMove currentMove;
            if (previousScore != int.MinValue)
            {
                alpha = previousScore - windowRange;
                beta = previousScore + windowRange;

                while (true)
                {
                    currentMove = _negaMax.Algorithm(startNode, depth-1, alpha, beta);
                    if (currentMove.Score <= alpha) alpha = int.MinValue+1;
                    else if (currentMove.Score >= beta) beta = int.MaxValue;
                    else break;
                }

                previousScore = currentMove.Score;
            }
            else
            {
                currentMove = _negaMax.Algorithm(startNode, depth-1, alpha, beta);
                previousScore = currentMove.Score;
            }
            
            return currentMove;
        }
    }
}