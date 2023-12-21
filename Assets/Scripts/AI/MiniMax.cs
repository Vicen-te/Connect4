using System;
using System.Collections.Generic;
using System.Diagnostics;
using Board;
using Core.Actor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AI
{
    public class MiniMax : MonoBehaviour, IScript
    {
        public int depth = 6;
        private ActorTurn _actorTurn;
        private int nodes;
        private readonly Average average = new();
        
        public int ExecuteAlgorithm(BoardState boardState, int turn)
        {
            /*
             *  MiniMax algorithm
             *  Expand 7 positions
             *      Example:
             *                  new
             *          new new act new new new new
             */
            nodes = 0;
            _actorTurn = new ActorTurn(turn);
            Node.SetActorTurn(_actorTurn);

            Node startNode = new Node(boardState, _actorTurn.Opponent);
            int alpha = int.MinValue, beta = int.MaxValue;
            
            NodeMove result = MiniMaxAlgorithm(startNode, depth-1, alpha, beta, _actorTurn.AI);
            average.Add(nodes);
            Debug.Log($"value: {result.Score}, column: {result.Column}, nodes: {nodes}, media: {average.Value}");
            
            return result.Column;
        }

        private NodeMove MiniMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta, int turn)
        {
            ++nodes;
            
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

            if (turn == _actorTurn.Opponent)
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    if(possibleMoves[i] == null) continue;
                    
                    NodeMove currentMove = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, _actorTurn.AI);
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
                    
                    NodeMove currentMove = MiniMaxAlgorithm(possibleMoves[i], actualDepth-1, alpha, beta, _actorTurn.Opponent);
                    if (maxEval < currentMove.Score)
                    {
                        maxEval = currentMove.Score;
                        column = i;
                    }
                    alpha = Math.Max(alpha, currentMove.Score);
                    // Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                    //           $"column: {column}, maxEval: {maxEval} i: {i}");
                    
                    if (beta <= alpha) 
                        return new NodeMove(maxEval,column);
                }
                return new NodeMove(maxEval,column);
            }
        }
    }
}
