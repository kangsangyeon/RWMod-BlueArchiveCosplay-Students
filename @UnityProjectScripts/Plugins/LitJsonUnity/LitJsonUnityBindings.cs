using LitJson;
using UnityEngine;

public static class LitJsonUnityBindings
{
    private static bool registered;

    public static void Register()
    {
        if (registered) return;
        registered = true;

        JsonMapper.RegisterExporter<Vector2>(ExportVector2);
        JsonMapper.RegisterExporter<Vector3>(ExportVector3);
        JsonMapper.RegisterExporter<Vector4>(ExportVector4);
        JsonMapper.RegisterExporter<Bounds>(ExportBounds);
        JsonMapper.RegisterExporter<Color>(ExportColor);
        JsonMapper.RegisterExporter<Color32>(ExportColor32);
        JsonMapper.RegisterExporter<Ray>(ExportRay);
        JsonMapper.RegisterExporter<Rect>(ExportRect);
        JsonMapper.RegisterExporter<Quaternion>(ExportQuaternion);
    }

    private static void ExportVector2(Vector2 v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("x");
        writer.Write(v.x);
        writer.WritePropertyName("y");
        writer.Write(v.y);
        writer.WriteObjectEnd();
    }

    private static void ExportVector3(Vector3 v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("x");
        writer.Write(v.x);
        writer.WritePropertyName("y");
        writer.Write(v.y);
        writer.WritePropertyName("z");
        writer.Write(v.z);
        writer.WriteObjectEnd();
    }

    private static void ExportVector4(Vector4 v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("x");
        writer.Write(v.x);
        writer.WritePropertyName("y");
        writer.Write(v.y);
        writer.WritePropertyName("z");
        writer.Write(v.z);
        writer.WritePropertyName("w");
        writer.Write(v.w);
        writer.WriteObjectEnd();
    }

    private static void ExportBounds(Bounds v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("center");
        ExportVector3(v.center, writer);
        writer.WritePropertyName("size");
        ExportVector3(v.size, writer);
        writer.WriteObjectEnd();
    }

    private static void ExportColor(Color v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("r");
        writer.Write(v.r);
        writer.WritePropertyName("g");
        writer.Write(v.g);
        writer.WritePropertyName("b");
        writer.Write(v.b);
        writer.WritePropertyName("a");
        writer.Write(v.a);
        writer.WriteObjectEnd();
    }

    private static void ExportColor32(Color32 v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("r");
        writer.Write(v.r);
        writer.WritePropertyName("g");
        writer.Write(v.g);
        writer.WritePropertyName("b");
        writer.Write(v.b);
        writer.WritePropertyName("a");
        writer.Write(v.a);
        writer.WriteObjectEnd();
    }

    private static void ExportRay(Ray v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("origin");
        ExportVector3(v.origin, writer);
        writer.WritePropertyName("direction");
        ExportVector3(v.direction, writer);
        writer.WriteObjectEnd();
    }

    private static void ExportRect(Rect v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("xMin");
        writer.Write(v.xMin);
        writer.WritePropertyName("xMax");
        writer.Write(v.xMax);
        writer.WritePropertyName("yMin");
        writer.Write(v.yMin);
        writer.WritePropertyName("yMax");
        writer.Write(v.yMax);
        writer.WriteObjectEnd();
    }

    private static void ExportQuaternion(Quaternion v, JsonWriter writer)
    {
        writer.WriteObjectStart();
        writer.WritePropertyName("x");
        writer.Write(v.x);
        writer.WritePropertyName("y");
        writer.Write(v.y);
        writer.WritePropertyName("z");
        writer.Write(v.z);
        writer.WritePropertyName("w");
        writer.Write(v.w);
        writer.WriteObjectEnd();
    }
}