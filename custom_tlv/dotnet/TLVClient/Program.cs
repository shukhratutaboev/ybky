// using CustomTLV;
// using CustomTLV.UDP;

// for (var i = 0; i < 1; i++)
// {
//     var client = new Client();
//     await client.SendAsync(new Person
//     {
//         FirstName = "Jane",
//         LastName = "Doe",
//         Age = 30
//     }, "127.0.0.2", 8080);

//     var person = await client.ReceiveAsync<Person>();

//     Console.WriteLine($"Received person {i+1}: {person.FirstName} {person.LastName} ({person.Age})");

//     await client.CloseAsync();
// }

// HTTP

// using System.Net.Http.Json;
// using CustomTLV;

// for (var i = 0; i < 1; i++)
// {
//     var client = new HttpClient
//     {
//         BaseAddress = new Uri("http://127.0.0.2:8080")
//     };

//     var person = new Person
//     {
//         FirstName = "Jane",
//         LastName = "Doe",
//         Age = 30
//     };

//     var response = await client.PostAsJsonAsync("api/person", person);

//     var responsePerson = await response.Content.ReadFromJsonAsync<Person>();

//     Console.WriteLine($"Received person {i+1}: {responsePerson.FirstName} {responsePerson.LastName} ({responsePerson.Age})");

//     client.Dispose();
// }

using BenchmarkDotNet.Running;
using TLVClient;

BenchmarkRunner.Run<ProtocolsBenchmarks>();