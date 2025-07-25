using BlackRise.Identity.Application.Helpers;
using BlackRise.Identity.Application.Utils;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace BlackRise.Identity.Middleware;

public class LocalizedMessageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LocalizedMessageMiddleware> _logger;
    public LocalizedMessageMiddleware(RequestDelegate next, ILogger<LocalizedMessageMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        try
        {
            await _next(context);
            if (!IsJsonResponse(context.Response)) return;

            var responseBody = await ReadResponseBody(memStream);
            var modifiedBody = TryModifyJsonBody(responseBody);

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(modifiedBody, Encoding.UTF8);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static bool IsJsonResponse(HttpResponse response)
    {
        return !string.IsNullOrWhiteSpace(response.ContentType) &&
               response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> ReadResponseBody(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream).ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return body;
    }

    private string TryModifyJsonBody(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody)) return responseBody;

        try
        {
            var jToken = JToken.Parse(responseBody);
            bool changed = false;

            changed |= HandleResultToMessage(jToken);
            changed |= HandleOtpTranslation(jToken);
            changed |= LocalizeTokenFields(jToken);

            return changed ? jToken.ToString() : responseBody;
        }
        catch
        {
            return responseBody;
        }
    }

    private bool HandleResultToMessage(JToken jToken)
    {
        var result = jToken["result"];
        var message = jToken["message"];

        if ((message == null || string.IsNullOrWhiteSpace(message.ToString())) &&
            result?.Type == JTokenType.String)
        {
            var originalMsg = result.ToString();
            var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
            var key = LocalizationHelper.GetConstantKey(originalMsg);
            var englishMsg = LocalizationHelper.GetLocalizedMessageFromKey(key, new CultureInfo("en"));

            _logger.LogInformation("[Localization] (result->message) Original={0}, Localized={1}", originalMsg, localizedMsg);

            jToken["message"] = localizedMsg;
            jToken["result"] = englishMsg;
            return true;
        }

        return false;
    }

    private static bool HandleOtpTranslation(JToken jToken)
    {
        var result = jToken["result"];
        if (result?.Type != JTokenType.String) return false;

        var resultValue = result.ToString();
        var localizedPrefix = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
        if (!resultValue.StartsWith(localizedPrefix)) return false;

        var englishPrefix = LocalizationHelper.GetLocalizedMessageFromKey(
            LocalizationHelper.GetConstantKey(Constants.OtpSentSuccessfully),
            new CultureInfo("en"));

        var codePart = resultValue.Substring(localizedPrefix.Length);
        jToken["result"] = englishPrefix + codePart;
        return true;
    }

    private bool LocalizeTokenFields(JToken token)
    {
        bool changed = false;

        void Localize(JToken current)
        {
            if (current.Type == JTokenType.Object)
            {
                foreach (var property in ((JObject)current).Properties())
                {
                    changed |= LocalizeField(property);
                    Localize(property.Value);
                }
            }
            else if (current.Type == JTokenType.Array)
            {
                foreach (var item in current)
                    Localize(item);
            }
        }

        Localize(token);
        return changed;
    }

    private bool LocalizeField(JProperty property)
    {
        if (property.Value.Type == JTokenType.String &&
            (property.Name.Equals("message", StringComparison.OrdinalIgnoreCase) ||
             property.Name.Equals("error", StringComparison.OrdinalIgnoreCase)))
        {
            return LocalizeSimpleField(property);
        }

        if (property.Name.Equals("validationErrors", StringComparison.OrdinalIgnoreCase) &&
            property.Value is JArray arr)
        {
            return LocalizeValidationErrors(arr);
        }

        return false;
    }

    private bool LocalizeSimpleField(JProperty property)
    {
        var original = property.Value.ToString();
        var localized = LocalizationHelper.GetLocalizedMessageFromConstantValue(original);

        _logger.LogInformation("[Localization] {0}: Original='{1}', Localized='{2}'", property.Name, original, localized);

        if (localized != original)
        {
            property.Value = localized;
            return true;
        }

        return false;
    }

    private bool LocalizeValidationErrors(JArray arr)
    {
        bool changed = false;

        for (int i = 0; i < arr.Count; i++)
        {
            var originalMsg = arr[i]?.ToString();
            if (string.IsNullOrEmpty(originalMsg)) continue;

            var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
            _logger.LogInformation("[Localization] validationError[{0}]: Original='{1}', Localized='{2}'", i, originalMsg, localizedMsg);

            if (localizedMsg != originalMsg)
            {
                arr[i] = localizedMsg;
                changed = true;
            }
        }

        return changed;
    }

}
