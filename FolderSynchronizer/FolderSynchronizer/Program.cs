using FolderSynchronizer.implemetation;
using FolderSynchronizer.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length < 4)
{
    Console.WriteLine("Usage: FolderSynchronizer <sourceFolder> <destinationFolder> <syncIntervalInHours> <logFilePath>");
    return;
}

string sourceFolder = args[0];
string destinationFolder = args[1];
if (!int.TryParse(args[2], out int syncIntervalInHours))
{
    Console.WriteLine("Invalid synchronization interval. Please provide a valid number.");
    return;
}
string logFilePath = !string.IsNullOrEmpty(args[3]) ? args[3] : "Logs/app-{Date}.txt";

// Set up dependency injection
var serviceProvider = new ServiceCollection()
    .AddLogging(configure =>
    {
        configure.AddConsole();
        configure.AddFile(logFilePath);
    })
    .AddSingleton<IFolderSynchronizer, Synchronizer>()
    .AddSingleton<IBackupService, BackupService>()
    .AddSingleton<ICompressionService, CompressionService>()
    .AddSingleton<IEncryptionService, EncryptionService>()
    .BuildServiceProvider();

var logger = serviceProvider.GetService<ILogger<Program>>();
var folderSynchronizer = serviceProvider.GetService<IFolderSynchronizer>();

// Schedule synchronization
Timer timer = new Timer(state =>
{
    try
    {
        folderSynchronizer?.SynchronizeFolders(sourceFolder, destinationFolder);
        logger?.LogInformation("Folders synchronized successfully.");
    }
    catch (Exception ex)
    {
        logger?.LogError($"An error occurred: {ex.Message}");
    }
}, null, TimeSpan.Zero, TimeSpan.FromHours(syncIntervalInHours));

Console.WriteLine("Press [Enter] to exit...");
Console.ReadLine();
 
