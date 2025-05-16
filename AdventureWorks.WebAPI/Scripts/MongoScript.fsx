#r "nuget: MongoDB.Driver"

open MongoDB.Driver;
open MongoDB.Bson
open System.Linq;

let connectionString = "mongodb://localhost:27017"

let client = MongoClient(connectionString)
let database = client.GetDatabase("FlashFood")
let offersCol = database.GetCollection<BsonDocument>("offers")