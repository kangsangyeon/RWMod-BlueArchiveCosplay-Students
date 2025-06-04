using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNodeWorker_Wing : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 offset = base.OffsetFor(node, parms, out pivot);
            // if (node.tree.pawn.kindDef is not BA.PawnKindDef pawnKind)
            //     return offset;
            // if (node is not BA.PawnRenderNode_Wing wingNode)
            //     return offset;
            if (node.Props is not BA.PawnRenderNodeProperties_Wing wingNodeProps)
                return offset;

            // wing 이미지 파일은 x 0.75를 몸통의 중앙으로 기준하여 제작되었음.
            // 잘은 모르겠으나, 0.4정도 왼쪽으로 이동하도록 offset을 줘야 의도한 대로 표시됨.
            if (parms.facing == Rot4.North)
            {
                // north를 바라보면 날개를 south를 바라볼 때의 위치의 반대편에 위치시켜야 함.
                // (오른쪽 날개 기준으로) 북쪽을 바라볼 때 날개를 오른쪽으로 보내야 함.
                offset = new Vector3(.4f, 0f, 0f);
            }
            else
            {
                offset = new Vector3(-0.4f, 0f, 0f);
            }

            if (wingNodeProps.isLeft)
                offset *= -1; // 반대편(왼쪽) 날개면 offset을 뒤집음.
            if (parms.facing == Rot4.West)
                offset *= -1; // 측면을 바라볼 때 offset을 뒤집음.
            return offset;
        }

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            bool canDrawNow = base.CanDrawNow(node, parms);
            if (node.Props is not BA.PawnRenderNodeProperties_Wing wingNodeProps)
                return canDrawNow;
            // 왼쪽 날개는 측면 이동할 때 표현하지 않음.
            if (wingNodeProps.isLeft && parms.facing.IsHorizontal)
                return false;
            return canDrawNow;
        }

        // 오른쪽 날개 텍스쳐를 좌우 반전해서 왼쪽 날개를 표현하려 했음.
        // 그래서 왼쪽 날개를 돌려서 그리려고 하는데 백페이스 컬링땜에 안되는 것 같음.
        // 그냥 날개 텍스쳐는 양쪽이 동일하더라도 개별적으로 만들어줘야 하는 것으로 정함.

        // public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        // {
        //     base.AppendDrawRequests(node, parms, requests);
        //     if (node.Props is not BA.PawnRenderNodeProperties_Wing wingNodeProps)
        //         return;
        //     if (!wingNodeProps.isLeft) // 왼쪽 날개만 뒤집어줄 것임.
        //         return;
        //     // 왼쪽 날개는 측면 이동할 때 표현하지 않음.
        //     // 따라서 south 또는 north일 때만 하위 코드가 실행됨.
        //     var lastRequest = requests[requests.Count - 1];
        //     lastRequest.preDrawnComputedMatrix.m00 *= -1;
        //     lastRequest.preDrawnComputedMatrix.m11 *= -1;
        //     lastRequest.preDrawnComputedMatrix.m22 *= -1;
        //     requests[requests.Count - 1] = lastRequest;
        // }

        // public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        // {
        //     var rot = base.RotationFor(node, parms);
        //     if (node.Props is not BA.PawnRenderNodeProperties_Wing wingNodeProps)
        //         return rot;
        //     if (!wingNodeProps.isLeft) // 왼쪽 날개만 뒤집어줄 것임.
        //         return rot;
        //     // 왼쪽 날개는 측면 이동할 때 표현하지 않음.
        //     // 따라서 south 또는 north일 때만 하위 코드가 실행됨.
        //     rot *= Quaternion.AngleAxis(Time.time * 30, Vector3.forward);
        //     return rot;
        // }

        // public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        // {
        //     var scale = base.ScaleFor(node, parms);
        //     if (node.Props is not BA.PawnRenderNodeProperties_Wing wingNodeProps)
        //         return scale;
        //     if (!wingNodeProps.isLeft) // 왼쪽 날개만 뒤집어줄 것임.
        //         return scale;
        //     // 왼쪽 날개는 측면 이동할 때 표현하지 않음.
        //     // 따라서 south 또는 north일 때만 하위 코드가 실행됨.
        //     scale.x *= -1;
        //     return scale;
        // }
    }
}