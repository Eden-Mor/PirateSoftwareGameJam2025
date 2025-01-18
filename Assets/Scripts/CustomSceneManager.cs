using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public enum Scenes
    {
        MainMenu = 0,
        GameScene = 1,
    }

    public void LoadGameScene() 
        => LoadScene(Scenes.GameScene);

    private void LoadScene(Scenes scene) 
        => SceneManager.LoadScene(scene.ToString());
}
