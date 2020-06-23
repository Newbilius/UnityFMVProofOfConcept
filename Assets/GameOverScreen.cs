using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text ScenesCount;
    public Text ChoicesCount;
    public Button CompleteButton;

    void Start()
    {
        ScenesCount.text = $"Сцен просмотрено: {GameplayStatus.ViewedScenes.Count} из {GameplayStatus.ScenesCount}";
        ChoicesCount.text = $"Выборов сделано: {GameplayStatus.ChoicesCount}";

        CompleteButton.onClick.AddListener(() =>
        {
            //todo вынести в ScreenNavigator или типа того
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        });
    }

    void Update()
    {
        //чтобы нельзя было кликнуть на пустом месте и выбрать "никакой" вариант в диалоге
        //todo похожий код повторяется, вынести
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(CompleteButton.gameObject);
    }
}