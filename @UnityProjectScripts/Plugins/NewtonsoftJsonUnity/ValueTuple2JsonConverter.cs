using System;
using Newtonsoft.Json;

public class ValueTuple2JsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
    {
        var _objectType = _value.GetType();
        var _field1 = _objectType.GetField("Item1");
        var _field2 = _objectType.GetField("Item2");

        _writer.WriteStartArray();
        _serializer.Serialize(_writer, _field1.GetValue(_value));
        _serializer.Serialize(_writer, _field2.GetValue(_value));
        _writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
    {
        if (_reader.TokenType != JsonToken.StartArray)
            return null;

        var _genericTypes = _objectType.GenericTypeArguments;
        var _result = Activator.CreateInstance(_objectType);
        var _idx = 0;
        while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
        {
            if (_idx < 2)
            {
                var _value = _serializer.Deserialize(_reader, _genericTypes[_idx]);
                var _field = _objectType.GetField($"Item{_idx + 1}");
                _field.SetValue(_result, _value);
            }

            _idx++;
        }

        return _result;
    }

    public override bool CanConvert(Type _objectType)
    {
        return _objectType.IsGenericType &&
               _objectType.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
    }
}