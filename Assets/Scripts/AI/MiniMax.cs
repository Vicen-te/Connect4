using System;
using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;

namespace AI
{
    public enum Player { Min, Max }
    public class MiniMax : MonoBehaviour, IScript
    {
        public int depth = 5;
        
        // Max == AI
        private Player _player;
        private struct MiniMaxValue
        {
            public int Value;
            public int Column;

            public MiniMaxValue(int value, int column)
            {
                Value = value;
                Column = column;
            }
        }
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            /*
             *  MiniMax algorithm
             *  Expand 7 positions
             *      Example:
             *                  new
             *          new new act new new new new
             */
            
            Node startNode = new Node(boardState, (int)Player.Min);

            int alpha = int.MinValue, beta = int.MaxValue;
            MiniMaxValue result = MiniMaxAlgorithm(startNode, depth-1, alpha, beta, Player.Max);
            Debug.Log($"Final:\n value: {result.Value}, column: {result.Column}");
            return result.Column;
        }

        private MiniMaxValue MiniMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta, Player player)
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
                return new MiniMaxValue(value, currentNode.ColumnSelected);
            }
            
            // Compare
            List<Node> possibleMoves = currentNode.PossibleMoves();
            // Debug.Log($"nodes: {possibleMoves.Count}");
            
            int column = 0;

            if (player == Player.Min)
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    // Debug.Log($"node: {i}");
                    if(possibleMoves[i] == null) continue;
                    
                    MiniMaxValue eval = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Player.Max);
                    if (minEval > eval.Value)
                    {
                        minEval = eval.Value;
                        column = i;
                    }
                    beta = Math.Min(beta, eval.Value);
                    
                    // Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                    //           $"column: {column}, minEval: {minEval} i: {i}");
                    if (beta <= alpha) 
                        return new MiniMaxValue(minEval,column);
                }
                return new MiniMaxValue(minEval,column);
            }
            else
            {
                int maxEval = int.MinValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    // Debug.Log($"node: {i}");
                    if(possibleMoves[i] == null) continue;
                    
                    MiniMaxValue eval = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, Player.Min);
                    if (maxEval < eval.Value)
                    {
                        maxEval = eval.Value;
                        column = i;
                    }
                    alpha = Math.Max(alpha, eval.Value);
                    
                    Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                              $"column: {column}, maxEval: {maxEval} i: {i}");
                    if (beta <= alpha) 
                        return new MiniMaxValue(maxEval,column);
                }
                return new MiniMaxValue(maxEval,column);
            }
        }
    }
}