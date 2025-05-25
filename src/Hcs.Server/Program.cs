// TODO: use static log for boot's errors
using Hcs.Server;

var builder = WebApplication.CreateBuilder(args);

builder.PreBuild();

var app = builder.Build();

await app.PostBuild();

await app.RunAsync();
