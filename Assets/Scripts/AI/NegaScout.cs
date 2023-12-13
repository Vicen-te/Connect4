using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace AI
{
    public class NegaScout : MonoBehaviour, IScript
    {
        private NegaMax _negaMax;
        public int depth = 6;
    
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
            
            NodeMove result = NegaScoutAlgorithm(startNode, depth-1, alpha, beta);
            Debug.Log($"Final:\n value: {-result.Score}, column: {result.Column}");
            
            return result.Column;
        }

        private NodeMove NegaScoutAlgorithm(Node currentNode, int actualDepth, int alpha, int beta)
        {
            // Suspend, If we are done recursing.
            if (currentNode.IsEndOfGame() || actualDepth == 0)
            {
                return _negaMax.Evaluation(currentNode, actualDepth);
            }

            int bestScore = int.MinValue+1;
            int bestColumn = 0;
            int adaptiveBeta = beta; //< keep track of the Test window value
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            
             // Go through each move
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;
                
                // Current move evaluation
                NodeMove currentMove = _negaMax.Algorithm(possibleMoves[i], actualDepth-1, -adaptiveBeta, -alpha);
                currentMove.InverseScore();
            
                // Update the best score.
                if (currentMove.Score > bestScore)
                {
                   // if we are in "narrow-mode" then widen and do a regular AB negamax search.
                   if (adaptiveBeta == beta || actualDepth <= 2)
                   {
                       bestScore = currentMove.Score;
                       bestColumn = i;
                   }
                   else //< otherwise we can do a Test, search with a null window
                   {
                       NodeMove negativeBestScore = NegaScoutAlgorithm(possibleMoves[i], actualDepth, -beta, -currentMove.Score);
                       //negativeBestScore.InverseScore();  // the return value is already inversed.
                       
                       bestScore = negativeBestScore.Score; 
                       bestColumn = i;
                   }

                   alpha = Math.Max(alpha, bestScore);
                   
                   Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                             $"bestColumn: {bestColumn}, bestScore: {bestScore} i: {i}");
                   
                   // if we're outside the bounds, prune by exiting
                   if (beta <= alpha) 
                       return new NodeMove(bestScore, bestColumn);

                   // otherwise update the window location
                   adaptiveBeta = alpha + 1;
                }
            }
            return new NodeMove(bestScore, bestColumn);
        }
    }
}
