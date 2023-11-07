using System.Collections.Generic;
using Interaction;

namespace Board
{
    // Class with references of BoardLoader
    // Getters Only
    public sealed class BoardInfo
    {
        public byte ColumnsInt { get; private set; }
        public byte RowsInt { get; private set; }
        public ushort Capacity { get; private set; }
        public List<Disc> Discs { get; private set; }
        public List<Column> Columns { get; private set; }

        public BoardInfo(BoardLoader boardLoader)
        {
            ColumnsInt = boardLoader.ColumnsInt;
            RowsInt = boardLoader.RowsInt;
            Capacity = boardLoader.Capacity;
            Discs = boardLoader.Discs;
            Columns = boardLoader.Columns;
        }

        public int ColumnIndex(Column column)
        {
            return Columns.FindIndex( element => element == column);
        }

        public Disc GetDisc(int index)
        {
            return Discs[index];
        }
    }
}