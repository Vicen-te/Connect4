using System;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using UnityEngine;

namespace Board
{
    // Class with references of BoardLoader
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
        
        public void ForEachDisc(ushort startColumn, ushort endColumn, Action<Disc> method)
        {
            startColumn *= RowsInt;
            endColumn *= RowsInt;
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort row    = (ushort)(i % RowsInt);
                ushort column = (ushort)(i / RowsInt);
                method(Discs[i]);
                Debug.Log($"{column}, {row}");
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
                ushort iRow    = (ushort)(i % RowsInt);
                ushort iColumn = (ushort)(i / RowsInt);
                Debug.Log($"{iColumn}, {iRow}");
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
            
            // No disc is available 
            return null;
        }
        
        /// <summary>
        /// Conncet4 Evaluation condition
        /// </summary>
        /// <param name="neighbor1Kvp">Adding column/rows to current disc</param>
        /// <param name="neighbor2Kvp">Adding column/rows to current disc</param>
        /// <param name="neighbor3Kvp">Adding column/rows to current disc</param>
        /// <param name="checkActorOwner">Checks for each disk if it is visible and if it belongs
        /// to the current actor.</param>
        /// <returns>True if it forms 4 in a row.</returns>
        private bool CheckConnect4(KeyValuePair<int, int> neighbor1Kvp,
                                  KeyValuePair<int, int> neighbor2Kvp,
                                  KeyValuePair<int, int> neighbor3Kvp,
                                  Func<Disc, bool> checkActorOwner)
        { 
            for (ushort i = 0; i < Capacity; ++i)
            {
                ushort row    = (ushort)(i % RowsInt);
                ushort column = (ushort)(i / RowsInt);
                Disc currentDisc = Discs[i];
                    
                if (!checkActorOwner(currentDisc)) continue;

                // Get neighbor index
                int neighbor1Index = (column + neighbor1Kvp.Key) * RowsInt +
                                     (row + neighbor1Kvp.Value);
                
                int neighbor2Index = (column + neighbor2Kvp.Key) * RowsInt +
                                     (row + neighbor2Kvp.Value);
                
                int neighbor3Index = (column + neighbor3Kvp.Key) * RowsInt +
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
                Disc neighbor1 = Discs[neighbor1Index];
                Disc neighbor2 = Discs[neighbor2Index];
                Disc neighbor3 = Discs[neighbor3Index];
                
                // Check for same actor owner
                bool actorOwner = checkActorOwner(currentDisc) &&
                                  checkActorOwner(neighbor1) &&
                                  checkActorOwner(neighbor2) &&
                                  checkActorOwner(neighbor3);
                
                if(!actorOwner) continue;
                return true;
            }
            return false;
        }
        
        public bool Draw() => Discs.All(disc => !disc.Visibility);
        public bool Winner(int turn)
        {
            // Vertical Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(0, 1),
                    new KeyValuePair<int, int>(0, 2),
                    new KeyValuePair<int, int>(0, 3),
                    (disc) => disc.ActorOwner == turn && disc.Visibility
                )
               )
                return true;

            // Horizontal Check 
            if (CheckConnect4
                (   new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(2, 0),
                    new KeyValuePair<int, int>(3, 0),
                    (disc) => disc.ActorOwner == turn && disc.Visibility
                )
               )
                return true;
            
            // Ascending Diagonal Check 
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(-1, 1),
                    new KeyValuePair<int, int>(-2, 2),
                    new KeyValuePair<int, int>(-3, 3),
                    (disc) => disc.ActorOwner == turn && disc.Visibility
                )
               )
                return true;
            
            // Descending Diagonal Check
            if (CheckConnect4
                (
                    new KeyValuePair<int, int>(-1, -1),
                    new KeyValuePair<int, int>(-2, -2),
                    new KeyValuePair<int, int>(-3, -3),
                    (disc) => disc.ActorOwner == turn && disc.Visibility
                )
               )
                return true;
            
            return false;
        }
    }
}