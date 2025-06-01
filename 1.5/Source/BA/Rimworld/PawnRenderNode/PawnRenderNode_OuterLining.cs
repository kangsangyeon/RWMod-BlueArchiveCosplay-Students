using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_OuterLining : Verse.PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_OuterLining(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }

        // public override GraphicMeshSet MeshSetFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_OuterLining)}::{nameof(MeshSetFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     var apparel = pawn.apparel.WornApparel.FirstOrDefault(x => x.def is BA.ApparelDef);
        //     if (apparel == null)
        //         return null;
        //     var apparelDef = (BA.ApparelDef)apparel.def;
        //     if (string.IsNullOrEmpty(apparelDef.texPathOuterLining))
        //         return null;
        //     return HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn);
        // }

        // public override Graphic GraphicFor(Pawn pawn)
        // {
        //     Log.Message($"{nameof(PawnRenderNode_OuterLining)}::{nameof(GraphicFor)} {pawn.Name}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     var apparel = pawn.apparel.WornApparel.FirstOrDefault(x =>
        //         x.def.apparel.layers.Contains(ApparelLayerDefOf.Shell) &&
        //         x.def is BA.ApparelDef);
        //     if (apparel == null)
        //         return null;
        //     if (apparel.def is not BA.ApparelDef apparelDef)
        //         return null;
        //     Log.Message($"texPathOuterLining: {apparelDef.texPathOuterLining}");
        //     if (string.IsNullOrEmpty(apparelDef.texPathOuterLining))
        //         return null;
        //     var shader = ShaderDatabase.Cutout;
        //     return GraphicDatabase.Get<Graphic_Multi>(apparelDef.texPathOuterLining, shader, Vector2.one, ColorFor(pawn));
        // }

        // protected override void EnsureMeshesInitialized()
        // {
        //     if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
        //         return;
        //     var apparel = _pawn.apparel.WornApparel.FirstOrDefault(x => x.def is BA.ApparelDef);
        //     if (apparel == null)
        //         return;
        //     var apparelDef = (BA.ApparelDef)apparel.def;
        //     if (string.IsNullOrEmpty(apparelDef.texPathOuterLining))
        //         return;
        //     this.meshSet = HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(_pawn);
        // }

        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;
            var apparel = _pawn.apparel.WornApparel.FirstOrDefault(x =>
                x.def.apparel.layers.Contains(ApparelLayerDefOf.Shell) &&
                x.def is BA.ApparelDef);
            if (apparel == null)
                return;
            if (apparel.def is not BA.ApparelDef apparelDef)
                return;
            if (string.IsNullOrEmpty(apparelDef.texPathOuterLining))
                return;
            var shader = ShaderDatabase.Cutout;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(apparelDef.texPathOuterLining, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}