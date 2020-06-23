using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text ScenesCount;
    public Text ChoicesCount;
    public Button CompleteButton;

    void Start()
    {
        ScenesCount.text = $"Сцен просмотрено: {GameplayStatisticss.ViewedScenes.Count} из {GameplayStatisticss.AllScenesCount}";
        ChoicesCount.text = $"Выборов сделано: {GameplayStatisticss.ChoicesCount}";

        CompleteButton.onClick.AddListener(ScreensNavigator.StartGameplay);
    }

    void Update()
    {
        UIHelpers.ReturnSelectToControl(CompleteButton);
    }
}