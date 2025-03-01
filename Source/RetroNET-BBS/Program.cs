using Microsoft.Extensions.Configuration;
using Parser.Markdown;
using Parser.Raw;
using RetroNET_BBS.Client;
using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Server;

static async void StartPetsciiServer()
{
    var svr = new Server("0.0.0.0", 8502, ConnectionType.Petscii);
    await svr.Start();
}

static async void StartTelnetServer()
{
    var svr = new Server("0.0.0.0", 23, ConnectionType.Telnet);
    await svr.Start();
}

Console.WriteLine("Hello, World! This is RetroNET-BBS!");

// Start document import
var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
var config = builder.Build();

var folder = config["Path"];
var homePath = Path.Combine(folder, "index.md");

Console.WriteLine("Parsing pages...");
PageContainer.Pages = Markdown.ParseAllFiles(folder);

Console.WriteLine("Parsing imports...");
PageContainer.Imports = Seq.ParseAllFiles(folder);

Console.WriteLine("Starting servers...");
Thread thread1 = new Thread(StartPetsciiServer);
thread1.IsBackground = true;
thread1.Start();

Thread thread2 = new Thread(StartTelnetServer);
thread2.IsBackground = true;
thread2.Start();

Thread.Sleep(1000);

while (true)
{
    Thread.Sleep(1000);
}

//var svr = new Server("192.168.1.2", 8502);
//await svr.Start();

Console.WriteLine("Goodbye, World!");
