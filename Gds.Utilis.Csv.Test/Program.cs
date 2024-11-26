using Gds.Utils.Csv;
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
            .ConfigureServices((hostContext, services) =>
            {
               
                services.AddLogging(configure => configure.AddConsole());

                
                services.AddScoped(typeof(CsvManager<>));
            })
            .Build();

        // Avvia un servizio per eseguire le operazioni CRUD
        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            await RunCsvOperations(services);
        }

        await host.RunAsync();
    }

    private static async Task RunCsvOperations(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            // Recuperiamo il CsvCrud per il tipo Person
            var csvCrud = new CsvManager<Person>("./people.csv", services.GetRequiredService<ILogger<CsvManager<Person>>>(), true);

            // Esempio di aggiunta di un record
            csvCrud.AddRecord(new Person { Id = 1, Name = "John Doe", Email = "john@example.com" });

            // Esempio di lettura dei record
            var people = csvCrud.ReadRecords();
            foreach (var person in people)
            {
                logger.LogInformation($"Read Person: {person.Id}, {person.Name}, {person.Email}");
            }

            // Esempio di aggiornamento di un record
            csvCrud.UpdateRecord(p => p.Id == 1, p => p.Name = "Jane Doe");

            // Esempio di eliminazione di un record
            csvCrud.DeleteRecord(p => p.Id == 1);

            // Simuliamo un'attesa
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred.");
        }
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

