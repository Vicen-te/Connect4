using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Board
{
    public readonly struct BoardState
    {
        private readonly byte _columnsInt;
        private readonly byte _rowsInt;
        private readonly List<int> _discs;
        private ushort Capacity => (ushort)_discs.Count;
        public readonly ushort Columns;
        
        public BoardState(BoardInfo boardInfo)
        {
            _columnsInt = boardInfo.ColumnsInt;
            _rowsInt = boardInfo.RowsInt;
            Columns = (ushort)boardInfo.Columns.Count;

            _discs = new List<int>();
            foreach (Disc disc in boardInfo.Discs)
            {
                _discs.Add(disc.ActorOwner);
            }
        }
        
        public BoardState(BoardState boardState)
        {
            _columnsInt = boardState._columnsInt;
            _rowsInt = boardState._rowsInt;
            Columns = boardState.Columns;

            _discs = new List<int>();
            foreach (int disc in boardState._discs)
            {
                _discs.Add(disc);
            }
        }

        public bool IsColumnEmpty(int column)
        {
            ushort endColumn = (ushort)(_rowsInt * (column + 1));
            return _discs[endColumn - 1] == -1;
        }
        
        public int FirstDiscInColumn(int column)
        {
            ushort startColumn = (ushort)(_rowsInt * column);
            ushort endColumn = (ushort)(_rowsInt * (column + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                if (_discs[i] == -1) return i;
            }
            
            // No disc is available 
            return -1;
        }

        // Change the virtual Board we created before
        public void AddDisc(int index, int turn)
        {
            _discs[index] = turn;
        }
        
        private bool CheckConnect4(KeyValuePair<int, int> neighbor1Kvp,
                                  KeyValuePair<int, int> neighbor2Kvp,
                                  KeyValuePair<int, int> neighbor3Kvp,
                                  int turn)
        { 
            for (ushort i = 0; i < Capacity; ++i)
            {
                ushort row    = (ushort)(i % _rowsInt);
                ushort column = (ushort)(i / _rowsInt);
                int currentDisc = _discs[i];
                    
                if (currentDisc != turn) continue;

                // Get neighbor index
                int neighbor1Index = (column + neighbor1Kvp.Key) * _rowsInt +
                                     (row + neighbor1Kvp.Value);
                
                int neighbor2Index = (column + neighbor2Kvp.Key) * _rowsInt +
                                     (row + neighbor2Kvp.Value);
                
                int neighbor3Index = (column + neighbor3Kvp.Key) * _rowsInt +
                                     (row + neighbor3Kvp.Value);
                
                // Don't cross the threshold
                if (neighbor1Index < 0 || 
                    neighbor2Index < 0 || 
                    neighbor3Index < 0 ||
                    neighbor1Index >= Capacity || 
                    neighbor2Index >= Capacity || 
                    neighbor3Index >= Capacity) 
                    continue;
                
                // Neighbor Discs
                int neighbor1 = _discs[neighbor1Index];
                int neighbor2 = _discs[neighbor2Index];
                int neighbor3 = _discs[neighbor3Index];
                
                // Check for same actor owner
                bool actorOwner = neighbor1 == turn &&
                                  neighbor2 == turn  &&
                                  neighbor3 == turn;
                
                if(!actorOwner) continue;
                
                // Check anomalies
                // Example:
                //      Columns: 7, Rows: 6
                //      Discs: 4,5,6,7 (first two last discs of the row: 0 and other last first two of the row: 1)
                bool columns = neighbor1Index / _rowsInt == column + neighbor1Kvp.Key &&
                               neighbor2Index / _rowsInt == column + neighbor2Kvp.Key &&
                               neighbor3Index / _rowsInt == column + neighbor3Kvp.Key;
                
                bool rows = neighbor1Index % _rowsInt == row + neighbor1Kvp.Value &&
                            neighbor2Index % _rowsInt == row + neighbor2Kvp.Value &&
                            neighbor3Index % _rowsInt == row + neighbor3Kvp.Value;
                    
                if(!columns || !rows) continue;
                return true;
            }
            return false;
        }

        public bool Draw() => _discs.All(disc => disc != -1);
         
        public bool Winner(int turn)
        {
            // Horizontal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(0, 1),
                    new KeyValuePair<int, int>(0, 2),
                    new KeyValuePair<int, int>(0, 3),
                    turn
                )
               )
                return true;

            // Vertical Check 
            if (CheckConnect4
                (   new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(2, 0),
                    new KeyValuePair<int, int>(3, 0),
                    turn
                )
               )
                return true;
            
            // Ascending Diagonal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, 1),
                    new KeyValuePair<int, int>(2, 2),
                    new KeyValuePair<int, int>(3, 3),
                    turn
                )
               )
                return true;
            
            // Descending Diagonal Check
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, -1),
                    new KeyValuePair<int, int>(2, -2),
                    new KeyValuePair<int, int>(3, -3),
                    turn
                )
               )
                return true;
            
            return false;
        }
        
        public void PrintDiscs()
        {
            string board = "";
            for (int i = _rowsInt-1; i > -1; --i)
            {
                string row = "";
                int position = i;
                
                for (int j = 0; j < _columnsInt; ++j)
                {
                    row += $"{_discs[position]}\t";
                    position += _rowsInt;
                }
                board += $"{row}\n";
            }
            Debug.Log(board);
        }
    }
}