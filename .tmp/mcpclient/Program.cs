using System.Diagnostics;
using System.Text;
using System.Text.Json;

var serverDll = @"C:\w\DemoCopilot260706\mcp\bin\Debug\net10.0\Backend.Mcp.dll";
var workingDirectory = @"C:\w\DemoCopilot260706";
var titles = new[] { "Demo Alpha", "Demo Beta", "Demo Gamma" };

using var process = new Process
{
    StartInfo = new ProcessStartInfo("dotnet", $"\"{serverDll}\"")
    {
        WorkingDirectory = workingDirectory,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    }
};

process.Start();

var stderrTask = Task.Run(async () =>
{
    var reader = process.StandardError;
    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();
        if (!string.IsNullOrWhiteSpace(line))
        {
            Console.Error.WriteLine(line);
        }
    }
});

var client = new McpClient(process);
await client.InitializeAsync();
var tools = await client.ListToolsAsync();
Console.WriteLine($"Tools disponibles: {string.Join(", ", tools.Select(t => t))}");

foreach (var title in titles)
{
    var result = await client.CallToolAsync("crear_tarea", new Dictionary<string, object?>
    {
        ["titulo"] = title,
        ["tipoTareaId"] = 3,
        ["prioridad"] = "Alta",
        ["notas"] = "Tarea demo creada por MCP"
    });

    Console.WriteLine($"Created: {title}");
    Console.WriteLine(result);
}

process.Kill(true);
await stderrTask;

internal sealed class McpClient
{
    private readonly Process _process;
    private int _nextId = 1;

    public McpClient(Process process)
    {
        _process = process;
    }

    public async Task InitializeAsync()
    {
        await SendAsync(new
        {
            jsonrpc = "2.0",
            id = NextId(),
            method = "initialize",
            @params = new
            {
                protocolVersion = "2025-03-26",
                capabilities = new { },
                clientInfo = new { name = "mcp-demo-client", version = "1.0" }
            }
        });

        await ReadMessageAsync();

        await SendAsync(new
        {
            jsonrpc = "2.0",
            method = "notifications/initialized",
            @params = new { }
        });
    }

    public async Task<IReadOnlyList<string>> ListToolsAsync()
    {
        await SendAsync(new
        {
            jsonrpc = "2.0",
            id = NextId(),
            method = "tools/list",
            @params = new { }
        });

        var response = await ReadMessageAsync();
        using var document = JsonDocument.Parse(response);
        var tools = document.RootElement.GetProperty("result").GetProperty("tools");
        return tools.EnumerateArray().Select(tool => tool.GetProperty("name").GetString()!).ToList();
    }

    public async Task<string> CallToolAsync(string toolName, IDictionary<string, object?> arguments)
    {
        await SendAsync(new
        {
            jsonrpc = "2.0",
            id = NextId(),
            method = "tools/call",
            @params = new
            {
                name = toolName,
                arguments
            }
        });

        var response = await ReadMessageAsync();
        return response;
    }

    private async Task SendAsync(object payload)
    {
        var body = JsonSerializer.Serialize(payload);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var header = Encoding.ASCII.GetBytes($"Content-Length: {bodyBytes.Length}\r\n\r\n");
        await _process.StandardInput.BaseStream.WriteAsync(header, 0, header.Length);
        await _process.StandardInput.BaseStream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
        await _process.StandardInput.BaseStream.FlushAsync();
    }

    private async Task<string> ReadMessageAsync()
    {
        var stream = _process.StandardOutput.BaseStream;
        var headerLines = new List<string>();
        while (true)
        {
            var line = await ReadLineAsync(stream);
            if (line is null)
            {
                throw new EndOfStreamException("El servidor MCP cerró la conexión antes de responder.");
            }

            if (line.Length == 0)
            {
                break;
            }

            headerLines.Add(line);
        }

        var contentLengthHeader = headerLines.Single(header => header.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase));
        var contentLength = int.Parse(contentLengthHeader.Split(':', 2)[1].Trim());
        var buffer = new byte[contentLength];
        await stream.ReadExactlyAsync(buffer, 0, contentLength);
        return Encoding.UTF8.GetString(buffer);
    }

    private static async Task<string?> ReadLineAsync(Stream stream)
    {
        var builder = new StringBuilder();
        while (true)
        {
            var value = await stream.ReadAsync(new byte[1], 0, 1);
            if (value == 0)
            {
                return builder.Length == 0 ? null : builder.ToString();
            }

            var b = (byte[])null;
        }
    }

    private int NextId() => _nextId++;
}
