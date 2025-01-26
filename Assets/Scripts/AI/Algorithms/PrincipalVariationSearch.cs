using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Core.Actor;
using UnityEngine;

namespace AI.Algorithms
{
    public class PrincipalVariationSearch  : MonoBehaviour, IScript
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
            
            NodeMove result = PVSAlgorithm(startNode, depth-1, alpha, beta);
            average.Add(nodes);
            Debug.Log($"value: {-result.Score}, column: {result.Column}, nodes: {nodes}, mean: {average.Value}");
            
            return result.Column;
        }

        private NodeMove PVSAlgorithm(Node currentNode, int actualDepth, int alpha, int beta)
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

            int bestScore = int.MinValue + 1;
            int bestColumn = -1;
            
            List<Node> possibleMoves = currentNode.PossibleMoves();
            
             // Go through each move
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                if (possibleMoves[i] == null) continue;

                NodeMove currentMove;
                if (possibleMoves[i] == possibleMoves.First())
                {
                    // search with a normal window
                    currentMove = PVSAlgorithm(possibleMoves[i], actualDepth-1, -beta , -alpha);
                    currentMove.InverseScore();
                }
                else
                {
                    // search with a null window
                    currentMove = PVSAlgorithm(possibleMoves[i], actualDepth-1, -alpha-1 , -alpha);
                    currentMove.InverseScore();

                    // if it failed high
                    if (currentMove.Score > alpha && currentMove.Score < beta) 
                    {
                        // do a full re-search
                        currentMove = PVSAlgorithm(possibleMoves[i], actualDepth-1, -beta , -currentMove.Score); 
                        currentMove.InverseScore();
                    }
                }
                
                if (currentMove.Score > bestScore)
                {
                    bestScore = currentMove.Score;
                    bestColumn = i;
                }
                
                alpha = Math.Max(alpha, bestScore);
                
                // if we're outside the bounds, prune by exiting
                if (beta <= alpha) 
                    break;
            }
            return new NodeMove(bestScore, bestColumn); // fail-hard
        }
    }
}
