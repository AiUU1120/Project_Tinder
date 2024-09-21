/*
* @Author: AiUU
* @Description: 编辑器快捷操作
* @AkanyaTech.Tinder
*/

using System.Diagnostics;
using UnityEditor;

namespace Editor.Common
{
    public sealed class EditorQuickOperation : UnityEditor.Editor
    {
        private const string unity_exe_path = "E:\\Unity Editor\\2022.3.45f1\\Editor\\Unity.exe";

        [MenuItem("Tools/Common/Restart Unity &R")]
        public static void ReStartUnity()
        {
            StartProcess(unity_exe_path);
            var args = unity_exe_path.Split('\\');
            var pro = Process.GetProcessesByName(args[^1].Split('.')[0]); //Unity
            foreach (var item in pro)
            {
                UnityEngine.Debug.Log(item.MainModule);
                item.Kill();
            }
        }

        private static void StartProcess(string applicationPath)
        {
            var po = new Process();
            po.StartInfo.FileName = applicationPath;
            po.Start();
        }
    }
}