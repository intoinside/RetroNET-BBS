// See https://aka.ms/new-console-template for more information
using RetroNet_BBS.Server;

Console.WriteLine("Hello, World!");

var svr = new Server("127.0.0.1");
await svr.Start();
Console.WriteLine("Goodbye, World!");
