﻿using QSP.TOPerfCalculation.Boeing;
using QSP.Utilities;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace QSP.TOPerfCalculation
{
    public class TOTableLoader
    {
        private string folderPath;
        private const string defaultFolderPath = @"PerformanceData\TO";

        public TOTableLoader(string folderPath = defaultFolderPath)
        {
            this.folderPath = folderPath;
        }

        /// <summary>
        /// Load all xml in the landing performance data folder.
        /// </summary>
        public TableImportResult Load()
        {
            var tables = new List<PerfTable>();

            foreach (var i in Directory.GetFiles(folderPath))
            {
                try
                {
                    tables.Add(new PerfDataLoader().ReadFromXml(i));
                }
                catch (Exception ex)
                {
                    LoggerInstance.WriteToLog(ex);
                }
            }

            var groups = tables.GroupBy(x => x.Entry.ProfileName);

            return new TableImportResult(
                groups
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .ToList(),
                Message(tables));
        }

        private static string Message(List<PerfTable> item)
        {
            var groups = item.GroupBy(x => x.Entry.ProfileName);

            try
            {
                var duplicate = groups.First(g => g.Count() > 1);

                return
                    "The following aircrafts have" +
                    " identical profile names:\n\n" +
                    string.Join("\n", duplicate.Select(x => x.Entry.FilePath)) +
                    "\n\nNone of these profiles will be loaded.";
            }
            catch (InvalidOperationException)
            {
                // There is not duplicate.
                return null;
            }
        }

        public class TableImportResult
        {
            public List<PerfTable> Tables { get; private set; }
            public string Message { get; private set; }

            public TableImportResult(List<PerfTable> Tables, string Message)
            {
                this.Tables = Tables;
                this.Message = Message;
            }
        }
    }
}
