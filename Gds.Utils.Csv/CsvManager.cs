using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Gds.Utils.Csv
{
    public class CsvManager<T> where T : class, new()
    {
        private readonly string _filePath;
        private readonly CsvConfiguration _config;
        private readonly ILogger<CsvManager<T>> _logger;
        private readonly bool _includeHeader;

        public CsvManager(string filePath, ILogger<CsvManager<T>> logger, bool includeHeader = true)
        {
            _filePath = filePath;
            _includeHeader = includeHeader;
            _logger = logger;
            _config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                HasHeaderRecord = includeHeader,
            };

            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    using (var stream = File.Create(_filePath))
                    {
                        if (_includeHeader)
                        {
                            using (var writer = new StreamWriter(stream))
                            using (var csv = new CsvWriter(writer, _config))
                            {
                                csv.WriteHeader<T>();
                                csv.NextRecord();
                            }
                        }
                    }
                    _logger.LogInformation("Created new file with headers: {FilePath}", _filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create file: {FilePath}", _filePath);
                throw;
            }
        }

        public List<T> ReadRecords()
        {
            try
            {
                using (var reader = new StreamReader(_filePath))
                using (var csv = new CsvReader(reader, _config))
                {
                    return csv.GetRecords<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read records");
                throw;
            }
        }

        public void WriteRecords(IEnumerable<T> records)
        {
            try
            {
                using (var writer = new StreamWriter(_filePath, false))
                using (var csv = new CsvWriter(writer, _config))
                {
                    if (_includeHeader) csv.WriteHeader<T>();
                    csv.NextRecord();
                    csv.WriteRecords(records);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write records");
                throw;
            }
        }

        public void AddRecord(T newRecord)
        {
            try
            {
                var records = ReadRecords();
                records.Add(newRecord);
                WriteRecords(records);
                _logger.LogInformation("Added new record");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add record");
                throw;
            }
        }

        public void UpdateRecord(Func<T, bool> predicate, Action<T> updateMethod)
        {
            try
            {
                var records = ReadRecords();
                var recordToUpdate = records.FirstOrDefault(predicate);
                if (recordToUpdate != null)
                {
                    updateMethod(recordToUpdate);
                    WriteRecords(records);
                    _logger.LogInformation("Updated record with ID: {Id}", recordToUpdate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update record");
                throw;
            }
        }

        public void DeleteRecord(Func<T, bool> predicate)
        {
            try
            {
                var records = ReadRecords();
                var recordToRemove = records.FirstOrDefault(predicate);
                if (recordToRemove != null)
                {
                    records.Remove(recordToRemove);
                    WriteRecords(records);
                    _logger.LogInformation("Deleted record with ID: {Id}", recordToRemove);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete record");
                throw;
            }
        }
    }
}
