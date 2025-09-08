using UnityEngine;
using Verse;
using RimWorld;  // ShaderTypeDef 참조 위해 추가

namespace BA
{
    public class PawnRenderNode_HaloLayer : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_HaloLayer(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }

        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;

            if (_pawn.kindDef is not BA.PawnKindDef kindDef)
            {
                Log.Warning($"[Halo] PawnKindDef is not BA.PawnKindDef for pawn {_pawn.LabelShort}");
                return;
            }

            if (kindDef.haloDef == null)
            {
                Log.Warning($"[Halo] haloDef is null for pawnKindDef {kindDef.defName}");
                return;
            }

            if (kindDef.haloDef.noGraphic)
            {
                Log.Message($"[Halo] haloDef.noGraphic=true; skipping graphic for {kindDef.defName}");
                return;
            }

            Log.Message($"[Halo] Loading halo texture: {kindDef.haloDef.texPath} for pawnKindDef {kindDef.defName}");

            var shader = kindDef.haloDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;

            // ColorFor 메서드가 없으면 Color.white로 대체 가능
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(kindDef.haloDef.texPath, shader, Vector2.one, Color.white);
        }
    }
}
