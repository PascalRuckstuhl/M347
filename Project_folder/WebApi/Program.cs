using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/check", () => {
try
{

    var mongoClient = new MongoClient("mongodb://mongodb:27017");
    var databaseNames = mongoClient.ListDatabaseNames().ToList();

    return "Zugriff auf MongDB ok." + string.Join(",", databaseNames);
}
catch (System.Exception e)
{
    return "Zugriff auf MongoDB funktioniert nicht: " + e.Message;
}
});

app.Run();
