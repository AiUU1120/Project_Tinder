/*
 * @Author: AiUU
 * @Description: 协程工具 避免GC
 * @AkanyaTech.CoroutineTool
 */

using System.Collections;
using UnityEngine;

namespace AkanyaTools.CoroutineTool
{
    public static class CoroutineTool
    {
        private struct WaitForFrameStruct : IEnumerator
        {
            public object Current => null;

            public bool MoveNext() => false;

            public void Reset()
            {
            }
        }

        private static WaitForEndOfFrame s_WaitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate s_WaitForFixedUpdate = new WaitForFixedUpdate();

        public static WaitForEndOfFrame WaitForEndOfFrame() => s_WaitForEndOfFrame;

        public static WaitForFixedUpdate WaitForFixedUpdate() => s_WaitForFixedUpdate;

        public static IEnumerator WaitForSeconds(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.deltaTime;
                yield return new WaitForFrameStruct();
            }
        }

        public static IEnumerator WaitForSecondsRealtime(float time)
        {
            float currTime = 0;
            while (currTime < time)
            {
                currTime += Time.unscaledDeltaTime;
                yield return new WaitForFrameStruct();
            }
        }

        public static IEnumerator WaitForFrame()
        {
            yield return new WaitForFrameStruct();
        }

        public static IEnumerator WaitForFrames(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new WaitForFrameStruct();
            }
        }
    }
}