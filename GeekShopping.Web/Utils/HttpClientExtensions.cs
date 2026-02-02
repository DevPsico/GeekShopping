using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.Web.Utils;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() } // ✅ Conversor padrão para enums como string
    };

    public static async Task<(T? Result, HttpResponseMessage Response)> PostAsJsonWithResponseAsync<T>(
        this HttpClient client,
        string requestUri,
        object content,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(requestUri, content, cancellationToken);
        var result = await ReadContentSafeAsync<T>(response, options, cancellationToken);
        return (result, response);
    }

    public static async Task<(T? Result, HttpResponseMessage Response)> PutAsJsonWithResponseAsync<T>(
        this HttpClient client,
        string requestUri,
        object content,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(requestUri, content, cancellationToken);
        var result = await ReadContentSafeAsync<T>(response, options, cancellationToken);
        return (result, response);
    }

    public static async Task<(T? Result, HttpResponseMessage Response)> PatchAsJsonWithResponseAsync<T>(
        this HttpClient client,
        string requestUri,
        object content,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(content, options ?? DefaultJsonOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = httpContent };

        var response = await client.SendAsync(request, cancellationToken);
        var result = await ReadContentSafeAsync<T>(response, options, cancellationToken);
        return (result, response);
    }

    public static async Task<HttpResponseMessage> DeleteWithResponseAsync(
        this HttpClient client,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(requestUri, cancellationToken);
        return response;
    }

    public static async Task<T?> ReadContentSafeAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response));

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}\n{errorContent}");
        }

        var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine("Conteúdo bruto da resposta JSON:");
        Console.WriteLine(rawJson);

        try
        {
            var jsonOptions = options ?? DefaultJsonOptions;

            var result = JsonSerializer.Deserialize<T>(rawJson, jsonOptions);
            if (result is null)
                throw new JsonException($"Não foi possível desserializar o conteúdo em {typeof(T).Name}.");

            return result;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Erro ao desserializar JSON para {typeof(T).Name}: {ex.Message}\nConteúdo: {rawJson}", ex);
        }
    }
}