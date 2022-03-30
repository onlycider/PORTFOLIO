using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MunchProject
{
    public enum EScene
    {
        Camera,
        ProductManagingScene,
    }

    public class MunchSceneManager
    {
        public static void LoadScene(EScene scene)
        {
            SceneManager.LoadScene(scene.ToString());
        }
    }
}

