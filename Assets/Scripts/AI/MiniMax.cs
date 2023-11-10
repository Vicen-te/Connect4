using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace AI
{
    public class MiniMax : MonoBehaviour, IScript
    {
        public int depth = 6;
        
        // AI == 1
        private Actor _actor;
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            /*
             *  MiniMax algorithm
             *  Expand 7 positions
             *      Example:
             *                  new
             *          new new act new new new new
             */
            
            Node startNode = new Node(boardState, (int)Actor.Player);
            int alpha = int.MinValue, beta = int.MaxValue;
            
            NodeMove result = MiniMaxAlgorithm(startNode, depth-1, alpha, beta, Actor.AI);
            Debug.Log($"Final:\n value: {result.Score}, column: {result.Column}");
            
            return result.Column;
        }

        private NodeMove MiniMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta, Actor actor)
        {
            // Suspend
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                if (actualDepth != 0)
                {
                    value *= actualDepth+1;
                }
                
                return new NodeMove(value, currentNode.ColumnSelected);
            }
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            int column = 0;

            if (actor == Actor.Player)
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    if(possibleMoves[i] == null) continue;
                    
                    NodeMove currentMove = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Actor.AI);
                    if (minEval > currentMove.Score)
                    {
                        minEval = currentMove.Score;
                        column = i;
                    }
                    beta = Math.Min(beta, currentMove.Score);
                    
                    if (beta <= alpha) 
                        return new NodeMove(minEval,column);
                }
                return new NodeMove(minEval,column);
            }
            else
            {
                int maxEval = int.MinValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    if(possibleMoves[i] == null) continue;
                    
                    NodeMove currentMove = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Actor.Player);
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
        }
    }
}