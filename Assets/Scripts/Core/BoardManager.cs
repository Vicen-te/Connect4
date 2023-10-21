using System;
using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;

namespace Core
{
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
            boardLoader.BuildBoard();
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
        
        /// <summary>
        /// Conncet4 Evaluation condition
        /// </summary>
        /// <param name="neighbor1">Adding row/columns to current disc</param>
        /// <param name="neighbor2">Adding row/columns to current disc</param>
        /// <param name="neighbor3">Adding row/columns to current disc</param>
        /// <param name="checkActorOwner">Checks for each disk if it is visible and if it belongs
        /// to the current actor.</param>
        /// <param name="checkNeighbors">Checks if all the neighbor form 4 in a row.</param>
        /// <returns>True if it forms 4 in a row.</returns>
        public bool CheckConnect4(KeyValuePair<int, int> neighbor1,
                                  KeyValuePair<int, int> neighbor2,
                                  KeyValuePair<int, int> neighbor3,
                                  Func<Disc, bool> checkActorOwner, 
                                  Func<Disc, Disc, Disc, Disc, bool> checkNeighbors)
        { 
            for (ushort i = 0; i < Capacity; ++i)
            {
                ushort row    = (ushort)(i / RowsInt);
                ushort column = (ushort)(i % RowsInt);

                if (checkActorOwner(Discs[i]))
                {
                    // Get neighbor index
                    int neighbor1Index = (row + neighbor1.Key) * RowsInt +
                                         (column + neighbor1.Value);
                    
                    int neighbor2Index = (row + neighbor2.Key) * RowsInt +
                                         (column + neighbor2.Value);
                    
                    int neighbor3Index = (row + neighbor3.Key) * RowsInt +
                                         (column + neighbor3.Value);
                    
                    // Don't cross the threshold
                    if (neighbor1Index < 0 || 
                        neighbor2Index < 0 || 
                        neighbor3Index < 0 ||
                        neighbor1Index >= Capacity || 
                        neighbor2Index >= Capacity || 
                        neighbor3Index >= Capacity) 
                        continue;
                    
                    // Check for same actor owner
                    bool actorOwner = checkActorOwner(Discs[i]) &&
                                      checkActorOwner(Discs[neighbor1Index]) &&
                                      checkActorOwner(Discs[neighbor2Index]) &&
                                      checkActorOwner(Discs[neighbor3Index]);
                    
                    if(!actorOwner) continue;
                    
                    // Check if the actual disc has the corresponding neighbors
                    bool neighbors = checkNeighbors(Discs[i],
                                                    Discs[neighbor1Index],
                                                    Discs[neighbor2Index],
                                                    Discs[neighbor3Index]);
                    
                    // Connect4  
                    if (neighbors)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}