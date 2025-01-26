using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents the state of the game board, including the discs placed, the number of rows and columns, and the players' discs.
    /// </summary>
    public struct BoardState
    {
        private readonly byte _numColumns;
        private readonly byte _numRows;
        private readonly List<int> _discs;
        private const int WinningConnectionValue = 6;
        private const int EmptyDisc = -1;
        
        public readonly ushort Columns;
        public readonly ushort Capacity => (ushort)_discs.Count;
        public ushort OpponentDisc {get; private set;}
        public ushort AIDisc {get; private set;}
        public ushort DiscInUse {get; private set;}
        
        /// <summary>
        /// Initializes a new board state with the given board info.
        /// </summary>
        /// <param name="boardInfo">The information about the board (columns, rows, and capacity).</param>
        public BoardState(BoardInfo boardInfo)
        {
            _numColumns = boardInfo.NumColumns;
            _numRows = boardInfo.NumRows;
            Columns = (ushort)boardInfo.Columns.Count;

            _discs = new List<int>();
            for(int i = 0; i < boardInfo.Capacity; i++)
            {
                _discs.Add(EmptyDisc);
            }

            DiscInUse = 0;
            OpponentDisc = 0;
            AIDisc = 0;
        }
        
        /// <summary>
        /// Creates a new board state by copying the data from an existing board state.
        /// </summary>
        /// <param name="boardState">The board state to copy from.</param>
        public BoardState(BoardState boardState)
        {
            _numColumns = boardState._numColumns;
            _numRows = boardState._numRows;
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

        /// <summary>
        /// Checks if the given column is empty (no discs placed in it).
        /// </summary>
        /// <param name="column">The column to check.</param>
        /// <returns>True if the column is empty, false otherwise.</returns>
        public readonly bool IsColumnEmpty(int column)
        {
            ushort endColumn = (ushort)(_numRows * (column + 1));
            return _discs[endColumn - 1] == -1;
        }
        
        /// <summary>
        /// Gets the index of the next available disc in the specified column.
        /// </summary>
        /// <param name="column">The column to check for an available spot.</param>
        /// <returns>The index of the next available disc, or -1 if no spot is available.</returns>
        public readonly int GetNextAvailableDiscIndex(int column)
        {
            ushort startColumn = (ushort)(_numRows * column);
            ushort endColumn = (ushort)(_numRows * (column + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                if (_discs[i] == -1) return i;
            }
            
            // No disc is available 
            return -1;
        }

        /// <summary>
        /// Places a disc at the specified index for the given turn.
        /// </summary>
        /// <param name="index">The index where the disc will be placed.</param>
        /// <param name="turn">The turn number (0 for opponent, 1 for AI).</param>
        public void PlaceDisc(int index, int turn)
        {
            _discs[index] = turn;
            ++DiscInUse;
            if (turn == 0) ++OpponentDisc;
            else           ++AIDisc;
        }

        /// <summary>
        /// Evaluates the value of a neighboring disc based on the current turn.
        /// </summary>
        /// <param name="turn">The current player's turn.</param>
        /// <param name="neighborIndex">The index of the neighbor disc to evaluate.</param>
        /// <returns>The value of the neighbor: 2 for the player's disc, 1 for an empty space, and -2 for the opponent's disc.</returns>
        private readonly int EvaluateNeighbor(int turn, int neighborIndex)
        {
            // adversary disc
            int neighborValue = -2;
            
            // your disc
            if (_discs[neighborIndex] == turn)
                neighborValue = 2;
            
            // empty = -1 by default
            else if (_discs[neighborIndex] == EmptyDisc)
                neighborValue = 1;

            return neighborValue;
        }
        
        /// <summary>
        /// Checks for a possible connection of 4 discs in a row based on given neighbor directions.
        /// </summary>
        /// <param name="neighbor1Kvp">The first neighbor direction (x, y).</param>
        /// <param name="neighbor2Kvp">The second neighbor direction (x, y).</param>
        /// <param name="neighbor3Kvp">The third neighbor direction (x, y).</param>
        /// <param name="turn">The current player's turn.</param>
        /// <returns>The value representing the best connection found (WinningConnectionValue, or a lower value).</returns>
        private readonly int CheckConnect4(KeyValuePair<int, int> neighbor1Kvp,
                                  KeyValuePair<int, int> neighbor2Kvp,
                                  KeyValuePair<int, int> neighbor3Kvp,
                                  int turn)
        {
            int maxNeighborValue = int.MinValue;
            for (ushort i = 0; i < Capacity; ++i)
            {
                ushort row    = (ushort)(i % _numRows);
                ushort column = (ushort)(i / _numRows);
                int currentDisc = _discs[i];
                    
                if (currentDisc != turn) continue;

                // Get neighbor index
                int neighbor1Index = (column + neighbor1Kvp.Key) * _numRows +
                                     (row + neighbor1Kvp.Value);
                
                int neighbor2Index = (column + neighbor2Kvp.Key) * _numRows +
                                     (row + neighbor2Kvp.Value);
                
                int neighbor3Index = (column + neighbor3Kvp.Key) * _numRows +
                                     (row + neighbor3Kvp.Value);
                
                // Don't cross the threshold
                if (neighbor1Index < 0 || 
                    neighbor2Index < 0 || 
                    neighbor3Index < 0 ||
                    neighbor1Index >= Capacity || 
                    neighbor2Index >= Capacity || 
                    neighbor3Index >= Capacity) 
                    continue;
                
                // Check anomalies (e.g., make sure the neighbors are in the same row/column)
                // Example:
                //      Columns: 7, Rows: 6
                //      Discs: 4,5,6,7 (first two last discs of the row: 0 and other last first two of the row: 1)
                bool columns = neighbor1Index / _numRows == column + neighbor1Kvp.Key &&
                               neighbor2Index / _numRows == column + neighbor2Kvp.Key &&
                               neighbor3Index / _numRows == column + neighbor3Kvp.Key;
                
                bool rows = neighbor1Index % _numRows == row + neighbor1Kvp.Value &&
                            neighbor2Index % _numRows == row + neighbor2Kvp.Value &&
                            neighbor3Index % _numRows == row + neighbor3Kvp.Value;
                    
                if(!columns || !rows) continue;
                
                // Neighbor Discs values
                int neighbor1Value = EvaluateNeighbor(turn, neighbor1Index);
                int neighbor2Value = EvaluateNeighbor(turn, neighbor2Index);
                int neighbor3Value = EvaluateNeighbor(turn, neighbor3Index);
                int neighborsValue = neighbor1Value + neighbor2Value + neighbor3Value;
                
                if(neighborsValue == WinningConnectionValue) return WinningConnectionValue;
                if(neighborsValue > maxNeighborValue) maxNeighborValue = neighborsValue;
            }
            return maxNeighborValue;
        }

        /// <summary>
        /// Checks if the game is a draw (no empty spaces left).
        /// </summary>
        /// <returns>True if the board is full, false otherwise.</returns>
        public readonly bool Draw() => _discs.All(disc => disc != -1);
         
        /// <summary>
        /// Evaluates the current board state for a potential winning connection.
        /// </summary>
        /// <param name="turn">The current player's turn.</param>
        /// <returns>The score for the current board state.</returns>
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
            if (connects[0] == WinningConnectionValue) return WinningConnectionValue;

            // Vertical Check 
            connects[1] = CheckConnect4
            (   new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 0),
                new KeyValuePair<int, int>(3, 0),
                turn
            );
            if (connects[1] == WinningConnectionValue) return WinningConnectionValue;

            
            // Ascending Diagonal Check 
            connects[2] = CheckConnect4
            (
                new KeyValuePair<int, int>(1, 1),
                new KeyValuePair<int, int>(2, 2),
                new KeyValuePair<int, int>(3, 3),
                turn
            );
            if (connects[2] == WinningConnectionValue) return WinningConnectionValue;
            
            // Descending Diagonal Check
            connects[3] = CheckConnect4
            (
                new KeyValuePair<int, int>(1, -1),
                new KeyValuePair<int, int>(2, -2),
                new KeyValuePair<int, int>(3, -3),
                turn
            );
            if (connects[3] == WinningConnectionValue) return WinningConnectionValue;

            int maxConnect = 0;
            foreach (int connect in connects)
            {
                if (connect > maxConnect) maxConnect = connect;
            }
            
            return maxConnect;
        }
        
        /// <summary>
        /// Checks if the given player has won by forming a Connect 4.
        /// </summary>
        /// <param name="turn">The current player's turn.</param>
        /// <returns>True if the player has won, false otherwise.</returns>
        public readonly bool HasWinner(int turn)
        {
            // Horizontal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(0, 1),
                    new KeyValuePair<int, int>(0, 2),
                    new KeyValuePair<int, int>(0, 3),
                    turn
                ) == WinningConnectionValue
               )
                return true;

            // Vertical Check 
            if (CheckConnect4
                (   new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(2, 0),
                    new KeyValuePair<int, int>(3, 0),
                    turn
                ) == WinningConnectionValue
               )
                return true;
            
            // Ascending Diagonal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, 1),
                    new KeyValuePair<int, int>(2, 2),
                    new KeyValuePair<int, int>(3, 3),
                    turn
                ) == WinningConnectionValue
               )
                return true;
            
            // Descending Diagonal Check
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(1, -1),
                    new KeyValuePair<int, int>(2, -2),
                    new KeyValuePair<int, int>(3, -3),
                    turn
                ) == WinningConnectionValue
               )
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Logs the current state of the board to the console.
        /// </summary>
        public void LogResult()
        {
            string board = "";
            for (int i = _numRows-1; i > -1; --i)
            {
                string row = "";
                int position = i;
                
                for (int j = 0; j < _numColumns; ++j)
                {
                    row += $"{_discs[position]}\t";
                    position += _numRows;
                }
                board += $"{row}\n";
            }
            Debug.Log(board);
        }
        
        /// <summary>
        /// Prints the result of the game condition and the current statistics to the console.
        /// </summary>
        /// <param name="condition">The condition of the game (win, draw, etc.).</param>
        public void PrintResult(string condition)
        {
            Debug.Log($"{condition}\nDiscs: {DiscInUse}/{Capacity} - Opponent: {OpponentDisc} - AI: {AIDisc}");

        }
    }
}