using UnityEngine.SceneManagement;

public static class ScreensNavigator
{
    private const string OptionsKey = "Options";
    private const string GameplayMenuKey = "GameplayMenu";

    public static void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public static void GoToGameplay()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public static void GotoMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public static void OpenOptionsScreen()
    {
        SceneManager.LoadScene(OptionsKey, LoadSceneMode.Additive);
    }

    public static void CloseOptionsScreen()
    {
        SceneManager.UnloadSceneAsync(OptionsKey);
    }

    public static void OpenGameplayMenu()
    {
        SceneManager.LoadScene(GameplayMenuKey, LoadSceneMode.Additive);
    }

    public static void CloseGameplayMenu()
    {
        SceneManager.UnloadSceneAsync(GameplayMenuKey);
    }
}