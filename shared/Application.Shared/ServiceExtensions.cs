using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application.Shared;

public static partial class ServiceExtensions
{
    public static string ToJsonString(this IConfigurationSection section)
    {
        // Рекурсивное преобразование секции в объект
        object ConvertSection(IConfigurationSection sec)
        {
            // Преобразование простых значений
            if (!sec.GetChildren().Any())
            {
                return sec.Value ?? string.Empty;
            }

            // Обработка массивов (ключи вида "0", "1", "2"...)
            if (sec.GetChildren().All(child => int.TryParse(child.Key, out _)))
            {
                var list = sec
                    .GetChildren()
                    .Select(x=>ConvertSection(x))
                    .ToList()
                    .AsReadOnly();

                // Массив строк превращаем в одну с переносами
                if (list.All(x => x is string))
                {
                    return string.Join(Environment.NewLine, list.Cast<string>());
                }

                return list;
            }

            // Обработка объекта
            var dict = new Dictionary<string, object>();
            foreach (var child in sec.GetChildren())
            {
                dict[child.Key] = ConvertSection(child);
            }
            return dict;
        }

        var data = ConvertSection(section);

        return JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                // Disable экранирование символов (include cyrrilic)
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
    }
}
