using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Boerman.Core.Helpers
{
    public static class CsvHelpers
    {
        private const int TabSpaces = 4;

        public static DataTable GetDataTableFromCsvFile(string csvLocation, char splitter = ';')
        {
            var sr = new StreamReader(csvLocation);
            var myStringRow = sr.ReadLine();
            var rows = myStringRow.Split(splitter);
            var csvData = new DataTable();

            foreach (var column in rows)
            {
                csvData.Columns.Add(column);
            }
            myStringRow = sr.ReadLine();
            while (myStringRow != null)
            {
                rows = myStringRow.Split(splitter);
                csvData.Rows.Add(rows);
                myStringRow = sr.ReadLine();
            }

            sr.Close();
            sr.Dispose();

            return csvData;
        }

        public static IEnumerable<dynamic> GetDynamicFromCsvFile(string csvLocation, char splitter = ';')
        {
            using (var reader = new StreamReader(csvLocation))
            {
                var columnDefinition = reader.ReadLine();

                if (columnDefinition == null) yield break;

                var columns = columnDefinition.Split(splitter);

                var row = reader.ReadLine();
                while (row != null)
                {
                    var expando = new ExpandedDynamic();

                    var values = row.Split(splitter);

                    for (var i = 0; i < columns.Length; i++)
                    {
                        expando[columns[i]] = values[i];
                    }

                    yield return expando;

                    row = reader.ReadLine();
                }
            }
        }

        public static void GenerateCsvFromXml(string xmlString, string resultFileName, bool isTabDelimited)
        {
            var xDoc = XDocument.Parse(xmlString);

            var tabsNeededList = new List<int>(); // only used for TabDelimited file

            var delimiter = isTabDelimited
                ? "\t"
                : ",";

            // Get title row 
            var titlesList = xDoc.Root?
                .Elements()
                .First()
                .Elements()
                .Select(s => s.Name.LocalName)
                .ToList();

            // Get the values
            var masterValuesList = xDoc.Root?
                .Elements()
                .Select(e => e
                    .Elements()
                    .Select(c => c.Value)
                    .ToList())
                .ToList();

            // Add titles as first row in master values list
            masterValuesList?.Insert(0, titlesList);

            // For tab delimited, we need to figure out the number of tabs
            // needed to keep the file uniform, for each column
            if (isTabDelimited)
            {
                for (var i = 0; i < titlesList.Count; i++)
                {
                    if (masterValuesList == null) continue;

                    var maxLength =
                        masterValuesList
                            .Select(vl => vl[i].Length)
                            .Max();
                    // assume tab is 4 characters
                    int rem;
                    var tabsNeeded = Math.DivRem(maxLength, TabSpaces, out rem);
                    tabsNeededList.Add(tabsNeeded);
                }
            }

            // Write the file
            using (var fs = new FileStream(resultFileName, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                if (masterValuesList == null) return;

                foreach (var values in masterValuesList)
                {
                    var line = string.Empty;

                    foreach (var value in values)
                    {
                        line += value;
                        if (titlesList == null || titlesList.IndexOf(value) >= titlesList.Count - 1) continue;

                        if (isTabDelimited)
                        {
                            int rem;
                            var tabsUsed = Math.DivRem(value.Length, TabSpaces, out rem);
                            var tabsLeft = tabsNeededList[values.IndexOf(value)] - tabsUsed + 1; // one tab is always needed!

                            for (var i = 0; i < tabsLeft; i++)
                            {
                                line += delimiter;
                            }
                        }
                        else // comma delimited
                        {
                            line += delimiter;
                        }
                    }

                    sw.WriteLine(line);
                }
            }
        }
    }
}
