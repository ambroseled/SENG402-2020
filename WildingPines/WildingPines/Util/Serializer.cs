using System.Text.Json;

namespace WildingPines.Util

{
    public static class Serializer
    {
        /// <summary>
        /// Serializes an object to pretty printed JSON.
        /// </summary>
        /// <typeparam name="T">The type of the object being serialized.</typeparam>
        /// <param name="o"></param>
        /// <returns>The serialized object as JSON.</returns>
        public static string ToJson<T>(T o)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(o, serializeOptions);
        }
    }
}
