using System;
using UnityEngine;

namespace AI.Algorithms.MTD
{
    public class ZobristKey
    {
        private readonly int[,] _keys;
        private readonly int _boardPosition;
        private readonly int _numberOfPieces;

        public ZobristKey(int boardPosition, int numberOfPieces)
        {
            System.Random random = new System.Random();
            _boardPosition = boardPosition;
            _numberOfPieces = numberOfPieces;
            _keys = new int[_boardPosition, _numberOfPieces];

            for (int i = 0; i < _boardPosition; i++)
            {
                for (int j = 0; j < _numberOfPieces; j++)
                {
                    _keys[i, j] = random.Next(int.MaxValue);
                }
            }
        }

        public ZobristKey (ZobristKey copy)
        {
            _keys = copy._keys;
            _boardPosition = copy._boardPosition;
            _numberOfPieces = copy._numberOfPieces;
        }

        public int Get(int position, int piece)
        {
            return _keys[position, piece];
        }
    
        public void Print ()
        {
            int i, j;
            string output = "";
            output += "Zobrist Keys:\n";
            for (i = 0; i < _boardPosition; i++)
            {
                for (j = 0; j < _numberOfPieces; j++)
                {
                    output += "Position " + Convert.ToString(i).PadLeft(2,'0') + ", Piece " + j + ": ";
                    output += Convert.ToString(_keys[i, j], 2).PadLeft(32, '0');
                    output += "\n";
                }
            }
            Debug.Log(output);
        }
    }
}
