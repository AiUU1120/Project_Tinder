/*
* @Author: AiUU
* @Description: 场景快捷跳转
* @AkanyaTech.FrameTools
*/

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor.Common
{
    public sealed class EditorSceneJumper : UnityEditor.Editor
    {
        private const string game_main_scene_path = "Assets/Static Resources/Scenes/GameMain.unity";

        [MenuItem("Tools/Scene/Goto GameMain &1")]
        public static void GotoGameMain()
        {
            var curScenePath = SceneManager.GetActiveScene().path;
            if (curScenePath == game_main_scene_path)
            {
                return;
            }
            EditorSceneManager.OpenScene(game_main_scene_path);
        }
    }
}