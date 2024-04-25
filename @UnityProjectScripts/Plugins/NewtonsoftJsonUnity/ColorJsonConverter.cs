using System;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

public class ColorJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var c = (Color)value;
        Color32 c32 = c;
        writer.WriteStartArray();
        serializer.Serialize(writer, c32.r);
        serializer.Serialize(writer, c32.g);
        serializer.Serialize(writer, c32.b);
        serializer.Serialize(writer, c32.a);
        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartArray)
            return null;

        var c32 = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        var idx = 0;
        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
        {
            byte value;
            if (reader.TokenType == JsonToken.String)
            {
                var sv = serializer.Deserialize<string>(reader);
                var byteConverter = new ByteConverter();
                value = (byte)byteConverter.ConvertFromString(sv);
            }
            else
            {
                value = serializer.Deserialize<byte>(reader);
            }

            switch (idx)
            {
                case 0:
                    c32.r = value;
                    break;
                case 1:
                    c32.g = value;
                    break;
                case 2:
                    c32.b = value;
                    break;
                case 3:
                    c32.a = value;
                    break;
            }
            idx++;
        }

        Color c = c32;
        return c;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color) || objectType == typeof(Color?);
    }
}