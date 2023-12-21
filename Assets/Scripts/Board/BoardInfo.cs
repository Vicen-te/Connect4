using System.Collections.Generic;
using Interaction;

namespace Board
{
    // Class with references of BoardLoader
    // Getters Only
    public sealed class BoardInfo
    {
        public readonly byte ColumnsInt;
        public readonly byte RowsInt;
        public readonly ushort Capacity;
        public readonly List<Disc> Discs;
        public readonly List<Column> Columns;

        // Constructor
        public BoardInfo(BoardLoader boardLoader)
        {
            ColumnsInt = boardLoader.ColumnsInt;
            RowsInt = boardLoader.RowsInt;
            Capacity = boardLoader.Capacity;
            Discs = boardLoader.Discs;
            Columns = boardLoader.Columns;
        }
        
        // Methods
        public int ColumnIndex(Column column) => Columns.FindIndex( element => element == column);
        public Disc GetDisc(int index) => Discs[index];
    }
}