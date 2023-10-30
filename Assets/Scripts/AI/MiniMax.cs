using System;
using System.Collections.Generic;
using System.Linq;
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
            public int Alpha;
            public int Beta;

            public MiniMaxValue(int value, int column, int alpha, int beta)
            {
                Value = value;
                Column = column;
                Alpha = alpha;
                Beta = beta;
            }
        }
        
        public Column ExecuteAlgorithm(BoardInfo boardInfo)
        {
            /*
             *  MiniMax algorithm
             *  Expand 7 positions
             *      Example:
             *                  new
             *          new new act new new new new
             */
            
            Node startNode = new Node(boardInfo, (int)Player.Min);

            int alpha = int.MinValue, beta = int.MaxValue;
            MiniMaxValue result = MiniMaxAlgorithm(startNode, depth-1, alpha, beta, Player.Max);
            Debug.Log($"Final:\n value: {result.Value}, column: {result.Column}");
            return boardInfo.Columns[result.Column];
        }

        private MiniMaxValue MiniMaxAlgorithm(Node currentNode, int actualDepth, int alpha, int beta, Player player)
        {
            Debug.Log($"Depth: {actualDepth}");

            // Suspend
            if (currentNode.Draw() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                int column = currentNode.ColumnSelected;
                
                Debug.Log($"Value: {value}");
                return new MiniMaxValue(value, column, alpha, beta);
            }
            
            List<Node> nodes = currentNode.CreateNodes();
            Debug.Log($"nodes: {nodes.Count}");

            if (player == Player.Min)
            {
                int minEval = int.MaxValue;
                int column = 0;
                for (int i = 0; i < nodes.Count; ++i)
                {
                    Debug.Log($"node: {nodes[i]}");
                    if(nodes[i] == null) continue;
                    MiniMaxValue eval = MiniMaxAlgorithm(nodes[i], actualDepth-1, alpha, beta, Player.Max);
                    if (minEval > eval.Value)
                    {
                        minEval = eval.Value;
                        column = i;
                    }
                    beta = Math.Min(beta, eval.Value);
                    
                    Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, i: {i}");
                    if (alpha >= beta) 
                        return new MiniMaxValue(minEval,column,beta,alpha);
                }
                return new MiniMaxValue(minEval,column,beta,alpha);
            }
            else
            {
                int maxEval = int.MinValue;
                int column = 0;
                for (int i = 0; i < nodes.Count; ++i)
                {
                    Debug.Log($"node: {nodes[i]}");
                    if(nodes[i] == null) continue;
                    MiniMaxValue eval = MiniMaxAlgorithm(nodes[i], actualDepth-1, alpha, beta, Player.Min);
                    if (maxEval < eval.Value)
                    {
                        maxEval = eval.Value;
                        column = i;
                    }
                    alpha = Math.Max(alpha, eval.Value);
                    
                    Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}, " +
                                     $"column: {column}, maxEval: {maxEval} i: {i}");
                    if (alpha >= beta) 
                        return new MiniMaxValue(maxEval,column,beta,alpha);
                }
                return new MiniMaxValue(maxEval,column,beta,alpha);
            }
        }
        
        private (int, int) MiniValue(Node currentNode, int actualDepth, int alpha, int beta)
        {
            Debug.Log($"Depth: {actualDepth}");

            // Suspend
            if (currentNode.Draw() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                Debug.Log($"Value: {value}");
                return (value, beta);
            }
            
            List<Node> nodes = currentNode.CreateNodes();
            
            int nodeAlpha = alpha;
            int nodeBeta = beta;
                
            for (int i = 0; i < nodes.Count; ++i)
            {
                if(nodes[i] == null) continue;
                var (newBeta, newAlpha) = MaxValue(nodes[i], actualDepth-1, nodeAlpha, nodeBeta);
                beta = Math.Min(beta, newBeta);
                alpha = newAlpha;
                Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}");
                if (alpha >= beta) 
                    return (beta, alpha);
            }
            return (beta, alpha);
        }
        
        private (int, int) MaxValue(Node currentNode, int actualDepth, int alpha, int beta)
        {
            Debug.Log($"Depth: {actualDepth}");

            // Suspend
            if (currentNode.Draw() || actualDepth == 0)
            {
                int value = currentNode.Evaluate();
                Debug.Log($"Value: {value}");
                return (alpha, value);
            }
            
            List<Node> nodes = currentNode.CreateNodes();

            int nodeAlpha = alpha;
            int nodeBeta = beta;
                
            for (int i = 0; i < nodes.Count; ++i)
            {
                if(nodes[i] == null) continue;

                var (newBeta, newAlpha) = MiniValue(nodes[i], actualDepth-1, nodeAlpha, nodeBeta);
                alpha = Math.Max(alpha, newAlpha);
                beta = newBeta;
                Debug.Log($"Depth: {actualDepth}, alpha: {alpha}, beta: {beta}");
                if (alpha >= beta) 
                    return (beta, alpha);
            }
            return (beta, alpha);
        }
    }
    
    public class Node
    {
        private BoardState _boardState;
        private int _columnSelected;
        private readonly int _turn;
        
        public Node(BoardInfo boardInfo, int turn)
        {
            _boardState = new BoardState(boardInfo);
            _turn = turn;
        }
        
        public Node(BoardState boardState, int turn)
        {
            _boardState = new BoardState(boardState);
            _turn = turn;
        }

        public bool Draw() => _boardState.Draw();

        public int Evaluate()
        {
            bool player = _boardState.Winner((int)Player.Min);
            bool ai = _boardState.Winner((int)Player.Max);
            
            Debug.Log($"{player}, {ai}");
            if(player) return -1000;
            if(ai) return 1000;
            return UnityEngine.Random.Range(-100,100);
        }

        public int ColumnSelected => _columnSelected;

        public List<Node> CreateNodes()
        {
            List<Node> list = new List<Node>();

            for (int i = 0; i < _boardState.Columns; ++i)
            {
                bool empty = _boardState.IsColumnEmpty(i);
                if (!empty)
                {
                    list.Add(null);
                    continue;
                }

                int nextTurn = (_turn + 1) % 2;
                
                Node node = new Node(_boardState, nextTurn);
                int disc = node._boardState.FirstDiscInColumn(i);
                
                Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
                node._boardState.AddDisc(disc, nextTurn);
                node._columnSelected = i;
                list.Add(node);
            }
            
            return list;
        }
    }
}