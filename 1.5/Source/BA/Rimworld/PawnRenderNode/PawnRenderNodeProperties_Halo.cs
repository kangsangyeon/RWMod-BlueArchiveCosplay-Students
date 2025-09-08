using System.Xml;
using Verse;
using RimWorld;

namespace BA
{
    public class PawnRenderNodeProperties_Halo : PawnRenderNodeProperties
    {
        public int northLayer = 0;

        // override 제거
        public PawnRenderNode CreateNode(Pawn pawn, PawnRenderTree tree)
        {
            return new PawnRenderNode_HaloLayer(pawn, this, tree);
        }

        // override, base 호출 제거
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.Attributes?["northLayer"] != null)
            {
                int.TryParse(xmlRoot.Attributes["northLayer"].Value, out northLayer);
            }
        }
    }
}
