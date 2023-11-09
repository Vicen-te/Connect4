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
            NodeValue result = AspirationWindowAlgorithm(startNode, alpha, beta);
            Debug.Log($"Final AWA:\n value: {result.Value}, column: {result.Column}");
            return result.Column;
        }

        private NodeValue AspirationWindowAlgorithm(Node startNode, int alpha, int beta)
        {
            NodeValue move;
            if (previousScore != int.MinValue)
            {
                alpha = previousScore - windowRange;
                beta = previousScore + windowRange;

                while (true)
                {
                    move = _negaMax.NegaMaxAlgorithm(startNode, depth-1, alpha, beta);
                    if (move.Value <= alpha) alpha = int.MinValue+1;
                    else if (move.Value >= beta) beta = int.MaxValue;
                    else break;
                }

                previousScore = move.Value;
            }
            else
            {
                move = _negaMax.NegaMaxAlgorithm(startNode, depth-1, alpha, beta);
                previousScore = move.Value;
            }
            
            return move;
        }
    }
}