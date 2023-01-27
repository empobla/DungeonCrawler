using UnityEngine;
using UnityEngine.SceneManagement;

public class GUI : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
