using Board;
using Core.Actor;
using UnityEngine;

namespace AI.Algorithms
{
    public class AspirationWindow : MonoBehaviour, IScript
    {
        private NegaMaxAB _negaMax;
        public int depth = 6;
        public int windowRange = 100;
        private int previousScore = int.MinValue;
        private int nodes;
        private readonly Average average = new();
        
        private void Start()
        {
            _negaMax = gameObject.AddComponent<NegaMaxAB>();
            _negaMax.depth = depth;
            TryGetComponent(out _negaMax);
        }
        
        public int ExecuteAlgorithm(BoardState boardState, int turn)
        {
            nodes = 0;
            _negaMax.ResetNodes();
            
            ActorTurn actorTurn = new ActorTurn(turn);
            Node.SetActorTurn(actorTurn);

            Node startNode = new Node(boardState, actorTurn.Opponent);
            int alpha = int.MinValue+1, beta = int.MaxValue;
            
            NodeMove result = AspirationWindowAlgorithm(startNode, alpha, beta);
            average.Add(nodes);
            Debug.Log($"value: {-result.Score}, column: {result.Column}, nodes: {nodes}, mean: {average.Value}");
            
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
                    nodes += _negaMax.Nodes;
                    if (currentMove.Score <= alpha) alpha = int.MinValue+1;
                    else if (currentMove.Score >= beta) beta = int.MaxValue;
                    else break;
                }

                previousScore = currentMove.Score;
            }
            else
            {
                currentMove = _negaMax.Algorithm(startNode, depth-1, alpha, beta);
                nodes += _negaMax.Nodes;
                previousScore = currentMove.Score;
            }
            
            return currentMove;
        }
    }
}