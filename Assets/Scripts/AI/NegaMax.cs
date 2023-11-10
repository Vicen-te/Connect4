using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace AI
{
    public class NegaMax : MonoBehaviour, IScript
    {
        public int depth = 6;
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            Node startNode = new Node(boardState, (int)Actor.Player);
            int alpha = int.MinValue+1, beta = int.MaxValue;
            
            NodeMove result = NegaMaxAlgorithm(startNode, depth-1, alpha, beta);
            Debug.Log($"Final:\n value: {-result.Score}, column: {result.Column}");
            
            return result.Column;
        }

        private NodeMove NegaMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta)
        {
            // Suspend
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                
                if (actualDepth != 0)
                {
                    value = actualDepth % 2 == 0 ? value : -value;
                    value *= actualDepth+1;
                }
                value = depth % 2 == 0 ? -value : value;
                
                return new NodeMove(value, currentNode.ColumnSelected);
            }
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            
            int maxEval = int.MinValue+1;
            int column = 0;
            
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;
                
                NodeMove currentMove = NegaMaxAlgorithm(possibleMoves[i], actualDepth-1, -beta, -alpha);
                currentMove.InverseScore();
                
                if (maxEval < currentMove.Score)
                {
                    maxEval = currentMove.Score;
                    column = i;
                }
                alpha = Math.Max(alpha, currentMove.Score);

                if (beta <= alpha) 
                    return new NodeMove(maxEval,column);
            }
            return new NodeMove(maxEval,column);
        }
        
        public NodeMove Algorithm(Node currentNode, int actualDepth, int alpha, int beta) =>
            NegaMaxAlgorithm(currentNode, actualDepth, alpha, beta);

        public NodeMove Evaluation(Node currentNode, int actualDepth)
        {
            int value = currentNode.Evaluate();
                
            if (actualDepth != 0)
            {
                value = actualDepth % 2 == 0 ? value : -value;
                value *= actualDepth+1;
            }
            value = depth % 2 == 0 ? -value : value;
                
            return new NodeMove(value, currentNode.ColumnSelected);
        }
    }
}