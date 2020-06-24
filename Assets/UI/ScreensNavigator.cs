using UnityEngine.SceneManagement;

//todo имена привести к идентичному стилю
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

    public static void GotoMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public static void OpenOptionsScreen()
    {
        SceneManager.LoadScene("Options", LoadSceneMode.Additive);
    }

    public static void CloseOptionsScreen()
    {
        SceneManager.UnloadSceneAsync("Options");
    }

    public static void OpenGameplayMenuScreen()
    {
        SceneManager.LoadScene("GameplayMenu", LoadSceneMode.Additive);
    }

    public static void CloseGameplayMenuScreen()
    {
        SceneManager.UnloadSceneAsync("GameplayMenu");
    }
}