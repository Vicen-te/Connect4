using System;
using System.Collections.Generic;
using Board;
using Core.Actor;
using UnityEngine;

namespace AI.Algorithms
{
    public class NegaScout : MonoBehaviour, IScript
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
            int alpha = int.MinValue + 1, beta = int.MaxValue;
            
            NodeMove result = NegaScoutAlgorithm(startNode, depth-1, alpha, beta);
            average.Add(nodes);
            Debug.Log($"value: {-result.Score}, column: {result.Column}, nodes: {nodes}, mean: {average.Value}");
            
            return result.Column;
        }

        private NodeMove NegaScoutAlgorithm(Node currentNode, int actualDepth, int alpha, int beta)
        {
            ++nodes;
            // Suspend, If we are done recursing.
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

            int bestScore = alpha;
            int bestColumn = -1;
            int adaptiveBeta = beta; //< keep track of the Test window value
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            
             // Go through each move
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;
                
                // search with a null window
                NodeMove currentMove = NegaScoutAlgorithm(possibleMoves[i], actualDepth-1, -adaptiveBeta, -alpha);
                currentMove.InverseScore();
                
                if (currentMove.Score > bestScore && currentMove.Score < beta && i > 0 && actualDepth < 1)
                {
                    // re-search
                    currentMove = NegaScoutAlgorithm(possibleMoves[i], actualDepth, -beta, -currentMove.Score);
                    currentMove.InverseScore();
                }
                
                // Update the best score.
                if (currentMove.Score > bestScore)
                {
                    bestScore = currentMove.Score;
                    bestColumn = i;
                }
                
                alpha = Math.Max(alpha, bestScore);
               
                // Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                //           $"bestColumn: {bestColumn}, bestScore: {bestScore} i: {i}");
               
                // if we're outside the bounds, prune by exiting
                if (alpha >= beta) 
                    return new NodeMove(bestScore, bestColumn);

                // otherwise update the window location
                adaptiveBeta = alpha + 1;
            }
            return new NodeMove(bestScore, bestColumn);
        }
    }
}
