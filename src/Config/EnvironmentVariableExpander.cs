using System.Text.RegularExpressions;

namespace ChatApp.Config
{
    public static class EnvironmentVariableExpander
    {
        private static readonly Regex EnvironmentVariableRegex = new(@"\$\{([^}]+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Expands environment variables in configuration values
        /// Supports ${VAR_NAME} syntax
        /// </summary>
        public static void ExpandEnvironmentVariables(IConfiguration configuration)
        {
            var configRoot = configuration as IConfigurationRoot;
            if (configRoot == null) return;

            foreach (var provider in configRoot.Providers)
            {
                if (provider is not Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider jsonProvider)
                    continue;

                ExpandProviderValues(jsonProvider);
            }
        }

        private static void ExpandProviderValues(Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider provider)
        {
            // Use reflection to access the Data property
            var dataProperty = provider.GetType().GetProperty("Data", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (dataProperty?.GetValue(provider) is not IDictionary<string, string?> data) return;

            var keysToUpdate = new List<(string key, string value)>();

            foreach (var kvp in data)
            {
                if (kvp.Value != null && EnvironmentVariableRegex.IsMatch(kvp.Value))
                {
                    var expandedValue = EnvironmentVariableRegex.Replace(kvp.Value, match =>
                    {
                        var envVarName = match.Groups[1].Value;
                        return Environment.GetEnvironmentVariable(envVarName) ?? match.Value;
                    });

                    keysToUpdate.Add((kvp.Key, expandedValue));
                }
            }

            // Update the values
            foreach (var (key, value) in keysToUpdate)
            {
                data[key] = value;
            }
        }
    }
}
