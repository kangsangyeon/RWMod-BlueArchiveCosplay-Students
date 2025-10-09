using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Verse;
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
                if (!File.Exists(path))
                {
                    return 1f;
                }
                var doc = XDocument.Load(path);
                XNamespace ns = doc.Root.GetDefaultNamespace();
                var pawnKindElement = doc.Descendants(ns + "PawnKindDef")
                    .FirstOrDefault(e => e.Element(ns + "defName")?.Value == defName);
                if (pawnKindElement == null)
                {
                    return 1f;
                }
                var healthScaleElem = pawnKindElement.Element(ns + "healthScale");
                if (healthScaleElem != null && float.TryParse(healthScaleElem.Value, out float val))
                {
                    return val;
                }
            }
            catch (Exception)
            {
                // 예외 발생 시 기본값 사용
            }
            return 1f;
        }
    }
}
