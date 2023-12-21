using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Board
{
    public struct BoardState
    {
        private readonly byte _columnsInt;
        private readonly byte _rowsInt;
        private readonly List<int> _discs;
        private const int MaxConnections = 6;
        private const int DiscEmpty = -1;
        
        public readonly ushort Columns;
        public readonly ushort Capacity => (ushort)_discs.Count;
        public ushort OpponentDisc {get; private set;}
        public ushort AIDisc {get; private set;}
        public ushort DiscInUse {get; private set;}
        
        // First State Constructor
        public BoardState(BoardInfo boardInfo)
        {
            _columnsInt = boardInfo.ColumnsInt;
            _rowsInt = boardInfo.RowsInt;
            Columns = (ushort)boardInfo.Columns.Count;

            _discs = new List<int>();
            for(int i = 0; i < boardInfo.Capacity; i++)
            {
                _discs.Add(DiscEmpty);
            }

            DiscInUse = 0;
            OpponentDisc = 0;
            AIDisc = 0;
        }
        
        public BoardState(BoardState boardState)
        {
            _columnsInt = boardState._columnsInt;
            _rowsInt = boardState._rowsInt;
            Columns = boardState.Columns;
            
            // Copy 
            _discs = new List<int>();
            foreach (int disc in boardState._discs)
            {
                _discs.Add(disc);
            }
            
            DiscInUse = boardState.DiscInUse;
            OpponentDisc = boardState.OpponentDisc;
            AIDisc = boardState.AIDisc;
        }

        public readonly bool IsColumnEmpty(int column)
        {
            ushort endColumn = (ushort)(_rowsInt * (column + 1));
            return _discs[endColumn - 1] == -1;
        }
        
        public readonly int FirstDiscInColumn(int column)
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
            ++DiscInUse;
            if (turn == 0) ++OpponentDisc;
            else           ++AIDisc;
        }

        private readonly int EvaluateNeighbor(int turn, int neighborIndex)
        {
            // adversary disc
            int neighborValue = -2;
            
            // your disc
            if (_discs[neighborIndex] == turn)
                neighborValue = 2;
            
            // empty = -1 by default
            else if (_discs[neighborIndex] == DiscEmpty)
                neighborValue = 1;

            return neighborValue;
        }
        
        private readonly int CheckConnect4(KeyValuePair<int, int> neighbor1Kvp,
                                  KeyValuePair<int, int> neighbor2Kvp,
                                  KeyValuePair<int, int> neighbor3Kvp,
                                  int turn)
        {
            int maxNeighborValue = int.MinValue;
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
                
                // Neighbor Discs values
                int neighbor1Value = EvaluateNeighbor(turn, neighbor1Index);
                int neighbor2Value = EvaluateNeighbor(turn, neighbor2Index);
                int neighbor3Value = EvaluateNeighbor(turn, neighbor3Index);
                int neighborsValue = neighbor1Value + neighbor2Value + neighbor3Value;
                
                if(neighborsValue == MaxConnections) return MaxConnections;
                if(neighborsValue > maxNeighborValue) maxNeighborValue = neighborsValue;
            }
            return maxNeighborValue;
        }

        public readonly bool Draw() => _discs.All(disc => disc != -1);
         
        public readonly int Evaluate(int turn)
        {
            int[] connects = new int[4];
            
            // Horizontal Check 
            connects[0] = CheckConnect4
            (
                new KeyValuePair<int, int>(0, 1),
                new KeyValuePair<int, int>(0, 2),
                new KeyValuePair<int, int>(0, 3),
                turn
            );
            if (connects[0] == MaxConnections) return MaxConnections;

            // Vertical Check 
            connects[1] = CheckConnect4
            (   new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 0),
                new KeyValuePair<int, int>(3, 0),
                turn
            );
            if (connects[1] == MaxConnections) return MaxConnections;

            
            // Ascending Diagonal Check 
            connects[2] = CheckConnect4
            (
                new KeyValuePair<int, int>(1, 1),
                new KeyValuePair<int, int>(2, 2),
                new KeyValuePair<int, int>(3, 3),
                turn
            );
            if (connects[2] == MaxConnections) return MaxConnections;
            
            // Descending Diagonal Check
            connects[3] = CheckConnect4
            (
                new KeyValuePair<int, int>(1, -1),
                new KeyValuePair<int, int>(2, -2),
                new KeyValuePair<int, int>(3, -3),
                turn
            );
            if (connects[3] == MaxConnections) return MaxConnections;

            int maxConnect = 0;
            foreach (int connect in connects)
            {
                if (connect > maxConnect) maxConnect = connect;
            }
            
            return maxConnect;
        }
        
        public readonly bool Winner(int turn)
        {
            // Horizontal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(0, 1),
                    new KeyValuePair<int, int>(0, 2),
                    new KeyValuePair<int, int>(0, 3),
                    turn
                ) == MaxConnections
               )
                return true;

            // Vertical Check 
            if (CheckConnect4
                (   new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(2, 0),
                    new KeyValuePair<int, int>(3, 0),
                    turn
                ) == MaxConnections
               )
                return true;
            
            // Ascending Diagonal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, 1),
                    new KeyValuePair<int, int>(2, 2),
                    new KeyValuePair<int, int>(3, 3),
                    turn
                ) == MaxConnections
               )
                return true;
            
            // Descending Diagonal Check
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, -1),
                    new KeyValuePair<int, int>(2, -2),
                    new KeyValuePair<int, int>(3, -3),
                    turn
                ) == MaxConnections
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
        
        public void PrintResult(string condition)
        {
            Debug.Log($"{condition}\nDiscs: {DiscInUse}/{Capacity} - Opponent: {OpponentDisc} - AI: {AIDisc}");

        }
    }
}