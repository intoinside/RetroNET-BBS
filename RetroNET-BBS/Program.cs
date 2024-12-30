// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Parser.Markdown;
using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Server;

Console.WriteLine("Hello, World!");

// Start document import
var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
var config = builder.Build();

var folder = config["Path"];
var homePath = Path.Combine(folder, "index.md");

PageContainer.Pages.Add(MarkdownStatic.ParseFile(homePath));

var svr = new Server("127.0.0.1");
await svr.Start();
Console.WriteLine("Goodbye, World!");
