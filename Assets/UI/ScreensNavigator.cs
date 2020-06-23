using UnityEngine.SceneManagement;

public static class ScreensNavigator
{
    public static void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public static void StartGameplay()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}