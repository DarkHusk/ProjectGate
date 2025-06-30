// Made by Marcin "DarkHusk" Przybylek

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[CreateAssetMenu(fileName = "SceneTransitionBase", menuName = "Scriptable Objects/SceneTransitionBase")]
public class SceneTransitionBase : ScriptableObject
{
    [SerializeField] string sceneName;

    public void ChangeScene()
    {
        SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(o => o.GetComponent<SavePlayerState>())?.GetComponent<SavePlayerState>().SaveData();
        SceneManager.LoadScene(sceneName);
    }
}
