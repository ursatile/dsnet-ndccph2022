namespace Messages;
public class Greeting
{
    public string Name { get; set; } = "World";
    public string Language { get; set; } = "en-GB";
    public string MachineName { get; set; } = Environment.MachineName;
}
