using ErrorOr;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Leaks;

public sealed class IncapJson : IEquatable<IncapJson>
{
    private static readonly IncapJson _empty = new()
    {
        Body = "{}",
        HashCode = 0
    };

    /*
UnsafeRelaxedJsonEscaping разрешает некоторые потенциально опасные символы (например, <>).
Если это критично, создайте кастомный JavaScriptEncoder с ограничениями:

var encoderSettings = new TextEncoderSettings();
encoderSettings.AllowCharacters('\u043A', '\u043B', ' '); // Добавьте нужные символы
encoderSettings.AllowRange(UnicodeRanges.BasicLatin);
encoderSettings.AllowRange(UnicodeRanges.Cyrillic);
var options = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.Create(encoderSettings),
    WriteIndented = true
};*/
    // TODO: force use camelCase
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        // Disable экранирование символов (include cyrrilic)
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // Remove splaces
        WriteIndented = false,
        // Keys sorting by custom converter
        Converters = { new SortedJsonObjectConverter() }
    };

    // TODO: need tests at serialization (mediatr & masstransit)
    private IncapJson()
    {
    }

    public static IncapJson Empty => _empty;

    public required string Body { get; init; }

    public int HashCode { get; init; }

    public static ErrorOr<IncapJson> CreateOrError(
        IReadOnlyDictionary<string, object> pairs)
    {
        if (pairs.Count == 0)
        {
            return Empty;
        }

        if (!pairs.Select(x => x.Value.GetType()).All(IsSupported))
        {
            return Error.Validation(
                code: "Leaks.IncapJson.BadParamType",
                description: "Unsupported param type");
        }

        pairs = pairs.OrderBy(x => x.Key).ToDictionary();

        try
        {
            var body = JsonSerializer.Serialize(pairs, _options);
            var canonicalBody = ToCanonicalJson(body);
            var obj = new IncapJson()
            {
                Body = canonicalBody,
                HashCode = canonicalBody.GetHashCode()
            };

            return obj;
        }
        catch (JsonException)
        {
            return Error.Unexpected(
                code: "Leaks.IncapJson.UnableCreate",
                description: "Some error on build incapsulated json");
        }
    }

    public static ErrorOr<IncapJson> CreateOrError(string body)
    {
        try
        {
            var canonicalBody = ToCanonicalJson(body);
            if (canonicalBody == "{}")
            {
                return Empty;
            }

            var obj = new IncapJson()
            {
                Body = canonicalBody,
                HashCode = canonicalBody.GetHashCode()
            };

            return obj;
        }
        catch (JsonException)
        {
            return Error.Unexpected(
                code: "Leaks.IncapJson.UnableCreate",
                description: "Some error on build incapsulated json");
        }
    }

    public static IncapJson CreateOrThrow(
        IReadOnlyDictionary<string, object> pairs)
    {
        if (pairs.Count == 0)
        {
            return Empty;
        }

        if (!pairs.Select(x => x.Value.GetType()).All(IsSupported))
        {
            throw new JsonException("Unsupported param type");
        }

        pairs = pairs.OrderBy(x => x.Key).ToDictionary();

        var body = JsonSerializer.Serialize(pairs, _options);
        var canonicalBody = ToCanonicalJson(body);
        var obj = new IncapJson()
        {
            Body = canonicalBody,
            HashCode = canonicalBody.GetHashCode()
        };

        return obj;
    }

    public static IncapJson CreateOrThrow(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException(nameof(body));
        }

        var canonicalBody = ToCanonicalJson(body);
        if (canonicalBody == "{}")
        {
            return Empty;
        }

        var obj = new IncapJson()
        {
            Body = canonicalBody,
            HashCode = canonicalBody.GetHashCode()
        };

        return obj;
    }

    public bool TryGetStringValue(string path, out string value)
    {
        value = default!;

        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        string[] segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return false;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(Body);
            JsonElement currentElement = document.RootElement;

            foreach (string segment in segments)
            {
                if (currentElement.ValueKind != JsonValueKind.Object ||
                    !currentElement.TryGetProperty(segment, out currentElement))
                {
                    return false;
                }
            }

            if (currentElement.ValueKind == JsonValueKind.String)
            {
                value = currentElement.GetString()!;
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    public bool TryGetBoolValue(string path, out bool value)
    {
        value = default;

        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        string[] segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return false;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(Body);
            JsonElement currentElement = document.RootElement;

            foreach (string segment in segments)
            {
                if (currentElement.ValueKind != JsonValueKind.Object ||
                    !currentElement.TryGetProperty(segment, out currentElement))
                {
                    return false;
                }
            }

            if (currentElement.ValueKind == JsonValueKind.False ||
                currentElement.ValueKind == JsonValueKind.True)
            {
                value = currentElement.GetBoolean();
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    public JsonObject ToJsonObject()
    {
        var node = JsonNode.Parse(Body);
        return node!.AsObject();
    }

    public override int GetHashCode() => HashCode;

    public override bool Equals(object? obj)
    {
        return Equals(obj as IncapJson);
    }

    public bool Equals(IncapJson? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // Если хэши не равны, то тела точно разные
        if (HashCode != other.HashCode)
        {
            return false;
        }

        // Если хэши равны, надо уточнить
        using JsonDocument otherDoc = JsonDocument.Parse(other.Body);
        using JsonDocument thisDoc = JsonDocument.Parse(this.Body);

        JsonElement otherRoot = otherDoc.RootElement;
        JsonElement thisRoot = thisDoc.RootElement;

        return JsonElement.DeepEquals(otherRoot, thisRoot);
    }

    private static bool IsSupported(Type type) =>
        type == typeof(int) ||
        type == typeof(double) ||
        // TODO: ancessor of source generated id
        type == typeof(Guid) ||
        type == typeof(string) ||
        type == typeof(bool) ||
        type == typeof(int[]) ||
        type == typeof(DateOnly);

    private static string ToCanonicalJson(string json)
    {
        var node = JsonNode.Parse(json);

        if (node is null)
        {
            throw new JsonException("Unexpected json body");
        }

        return node.ToJsonString(_options);
    }

    // Custom converter for object keys sorting
    private sealed class SortedJsonObjectConverter : JsonConverter<JsonObject>
    {
        public override JsonObject Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            JsonNode.Parse(ref reader)!.AsObject();

        public override void Write(
            Utf8JsonWriter writer,
            JsonObject value,
            JsonSerializerOptions options)
        {
            var pairs = value
                .OrderBy(kv => kv.Key)
                .Select(kv => KeyValuePair.Create<string, JsonNode?>(kv.Key, kv.Value));
            
            var sorted = new JsonObject(pairs);

            sorted.WriteTo(writer, options);
        }
    }
}
