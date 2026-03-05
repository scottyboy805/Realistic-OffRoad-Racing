using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RGSK
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneReference scene;

        void Start() => SceneLoadManager.LoadScene(scene.ScenePath);
    }
}