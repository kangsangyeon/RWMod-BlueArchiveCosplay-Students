﻿using System;
using Newtonsoft.Json;
using UnityEngine;

public class Vector4JsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
    {
        var _v = (Vector4)_value;
        _writer.WriteStartArray();
        _serializer.Serialize(_writer, _v.x);
        _serializer.Serialize(_writer, _v.y);
        _serializer.Serialize(_writer, _v.z);
        _serializer.Serialize(_writer, _v.w);
        _writer.WriteEndArray();
    }

    public override object ReadJson(
        JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
    {
        if (_reader.TokenType != JsonToken.StartArray)
            return null;

        var _result = new Vector4();
        var _idx = 0;
        while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
        {
            var _value = (float)_serializer.Deserialize(_reader, typeof(float));
            if (_idx < 4)
                _result[_idx] = _value;
            _idx++;
        }

        return _result;
    }

    public override bool CanConvert(Type _objectType)
    {
        return _objectType == typeof(Vector4);
    }
}