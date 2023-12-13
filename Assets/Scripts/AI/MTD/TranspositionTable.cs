using System.Collections.Generic;
using UnityEngine;

namespace AI.MTD
{
    public class TranspositionTable
    {
        public int length;
        Dictionary<int, BoardRecord> records;

        protected int UsedRecords, OverwritenRecords, NotFoundRecords, RegCoincidentes, RegNoCoincidentes;

        public TranspositionTable(int length)
        {
            records = new Dictionary<int, BoardRecord>();
            this.length = length;
        }
        public void SaveRecord(BoardRecord record)
        {
            records[record.hashValue % length] = record;
        }
        
        public BoardRecord GetRecord(int hash)
        {
            int key = hash % length;
            if (records.TryGetValue(key, out BoardRecord record))
            {
                return record.hashValue == hash ? record : null;
            }
            return null;
        }

    }
}
