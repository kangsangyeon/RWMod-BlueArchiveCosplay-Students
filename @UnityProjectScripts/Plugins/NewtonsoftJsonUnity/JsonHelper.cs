using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class JsonHelper
{
    private static JsonSerializerSettings s_SerializerSettings;
    private static JsonSerializerSettings s_DeserializerSettings;

    public static JsonSerializerSettings SerializerSettings
    {
        get
        {
            JsonSerializerSettings _serializerSettings = JsonHelper.s_SerializerSettings;
            if (_serializerSettings == null)
                _serializerSettings = JsonHelper.s_SerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    SerializationBinder = (ISerializationBinder)JsonSerializationBinder.Instance,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    //   Converters = (IList<JsonConverter>)new JsonConverter[1]
                    // {
                    //       (JsonConverter) new GoodsJsonConverter()
                    // }
                };
            return _serializerSettings;
        }
    }

    public static JsonSerializerSettings DeserializerSettings
    {
        get
        {
            JsonSerializerSettings _deserializerSettings = JsonHelper.s_DeserializerSettings;
            if (_deserializerSettings == null)
            {
                JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();
                _serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                _serializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
                _serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                _serializerSettings.SerializationBinder = (ISerializationBinder)JsonSerializationBinder.Instance;
                _serializerSettings.Converters = (IList<JsonConverter>)JsonHelper.SerializerSettings.Converters.ToArray<JsonConverter>();
                JsonHelper.s_DeserializerSettings = _serializerSettings;
                _deserializerSettings = _serializerSettings;
            }

            return _deserializerSettings;
        }
    }

    public static string SerializeObject(
        object _value,
        Formatting _formatting = Formatting.None,
        JsonSerializerSettings _settings = null)
    {
        return JsonConvert.SerializeObject(_value, _formatting, _settings ?? JsonHelper.SerializerSettings);
    }

    public static string SerializeObject(
        object _value,
        Type _type,
        Formatting _formatting = Formatting.None,
        JsonSerializerSettings _settings = null)
    {
        return JsonConvert.SerializeObject(_value, _type, _formatting, _settings ?? JsonHelper.SerializerSettings);
    }

    public static T DeserializeObject<T>(string _value, JsonSerializerSettings _settings = null) =>
        JsonConvert.DeserializeObject<T>(_value, _settings ?? JsonHelper.DeserializerSettings);

    [Obsolete("BSON Reader 서포트문제. BSON안사용하면 문제없음.")]
    public static T DeserializeObjectFromData<T>(object _data, JsonSerializerSettings _settings = null)
    {
        switch (_data)
        {
            case string _:
                return JsonHelper.DeserializeObject<T>(JsonHelper.RemoveLineComment((string)_data));
            case Stream _:
                using (BsonReader _reader = new BsonReader((Stream)_data, typeof(T).IsArray, DateTimeKind.Utc))
                    return JsonSerializer.Create(_settings ?? JsonHelper.DeserializerSettings).Deserialize<T>((JsonReader)_reader);
            default:
                return default(T);
        }
    }

    public static void PopulateObject(string _value, object _target, JsonSerializerSettings _settings = null) =>
        JsonConvert.PopulateObject(_value, _target, _settings ?? JsonHelper.DeserializerSettings);

    [Obsolete("BSON Reader 서포트문제. BSON안사용하면 문제없음.")]
    public static void PopulateObjectFromData(
        object _data,
        object _target,
        JsonSerializerSettings _settings = null)
    {
        if (_data is string)
            JsonConvert.PopulateObject((string)_data, _target, _settings ?? JsonHelper.DeserializerSettings);
        if (!(_data is Stream))
            return;
        using (BsonReader _reader = new BsonReader((Stream)_data, _target.GetType().IsArray, DateTimeKind.Utc))
            JsonSerializer.Create(_settings ?? JsonHelper.DeserializerSettings).Populate((JsonReader)_reader, _target);
    }

    public static string RemoveLineComment(string _str) => Regex.Replace(_str, "//(.*?)\\r?\\n", "\n", RegexOptions.Singleline);
}