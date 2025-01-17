namespace Streaming.SharedKernel.Extensions;

public static class RouteExtensions
{
    /// <summary>
    /// Receive a base url and a filter object and build a query string with the filter properties that are not null.
    /// </summary>
    /// <param name="url">The base url of the endpoint</param>
    /// <param name="filter">The object to analyze and concat their attributes with the url</param>
    /// <returns>The formatted url</returns>
    public static string CreateRoute(this string url, object? filter)
    {
        if (filter is null)
        {
            return url;
        }

        var properties = filter.GetType().GetProperties();
        var queryStringParams = new List<string>();

        foreach (var property in properties)
        {
            var value = property.GetValue(filter, null);
            var propertyName = property.Name.ToLowerInvariant();

            if (value == null)
                continue;
            
            if(value is Uri uriValue)
            {
                queryStringParams.Add(
                    $"{propertyName}={Uri.EscapeDataString(uriValue.ToString() ?? string.Empty)}"
                );

                continue;
            }

            if (value is IEnumerable<string> enumerableValues)
            {
                queryStringParams.AddRange(enumerableValues.Select(enumerableValue =>
                    $"{propertyName}={Uri.EscapeDataString(enumerableValue ?? string.Empty)}"));

                continue;
            }

            // Handle DateTime formatting if needed
            var stringValue = value is DateTime dateTimeValue
                ? dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss")
                : value.ToString();

            queryStringParams.Add(
                $"{propertyName}={Uri.EscapeDataString(stringValue ?? string.Empty)}"
            );
        }

        if (queryStringParams.Count <= 0)
            return url;

        var queryString = string.Join("&", queryStringParams);

        return $"{url}?{queryString}";
    }
}