// using UnityEngine;
// using Verse;
// using RimWorld;
//
// namespace BA
// {
//     public class PawnRenderNodeWorker_Face : PawnRenderNodeWorker
//     {
//         // GetMaterial 오버라이드 (부모 시그니처에 맞춰 PawnDrawParms 추가 가능성 고려, 하지만 기본적으로 pawn/node 사용)
//         protected override Material GetMaterial(PawnRenderNode node, PawnDrawParms parms)
//         {
//             Pawn pawn = parms.pawn; // Pawn 객체는 parms에서 얻어올 수 있음
//
//             if (node == null || pawn.kindDef is not BA.PawnKindDef kindDef || kindDef.faceDef == null || kindDef.faceDef.noGraphic)
//             {
//                 return base.GetMaterial(node, parms);
//             }
//
//             string baseTexPath = kindDef.faceDef.texPath;
//             string modifiedTexPath = GetModifiedTexturePath(pawn, baseTexPath);
//
//             Shader shader = kindDef.faceDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
//             Color color = node.ColorFor(pawn);
//
//             Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(modifiedTexPath, shader, Vector2.one, color);
//             return graphic.MatSingle;
//         }
//
//
//         private string GetModifiedTexturePath(Pawn pawn, string baseTexPath)
//         {
//             string stateSuffix = "";
//
//             // 고통을 제외한 상태 우선순위: 수면 > 소집 > 통상
//             if (IsSleeping(pawn))
//             {
//                 stateSuffix = "_Sleep";
//             }
//             else if (IsDrafted(pawn))
//             {
//                 stateSuffix = "_Drafted";
//             }
//             // else: 통상 상태, suffix 없음
//
//             // 고통 조건 발생시 모든 표정 위에 고통 추가
//             if (IsInPain(pawn))
//             {
//                 stateSuffix += "_Pain"; // 예: "_Sleep_Pain", "_Drafted_Pain", "_Pain"
//             }
//
//             return baseTexPath + stateSuffix;
//         }
//
//         private bool IsInPain(Pawn pawn)
//         {
//             // 고통 상태: 통증 쇼크 또는 통증 레벨이 높을 때 (예: 0.5 이상)
//             return pawn.health.InPainShock || pawn.health.hediffSet.PainTotal > 0.5f;
//         }
//
//         private bool IsSleeping(Pawn pawn)
//         {
//             // 수면 상태: 현재 자세가 누워 있거나 수면 중
//             return pawn.CurJobDef == JobDefOf.LayDown || pawn.needs.rest?.CurLevel < 0.1f; // 필요에 따라 임계값 조정
//         }
//
//         private bool IsDrafted(Pawn pawn)
//         {
//             // 소집 상태: Drafted (소집됨)
//             return pawn.Drafted;
//         }
//     }
// }