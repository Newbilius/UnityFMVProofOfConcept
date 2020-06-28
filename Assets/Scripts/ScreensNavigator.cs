using UnityEngine.SceneManagement;

public static class ScreensNavigator
{
    private const string OptionsKey = "Options";
    private const string GameplayMenuKey = "GameplayMenu";
    public static int ScreenNumberCounter { get; private set; } = 1;

    public static void GoToGameOver()
    {
        ScreenNumberCounter = 1;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public static void GoToGameplay()
    {
        ScreenNumberCounter = 1;
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public static void GotoMainMenu()
    {
        ScreenNumberCounter = 1;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public static void OpenOptionsScreen()
    {
        ScreenNumberCounter++;
        SceneManager.LoadScene(OptionsKey, LoadSceneMode.Additive);
    }

    public static void CloseOptionsScreen()
    {
        ScreenNumberCounter--;
        SceneManager.UnloadSceneAsync(OptionsKey);
    }

    public static void OpenGameplayMenu()
    {
        ScreenNumberCounter++;
        SceneManager.LoadScene(GameplayMenuKey, LoadSceneMode.Additive);
    }

    public static void CloseGameplayMenu()
    {
        ScreenNumberCounter--;
        SceneManager.UnloadSceneAsync(GameplayMenuKey);
    }
}