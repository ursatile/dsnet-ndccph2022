using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeter;
using System.Diagnostics;

using var channel = GrpcChannel.ForAddress("https://localhost:7257");
var client = new Greeter.GreeterClient(channel);
var count = 1;
var MAX = 10000;
var sw = new Stopwatch();
sw.Start();
while(count < MAX) {
    var reply = await client.SayHelloAsync(
        new HelloRequest { Name = $"NDC Copenhagen {count++}" }
    );
    // Console.WriteLine(reply.Message);
}
sw.Stop();
Console.WriteLine($"gRPC did {MAX} requests in {sw.ElapsedMilliseconds}ms");

