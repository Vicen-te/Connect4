using System.Collections.Generic;

namespace AI.MTD
{
    public class TranspositionTable
    {
        private readonly int length;
        private readonly Dictionary<int, BoardRecord> records;

        public int SavedRecords { get; private set; }
        public int UsedRecords { get; private set; }
        public int OverwrittenRecords { get; private set; }
        public int NotFoundRecords { get; private set; }
        
        // RegCoincidences, RegNoCoincidencesx

        public TranspositionTable(int length)
        {
            records = new Dictionary<int, BoardRecord>();
            this.length = length;
        }
        
        public void SaveRecord(BoardRecord record)
        {
            int index = record.HashValue % length;

            if (records.TryGetValue(index, out BoardRecord actualRecord))
                ++OverwrittenRecords;
            else
                ++SavedRecords;
            
            records[index] = record;
        }
        
        public BoardRecord GetRecord(int hash)
        {
            int key = hash % length;
            if (records.TryGetValue(key, out BoardRecord record))
            {
                if (record.HashValue == hash)
                {
                    ++UsedRecords;
                    return record;
                }
                
                ++NotFoundRecords;
                return null;
            }
            return null;
        }

    }
}
