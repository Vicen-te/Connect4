using System.Collections.Generic;
using Board;

namespace AI
{
    public class Node
    {
        private BoardState _boardState;
        private int _columnSelected;
        private readonly int _turn;
        public int WinningTurn { get; private set; } = -1;

        public Node(BoardState boardState, int turn)
        {
            _boardState = new BoardState(boardState);
            _turn = turn;
        }

        public bool IsEndOfGame()
        {
            // if there is a winner or we can't add more discs
            bool draw = _boardState.Draw();
            bool winner = _boardState.Winner((int)Player.Min) || _boardState.Winner((int)Player.Max);
            return draw || winner;
        }

        public int Evaluate()
        {
            bool player = _boardState.Winner((int)Player.Min);
            bool ai = _boardState.Winner((int)Player.Max);
            
            // Debug.Log($"{player}, {ai}");
            if(ai) return 1000;
            if(player) return -1000;
            return UnityEngine.Random.Range(-100,100);
        }

        public int ColumnSelected => _columnSelected;

        public List<Node> PossibleMoves()
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
                
                // Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
                node._boardState.AddDisc(disc, nextTurn);
                node._columnSelected = i;
                list.Add(node);
            }
            
            return list;
        }
    }
}