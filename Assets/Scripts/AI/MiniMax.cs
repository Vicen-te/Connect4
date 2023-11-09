using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace AI
{
    public class MiniMax : MonoBehaviour, IScript
    {
        public int depth = 6;
        
        // Max == AI
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
            NodeValue result = MiniMaxAlgorithm(startNode, depth-1, alpha, beta, Actor.AI);
            
            Debug.Log($"Final:\n value: {result.Value}, column: {result.Column}");
            return result.Column;
        }

        private NodeValue MiniMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta, Actor actor)
        {
            // Debug.Log($"Depth: {actualDepth}");

            // Suspend
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                if (actualDepth != 0)
                {
                    value *= actualDepth+1;
                }
                
                // Debug.Log($"Value: {value}");
                return new NodeValue(value, currentNode.ColumnSelected);
            }
            
            // Compare
            List<Node> possibleMoves = currentNode.PossibleMoves();
            // Debug.Log($"nodes: {possibleMoves.Count}");
            
            int column = 0;

            if (actor == Actor.Player)
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    // Debug.Log($"node: {i}");
                    if(possibleMoves[i] == null) continue;
                    
                    NodeValue eval = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Actor.AI);
                    if (minEval > eval.Value)
                    {
                        minEval = eval.Value;
                        column = i;
                    }
                    beta = Math.Min(beta, eval.Value);
                    
                    // Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                    //           $"column: {column}, minEval: {minEval} i: {i}");
                    if (beta <= alpha) 
                        return new NodeValue(minEval,column);
                }
                return new NodeValue(minEval,column);
            }
            else
            {
                int maxEval = int.MinValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    // Debug.Log($"node: {i}");
                    if(possibleMoves[i] == null) continue;
                    
                    NodeValue eval = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Actor.Player);
                    if (maxEval < eval.Value)
                    {
                        maxEval = eval.Value;
                        column = i;
                    }
                    alpha = Math.Max(alpha, eval.Value);
                    
                    Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                              $"column: {column}, maxEval: {maxEval} i: {i}");
                    if (beta <= alpha) 
                        return new NodeValue(maxEval,column);
                }
                return new NodeValue(maxEval,column);
            }
        }
    }
}