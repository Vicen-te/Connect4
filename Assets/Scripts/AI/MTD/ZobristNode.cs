using System.Collections.Generic;
using Board;

namespace AI.MTD
{
    public class ZobristNode : Node
    {
        public int HasValue { get; private set; }

        private ZobristKey _zobristKey;
        
        public ZobristNode(BoardState boardState, int turn, ZobristKey zobristKey, int hasValue = default) : base(boardState, turn)
        {
            HasValue = hasValue;
            _zobristKey = new ZobristKey(zobristKey);
        }
        
        // HashMove
        public void GenerateHashValue()
        {
            int zobristKeyInt = _zobristKey.Get(Position, ActorTurn.AI);
            HasValue ^= zobristKeyInt;
        }
        
        // There is no template specialization (c++) in c# :/ 
        public new List<ZobristNode> PossibleMoves()
        {
            List<ZobristNode> list = new List<ZobristNode>();

            for (int i = 0; i < BoardState.Columns; ++i)
            {
                bool empty = BoardState.IsColumnEmpty(i);
                if (!empty)
                {
                    list.Add(default);
                    continue;
                }

                ZobristNode node = GenerateNode(i);
                list.Add(node);
            }
            
            return list;
        }

        private ZobristNode GenerateNode(int column)
        {
            int nextTurn = (Turn + 1) % 2;
            ZobristNode node = new ZobristNode(BoardState, nextTurn, _zobristKey, HasValue);
            node.Position = node.BoardState.FirstDiscInColumn(column);
                
            // Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
            node.BoardState.AddDisc(node.Position, nextTurn);
            node.ColumnSelected = column;
            node.GenerateHashValue();
            return node;
        }
    }
}