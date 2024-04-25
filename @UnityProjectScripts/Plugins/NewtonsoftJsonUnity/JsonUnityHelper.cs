using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonUnityHelper
{
    private static JsonSerializerSettings s_SerializerSettings;

    public static JsonSerializerSettings SerializerSettings
    {
        get
        {
            if (s_SerializerSettings == null)
            {
                var _jsonSettings = JsonHelper.SerializerSettings;
                var _converter = _jsonSettings.Converters.ToList();
                _converter.Add(new Vector2JsonConverter());
                _converter.Add(new Vector3JsonConverter());
                _converter.Add(new ValueTuple2JsonConverter());
                _converter.Add(new ColorJsonConverter());
                _jsonSettings.Converters = _converter.ToArray();
                s_SerializerSettings = _jsonSettings;
            }

            return s_SerializerSettings;
        }
    }

    private static JsonSerializerSettings s_DeserializerSettings;

    // Deserialize/Populate 때는 DefaultValue 를 가진 Json 값을 무시하지 않고 Populdate 가 되도록
    // DefaultValueHandling.Ignore 를 사용하지 않는 이 속성을 사용한다.
    public static JsonSerializerSettings DeserializerSettings
    {
        get
        {
            if (s_DeserializerSettings == null)
            {
                var _jsonSettings = JsonHelper.DeserializerSettings;
                var _converter = _jsonSettings.Converters.ToList();
                _converter.Add(new Vector2JsonConverter());
                _converter.Add(new Vector3JsonConverter());
                _converter.Add(new ValueTuple2JsonConverter());
                _converter.Add(new ColorJsonConverter());
                _jsonSettings.Converters = _converter.ToArray();
                s_DeserializerSettings = _jsonSettings;
            }

            return s_DeserializerSettings;
        }
    }

    public static string LoadJson(object _arg)
    {
        TextAsset _data = null;
        if (_arg is string)
        {
            var _path = _arg as string;
            _data = Resources.Load(_path, typeof(TextAsset)) as TextAsset;
        }

        if (_arg is TextAsset)
            _data = _arg as TextAsset;

        return _data == null ? null : JsonHelper.RemoveLineComment(_data.text);
    }

    public static T LoadFromJson<T>(params object[] _objs) where T : class
    {
        T _result = null;

        foreach (var _obj in _objs)
        {
            var _json = LoadJson(_obj);
            if (_json == null)
            {
                Debug.LogErrorFormat("Json 파일을 읽지 못했습니다. Type: {0}", typeof(T).Name);
                return default(T);
            }

            try
            {
                if (_result == null)
                {
                    _result = JsonConvert.DeserializeObject<T>(_json, DeserializerSettings);
                }
                else
                {
                    JsonConvert.PopulateObject(_json, _result, DeserializerSettings);
                }
            }
            catch (Exception _e)
            {
                Debug.LogErrorFormat("Json 포맷을 읽는 도중 오류가 발생했습니다. Type: {0}, Message: {1}",
                    _e, typeof(T).Name, _e.Message);
                return default(T);
            }
        }

        return _result;
    }

    public static void SaveJson(string _path, object _obj)
    {
        string _jsonText = JsonConvert.SerializeObject(_obj, Formatting.Indented, SerializerSettings);
        using (var _writer = new StreamWriter(_path))
        {
            _writer.Write(_jsonText);
        }
    }
}