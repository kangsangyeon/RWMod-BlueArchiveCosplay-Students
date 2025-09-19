using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Verse;
using UnityEngine;

namespace BA
{
    [StaticConstructorOnStartup]
    public static class PawnKindDefLoader
    {
        static PawnKindDefLoader()
        {
            foreach (var def in DefDatabase<PawnKindDef>.AllDefs)
            {
                float scale = TryReadHealthScaleFromXml(def.defName);
                def.healthScale = scale > 0f ? scale : 1f;
                Log.Message($"[Mod] Loaded PawnKindDef '{def.defName}' healthScale: {def.healthScale}");
            }
        }

        private static float TryReadHealthScaleFromXml(string defName)
        {
            try
            {
                string path = Path.Combine(
                    GenFilePaths.ModsFolderPath,  // RimWorld 공식 모드 폴더 경로 사용
                    "RWMod-BlueArchiveCosplay-Race-master",
                    "1.5",
                    "Defs",
                    "Race",
                    "BA_PawnKind.xml");

                Log.Message($"[Mod] Loading XML path: {path}");
                if (!File.Exists(path))
                {
                    Log.Message("[Mod] XML file not found");
                    return 1f;
                }

                var doc = XDocument.Load(path);
                XNamespace ns = doc.Root.GetDefaultNamespace();

                var pawnKindElement = doc.Descendants(ns + "PawnKindDef")
                    .FirstOrDefault(e => e.Element(ns + "defName")?.Value == defName);

                if (pawnKindElement == null)
                {
                    Log.Message($"[Mod] PawnKindDef element not found for {defName}");
                    return 1f;
                }

                Log.Message($"[Mod] Found PawnKindDef element for {defName}: {pawnKindElement}");

                var healthScaleElem = pawnKindElement.Element(ns + "healthScale");
                Log.Message($"[Mod] healthScale element value: {healthScaleElem?.Value ?? "null"}");

                if (healthScaleElem != null && float.TryParse(healthScaleElem.Value, out float val))
                {
                    Log.Message($"[Mod] Parsed healthScale value: {val}");
                    return val;
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to read healthScale for {defName}: {ex}");
            }

            return 1f;
        }
    }
}
