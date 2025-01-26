using System.Collections.Generic;
using Board;
using Core.Actor;
using UnityEngine;

namespace AI.Algorithms
{
    public class NegaMax : MonoBehaviour, IScript
    {
        public int depth = 6;
        private int nodes;
        private readonly Average average = new();
        
        public int ExecuteAlgorithm(BoardState boardState, int turn)
        {
            nodes = 0;
            ActorTurn actorTurn = new ActorTurn(turn);
            Node.SetActorTurn(actorTurn);

            Node startNode = new Node(boardState, actorTurn.Opponent);
            NodeMove result = NegaMaxAlgorithm(startNode, depth-1);
            average.Add(nodes);
            Debug.Log($"value: {result.Score}, column: {result.Column}, nodes: {nodes}, mean: {average.Value}");
            
            return result.Column;
        }

        private NodeMove NegaMaxAlgorithm(Node currentNode, int actualDepth)
        {
            ++nodes;
            
            // Suspend
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                
                if (actualDepth != 0)
                {
                    value = actualDepth % 2 == 0 ? value : -value;
                    value *= actualDepth + 1;
                }
                value = depth % 2 == 0 ? -value : value;
                
                return new NodeMove(value, currentNode.ColumnSelected);
            }
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            
            int maxEval = int.MinValue + 1;
            int column = 0;
            
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;
                
                NodeMove currentMove = NegaMaxAlgorithm(possibleMoves[i], actualDepth-1);
                currentMove.InverseScore();
                
                if (maxEval < currentMove.Score)
                {
                    maxEval = currentMove.Score;
                    column = i;
                }
            }
            return new NodeMove(maxEval,column);
        }
    }
}