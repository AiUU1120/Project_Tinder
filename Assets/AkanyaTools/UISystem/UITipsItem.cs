using System.Collections;
using FrameTools.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace AkanyaTools.UISystem
{
    public sealed class UITipsItem : MonoBehaviour
    {
        [SerializeField]
        private Image m_ImgBg;

        [SerializeField]
        private Text m_TxtInfo;

        public void Init(string info)
        {
            m_TxtInfo.text = info;
            // 显现出来
            StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            var bgColor = m_ImgBg.color;
            bgColor.a = 0;
            var textColor = m_TxtInfo.color;
            textColor.a = 0;

            m_ImgBg.color = bgColor;
            m_TxtInfo.color = textColor;
            while (bgColor.a < 1)
            {
                yield return null;
                bgColor.a += Time.deltaTime;
                textColor.a += Time.deltaTime;
                m_ImgBg.color = bgColor;
                m_TxtInfo.color = textColor;
            }
            yield return CoroutineTool.CoroutineTool.WaitForSeconds(1.5f);
            while (bgColor.a > 0)
            {
                yield return null;
                bgColor.a -= Time.deltaTime;
                textColor.a -= Time.deltaTime;
                m_ImgBg.color = bgColor;
                m_TxtInfo.color = textColor;
            }
            this.GameObjectPushPool();
        }
    }
}