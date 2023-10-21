using System;
using System.Collections.Generic;
using Interaction;
using UnityEngine;

namespace Board
{
    public sealed class BoardInfo
    {
        private readonly BoardLoader _boardLoader;
        public byte ColumnsInt => _boardLoader.ColumnsInt;
        public byte RowsInt => _boardLoader.RowsInt;
        public ushort Capacity => _boardLoader.Capacity;
        public List<Disc> Discs => _boardLoader.Discs;
        public List<Column> Columns => _boardLoader.Columns;

        public BoardInfo(BoardLoader boardLoader)
        {
            _boardLoader = boardLoader;
        }

        public void ForEachDisc(ushort startColumn, ushort endColumn, Action<Disc> method)
        {
            startColumn *= RowsInt;
            endColumn *= RowsInt;
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort row    = (ushort)(i / RowsInt);
                ushort column = (ushort)(i % RowsInt);
                method(Discs[i]);
                
                Debug.Log($"{row}, {column}");
            }
        }
        
        /// Debug, no use Actually
        public void ForEachColumn(Column column)
        {
            ushort index = (ushort)Columns.FindIndex( element => element == column);
            
            ushort startColumn = (ushort)(RowsInt * index);
            ushort endColumn = (ushort)(RowsInt * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort iRow    = (ushort)(i / RowsInt);
                ushort iColumn = (ushort)(i % RowsInt);
                Debug.Log($"{iRow}, {iColumn}");
            }
        }
        
        
        /// List of unused disks
        public List<Disc> ListOfDiscsInColumn(Column column)
        {
            List<Disc> spacesForColumn = new List<Disc>();
            ushort index = (ushort)Columns.FindIndex( element => element == column);
            
            ushort startColumn = (ushort)(RowsInt * index);
            ushort endColumn = (ushort)(RowsInt * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                spacesForColumn.Add(Discs[i]);
            }
            
            return spacesForColumn;
        }
        
        public Disc FirstDiscInColumn(Column column)
        {
            ushort index = (ushort)Columns.FindIndex( element => element == column);
            ushort startColumn = (ushort)(RowsInt * index);
            ushort endColumn = (ushort)(RowsInt * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                if (!Discs[i].Visibility) return Discs[i];
            }
            
            // Any disc is available 
            return null;
        }
    }
}