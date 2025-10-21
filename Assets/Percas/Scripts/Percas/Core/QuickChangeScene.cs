#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Percas
{
    public class QuickChangeScene : Editor
    {
        [MenuItem("Open Scene/Loading #1")]
        public static void OpenLoading()
        {
            OpenScene(Const.SCENE_LOADING);
        }

        [MenuItem("Open Scene/Home #2")]
        public static void OpenHome()
        {
            OpenScene(Const.SCENE_HOME);
        }

        [MenuItem("Open Scene/Game #3")]
        public static void OpenGame()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene($"Assets/ThreadPuzzle/Scenes/{Const.SCENE_GAME}.unity");
            }
        }

        [MenuItem("Open Scene/Game Test #4")]
        public static void OpenGameTest()
        {
            OpenScene(Const.SCENE_GAME_TEST);
        }

        private static void OpenScene(string sceneName)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Percas/Scenes/" + sceneName + ".unity");
            }
        }
    }
}
#endif
