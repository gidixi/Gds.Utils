using Gds.Utilis.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                // Configurazione dell'applicazione (opzionale)
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Configurazione del logging
                services.AddLogging(configure => configure.AddConsole());

                // Registrazione del servizio JsonKeyValueStore
                services.AddScoped<JsonKeyValueStore>();
            })
            .Build();

        // Avvia un servizio per eseguire le operazioni CRUD
        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            await RunJsonOperations(services);
        }

        await host.RunAsync();
    }

    private static async Task RunJsonOperations(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            // Recuperiamo il JsonKeyValueStore
            var jsonStore = new JsonKeyValueStore("./data.json", services.GetRequiredService<ILogger<JsonKeyValueStore>>());

            // Esempio di aggiunta di un record
            jsonStore.Set("key1", "value1");
            jsonStore.Set("key2", 42);

            // Esempio di lettura dei record
            var value1 = jsonStore.Get<string>("key1");
            var value2 = jsonStore.Get<int>("key2");
            logger.LogInformation($"Read values: key1 = {value1}, key2 = {value2}");

            // Esempio di aggiornamento di un record
            jsonStore.Set("key1", "newValue1");

            // Esempio di eliminazione di un record
            jsonStore.Remove("key2");

            // Esempio di verifica dell'esistenza di una chiave
            var exists = jsonStore.ContainsKey("key1");
            logger.LogInformation($"Key 'key1' exists: {exists}");

            // Esempio di lettura di tutti i record
            var allData = jsonStore.GetAll();
            logger.LogInformation("All data:");
            foreach (var kvp in allData)
            {
                logger.LogInformation($"Key: {kvp.Key}, Value: {kvp.Value}");
            }

            // Simuliamo un'attesa
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred.");
        }
    }
}