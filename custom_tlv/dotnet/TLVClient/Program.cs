// using CustomTLV;
// using CustomTLV.TCP;

// for (var i = 0; i < 10; i++)
// {
//     var client = new Client("127.0.0.2", 8080);
//     client.Send(new Person
//     {
//         FirstName = "Jane",
//         LastName = "Doe",
//         Age = 30
//     });

//     var person = client.Receive<Person>();

//     Console.WriteLine($"Received person {i+1}: {person.FirstName} {person.LastName} ({person.Age})");

//     client.Close();
// }

using System.Net.Http.Json;
using CustomTLV;

for (var i = 0; i < 10; i++)
{
    var client = new HttpClient();
    client.BaseAddress = new Uri("http://127.0.0.2:8080");

    var person = new Person
    {
        FirstName = "Jane",
        LastName = "Doe",
        Age = 30
    };

    var response = await client.PostAsJsonAsync("api/person", person);

    var responsePerson = await response.Content.ReadFromJsonAsync<Person>();

    Console.WriteLine($"Received person {i+1}: {responsePerson.FirstName} {responsePerson.LastName} ({responsePerson.Age})");

    client.Dispose();
}