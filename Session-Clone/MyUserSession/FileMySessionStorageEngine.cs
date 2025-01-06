using System.Text.Json;
using FileStream = System.IO.FileStream;

namespace Session_Clone.MyUserSession;

public class FileMySessionStorageEngine(string directoryPath) : IMySessionStorageEngine
{
    public async Task CommitAsync(string id, Dictionary<string, byte[]> sessionsStore,
        CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(directoryPath, id);
        using FileStream fileStream = new FileStream(filePath, FileMode.Create);
        using StreamWriter streamWriter = new StreamWriter(fileStream);

        streamWriter.Write(JsonSerializer.Serialize(sessionsStore));
    }

    public async Task<Dictionary<string, byte[]>> LoadAsync(string id, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(directoryPath, id);
        if (!File.Exists(filePath))
        {
            return [];
        }

        using FileStream fileStream = new FileStream(filePath, FileMode.Open);
        using StreamReader streamReader = new StreamReader(fileStream);

        var read_json_string = await streamReader.ReadToEndAsync(cancellationToken);

        return JsonSerializer.Deserialize<Dictionary<string, byte[]>>(read_json_string) ?? [];
    }
}