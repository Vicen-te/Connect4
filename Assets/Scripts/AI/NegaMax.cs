using System;
using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;

namespace AI
{
    public class NegaMax : MonoBehaviour, IScript
    {
        public int depth = 5;
        
        // Max == AI
        private Player _player;
        private struct NegaMaxValue
        {
            public int Value;
            public int Column;

            public NegaMaxValue(int value, int column)
            {
                Value = value;
                Column = column;
            }
        }
        
        public int ExecuteAlgorithm(BoardState boardState)
        {
            Node startNode = new Node(boardState, (int)Player.Min);

            int alpha = int.MinValue+1, beta = int.MaxValue;
            NegaMaxValue result = NegaMaxAlgorithm(startNode, depth-1, alpha, beta);
            Debug.Log($"Final:\n value: {-result.Value}, column: {result.Column}");
            return result.Column;
        }

        private NegaMaxValue NegaMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta)
        {
            //Debug.Log($"Depth: {actualDepth}");
            
            // Suspend
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                
                if (actualDepth != 0)
                {
                    Debug.Log($"Depth: 4, actualDepth: {actualDepth}");
                    // if (currentNode.WinningTurn == 1) // Ai
                    //     value = (actualDepth+1) % 2 == 0 ? value : -value;
                    value = actualDepth % 2 == 0 ? value : -value;
                    value *= actualDepth+1;
                }
                
                value = depth % 2 == 0 ? -value : value;
                
                // Debug.Log($"Value: {value}");
                return new NegaMaxValue(value, currentNode.ColumnSelected);
            }
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            // Debug.Log($"nodes: {possibleMoves.Count}");
            
            int maxEval = int.MinValue+1;
            int column = 0;
            
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null)
                {
                    //Debug.Log($"node {i}: null");
                    continue;
                }
                
                NegaMaxValue eval = NegaMaxAlgorithm(possibleMoves[i], actualDepth-1, -beta, -alpha);
                int inverseValue = -eval.Value;
                //Debug.Log($"node {i}: {inverseValue}, {eval.Value}");
                
                if (maxEval < inverseValue)
                {
                    maxEval = inverseValue;
                    column = i;
                }
                alpha = Math.Max(alpha, inverseValue);

                Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                          $"column: {column}, maxEval: {maxEval} i: {i}");
                    
                if (beta <= alpha) 
                    return new NegaMaxValue(maxEval,column);
            }
            return new NegaMaxValue(maxEval,column);
        }
    }
}