using System.Collections.Generic;
using Board;
using Core.Actor;

namespace AI
{
    public class Node
    {
        protected readonly BoardState BoardState;
        protected static ActorTurn ActorTurn;
        private static int Opponent => ActorTurn.Opponent;
        private static int AI => ActorTurn.AI;
        protected readonly int Turn;
        public int ColumnSelected { get; protected set; }
        protected int Position;

        public static void SetActorTurn(ActorTurn actorTurn)
        {
            ActorTurn = actorTurn;
        }

        public Node(BoardState boardState, int turn)
        {
            BoardState = new BoardState(boardState);
            Turn = turn;
        }
        
        public bool IsEndOfGame()
        {
            // if there is a winner or we can't add more discs
            bool draw = BoardState.Draw();
            bool winner = BoardState.Winner(Opponent) || BoardState.Winner(AI);
            return draw || winner;
        }

        public int Evaluate()
        {
            int player = BoardState.Evaluate(Opponent);
            int ai = BoardState.Evaluate(AI);
            
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
                Node node = GenerateNode(i);
                list.Add(node);
            }
            
            return list;
        }

        private Node GenerateNode(int column)
        {
            int nextTurn = (Turn + 1) % 2;
            Node node = new Node(BoardState, nextTurn);
            node.Position = node.BoardState.FirstDiscInColumn(column);
            
            // Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            node.BoardState.AddDisc(node.Position, nextTurn);
            node.ColumnSelected = column;
            return node;
        }
    }
}