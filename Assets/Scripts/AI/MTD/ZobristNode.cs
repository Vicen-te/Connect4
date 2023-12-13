using System.Collections.Generic;
using Board;

namespace AI.MTD
{
    public class ZobristNode : Node
    {
        private int _hasValue;
        public int HasValue => _hasValue;
        private ZobristKey _zobristKey;
        
        public ZobristNode(BoardState boardState, int turn, ZobristKey zobristKey, int hasValue = default) : base(boardState, turn)
        {
            _hasValue = hasValue;
            _zobristKey = new ZobristKey(zobristKey);
        }
        
        // HashMove
        public void GenerateHashValue()
        {
            int zobristKeyInt = _zobristKey.Get(Position, Turn);
            _hasValue ^= zobristKeyInt;
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

                int nextTurn = (Turn + 1) % 2;
                GenerateNode(ref list, nextTurn, i);
            }
            
            return list;
        }

        private void GenerateNode(ref List<ZobristNode> list, int nextTurn, int column)
        {
            ZobristNode node = new ZobristNode(BoardState, nextTurn, _zobristKey, _hasValue);
            node.Position = node.BoardState.FirstDiscInColumn(column);
                
            // Debug.Log($"Disc: {disc}, Column: {_columnSelected}");
            node.BoardState.AddDisc(node.Position, nextTurn);
            node.Column = column;
            node.GenerateHashValue();

            list.Add(node);
        }
    }
}