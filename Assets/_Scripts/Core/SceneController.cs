using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public sealed class SceneController : MonoBehaviour
    {
        public void Load(string scene) => SceneManager.LoadScene(scene);
    }
}
