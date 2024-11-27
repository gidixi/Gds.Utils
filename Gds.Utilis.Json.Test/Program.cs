using Gds.Utilis.Json;

public static class Program
{
    static void Main(string[] args)
    {
        string filePath = "data.json";
        JsonKeyValueStore store = new JsonKeyValueStore(filePath);

        // Scrivere una chiave-valore
        store.Set("name", "John Doe");
        store.Set("age", 30);

        // Leggere una chiave-valore
        string name = store.Get<string>("name");
        int age = store.Get<int>("age");

        Console.WriteLine($"Name: {name}, Age: {age}");

        // Aggiornare una chiave-valore
        store.Set("age", 31);

        // Verificare se una chiave esiste
        bool hasName = store.ContainsKey("name");
        Console.WriteLine($"Has 'name' key: {hasName}");

        // Rimuovere una chiave-valore
        store.Remove("name");

        // Ottenere tutte le chiavi-valori
        var allData = store.GetAll();
        foreach (var kvp in allData)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        // Pulire tutto il contenuto
        store.Clear();
    }
}
