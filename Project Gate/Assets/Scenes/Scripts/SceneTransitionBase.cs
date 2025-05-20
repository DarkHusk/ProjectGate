// Made by Marcin "DarkHusk" Przybylek

using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneTransitionBase", menuName = "Scriptable Objects/SceneTransitionBase")]
public class SceneTransitionBase : ScriptableObject
{
    [SerializeField] string sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
