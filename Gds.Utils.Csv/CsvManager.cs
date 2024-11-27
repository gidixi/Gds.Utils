using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Gds.Utils.Csv
{
    /// <summary>
    /// Gestisce la lettura e la scrittura di record CSV per un tipo specifico <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Il tipo di record da gestire.</typeparam>
    public class CsvManager<T> where T : class, new()
    {
        private readonly string _filePath;
        private readonly CsvConfiguration _config;
        private readonly ILogger<CsvManager<T>> _logger;
        private readonly bool _includeHeader;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="CsvManager{T}"/> con il percorso del file specificato.
        /// </summary>
        /// <param name="filePath">Il percorso del file CSV.</param>
        /// <param name="logger">Il logger per registrare le operazioni.</param>
        /// <param name="includeHeader">Indica se includere l'intestazione nel file CSV (default è true).</param>
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

        /// <summary>
        /// Assicura che il file CSV esista. Se non esiste, lo crea e aggiunge l'intestazione se necessario.
        /// </summary>
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

        /// <summary>
        /// Legge tutti i record dal file CSV.
        /// </summary>
        /// <returns>Una lista di record del tipo <typeparamref name="T"/>.</returns>
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

        /// <summary>
        /// Scrive una serie di record nel file CSV.
        /// </summary>
        /// <param name="records">La serie di record da scrivere.</param>
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

        /// <summary>
        /// Aggiunge un nuovo record al file CSV.
        /// </summary>
        /// <param name="newRecord">Il nuovo record da aggiungere.</param>
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

        /// <summary>
        /// Aggiorna un record esistente nel file CSV.
        /// </summary>
        /// <param name="predicate">La funzione per trovare il record da aggiornare.</param>
        /// <param name="updateMethod">Il metodo per aggiornare il record trovato.</param>
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

        /// <summary>
        /// Elimina un record dal file CSV.
        /// </summary>
        /// <param name="predicate">La funzione per trovare il record da eliminare.</param>
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
