using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;

namespace Core
{
    // setup board
    public sealed class BoardManager : MonoBehaviour
    {
        [SerializeField] private BoardLoader boardLoader;
        public BoardInfo BoardInfo { get; private set; }
        private byte ColumnsInt => boardLoader.ColumnsInt;
        private byte RowsInt => boardLoader.RowsInt;
        private ushort Capacity => boardLoader.Capacity;
        private List<Disc> Discs => boardLoader.Discs;
        private List<Column> Columns => boardLoader.Columns;
        
        public void SetupScene()
        {
            // Create board
            boardLoader.BuildBoard();
            
            // Pass references
            BoardInfo = new BoardInfo(boardLoader);
        }
        
        public void ForEachColumnAddInteraction(IColumInteraction.Interaction method)
        {
            for (byte columnInt = 0; columnInt < ColumnsInt; ++columnInt)
            {
                Columns[columnInt].OnInteraction += method;
            }
        }
        
        public void ForEachColumnRemoveInteraction(IColumInteraction.Interaction method)
        {
            for (byte columnInt = 0; columnInt < ColumnsInt; ++columnInt)
            {
                Columns[columnInt].OnInteraction -= method;
            }
        }
    }
}