using System.Collections.Generic;
using AI.MTD;
using Board;

namespace AI
{
    public class Node
    {
        protected readonly BoardState BoardState;
        protected readonly int Turn;
        
        protected int Column;
        public int ColumnSelected => Column;
        protected int Position;
        

        public Node(BoardState boardState, int turn)
        {
            BoardState = new BoardState(boardState);
            Turn = turn;
        }
        
        public bool IsEndOfGame()
        {
            // if there is a winner or we can't add more discs
            bool draw = BoardState.Draw();
            bool winner = BoardState.Winner((int)Actor.Player) || BoardState.Winner((int)Actor.AI);
            return draw || winner;
        }

        public int Evaluate()
        {
            int player = BoardState.Evaluate((int)Actor.Player);
            int ai = BoardState.Evaluate((int)Actor.AI);
            
            const int multiplier = 100;
            if(ai > player) return ai * multiplier;
            return -player * multiplier;
        }

        public List<Node> PossibleMoves()
        {
            List<Node> list = new List<Node>();

            for (int i = 0; i < BoardState.Columns; ++i)
            {
                bool empty = BoardState.IsColumnEmpty(i);
                if (!empty)
                {
                    list.Add(default);
                    continue;
                }

                int nextTurn = (Turn + 1) % 2;
                GenerateNode(ref list, nextTurn, i);
            }
            
            return list;
        }

        private void GenerateNode(ref List<Node> list, int nextTurn, int column)
        {
            Node node = new Node(BoardState, nextTurn);
            node.Position = node.BoardState.FirstDiscInColumn(column);
                
            // Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
            node.BoardState.AddDisc(node.Position, nextTurn);
            node.Column = column;
            list.Add(node);
        }
    }
}