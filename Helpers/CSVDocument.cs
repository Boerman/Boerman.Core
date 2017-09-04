using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Boerman.Core.Helpers
{
    public class CSVDocument
    {
        private string _location;
        private char _splitter;

        private ICollection<Entry> _entries = new List<Entry>();

        public CSVDocument(string location, char splitter = ';')
        {
            _location = location;
            _splitter = splitter;
        }

        private async Task Writer()
        {
            StreamWriter writer = new StreamWriter(_location);
            await writer.WriteLineAsync($"SPLIT={_splitter}");
        }

        public void AddColumn(string column, IEnumerable<Entry> entries)
        {
            foreach (var entry in entries)
            {
                entry.Column = column;
                _entries.Add(entry);
            }
        }

        public class Entry
        {
            public Entry(DateTime timeStamp, string value)
            {
                TimeStamp = timeStamp;
                Value = value;
            }

            public DateTime TimeStamp { get; }
            public string Value { get; }

            internal string Column;
        }
    }
}
