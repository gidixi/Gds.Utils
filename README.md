# Gds.Utils

## Descrizione

Il namespace `Gds.Utils` contiene utility per la gestione di dati persistenti in formato CSV e JSON. Queste utility sono progettate per semplificare la lettura, scrittura, aggiornamento e cancellazione di dati in file CSV e JSON.

## Scopo

Lo scopo principale di questo namespace è fornire un'interfaccia semplice e robusta per la gestione di dati persistenti in applicazioni .NET. Le classi implementate consentono di lavorare con file CSV e JSON senza dover gestire manualmente la serializzazione e la deserializzazione dei dati.

## Classi Implementate

### 1. `CsvManager<T>`

#### Descrizione

La classe `CsvManager<T>` gestisce la lettura e la scrittura di record CSV per un tipo specifico `T`. Utilizza la libreria `CsvHelper` per la gestione del formato CSV e `ILogger` per la registrazione delle operazioni.

#### Metodi Principali

- **ReadRecords()**: Legge tutti i record dal file CSV e li restituisce come una lista di tipo `T`.
- **WriteRecords(IEnumerable<T> records)**: Scrive una serie di record nel file CSV.
- **AddRecord(T newRecord)**: Aggiunge un nuovo record al file CSV.
- **UpdateRecord(Func<T, bool> predicate, Action<T> updateMethod)**: Aggiorna un record esistente nel file CSV.
- **DeleteRecord(Func<T, bool> predicate)**: Elimina un record dal file CSV.

#### Scopo Futuro

- **Miglioramento delle Prestazioni**: Implementazione di bufferizzazione per migliorare le prestazioni in caso di operazioni frequenti.
- **Supporto per Compressione**: Aggiunta del supporto per la compressione dei file CSV per ridurre lo spazio di archiviazione.
- **Integrazione con Database**: Possibilità di sincronizzare i dati CSV con un database relazionale.

### 2. `JsonKeyValueStore`

#### Descrizione

La classe `JsonKeyValueStore` implementa un archivio chiave-valore persistente utilizzando un file JSON. I dati vengono caricati dal file all'avvio e salvati ogni volta che viene effettuata una modifica.

#### Metodi Principali

- **Set(string key, object value)**: Imposta un valore per una chiave specificata. Se la chiave esiste già, il valore viene aggiornato.
- **Get<T>(string key)**: Ottiene il valore associato alla chiave specificata.
- **ContainsKey(string key)**: Verifica se una chiave esiste nell'archivio.
- **Remove(string key)**: Rimuove una chiave e il suo valore associato dall'archivio.
- **Clear()**: Rimuove tutte le chiavi e i valori dall'archivio.
- **GetAll()**: Ottiene una copia di tutte le chiavi e i valori nell'archivio.

#### Scopo Futuro

- **Miglioramento delle Prestazioni**: Implementazione di bufferizzazione per migliorare le prestazioni in caso di operazioni frequenti.
- **Supporto per Compressione**: Aggiunta del supporto per la compressione dei file JSON per ridurre lo spazio di archiviazione.
- **Integrazione con Database**: Possibilità di sincronizzare i dati JSON con un database relazionale.
- **Supporto per Tipi Complessi**: Estensione del supporto per tipi di dati complessi come oggetti annidati e collezioni.

## Requisiti

- .NET Core 3.1 o versioni successive.
- Libreria `CsvHelper` per la gestione dei file CSV.
- Libreria `Newtonsoft.Json` per la gestione dei file JSON.
- Libreria `Microsoft.Extensions.Logging` per la registrazione delle operazioni.

## Installazione

Per utilizzare queste utility nel tuo progetto, aggiungi i seguenti pacchetti NuGet:

```bash
dotnet add package CsvHelper
dotnet add package Newtonsoft.Json
dotnet add package Microsoft.Extensions.Logging
```

## Esempi di Uso
Utilizzo di CsvManager<T>

```csharp
using Gds.Utils.Csv;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<CsvManager<MyRecord>>();

var csvManager = new CsvManager<MyRecord>("path/to/file.csv", logger);
csvManager.AddRecord(new MyRecord { Id = 1, Name = "Example" });
```


Utilizzo di JsonKeyValueStore

```csharp
using Gds.Utilis.Json;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<JsonKeyValueStore>();

var jsonStore = new JsonKeyValueStore("path/to/file.json", logger);
jsonStore.Set("key1", "value1");
var value = jsonStore.Get<string>("key1");
```
