using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIHelpers
{
    //чтобы нельзя было кликнуть на пустом месте и выбрать "никакой" вариант в диалоге
    public static void ReturnSelectToControl(Selectable uiControl)
    {
        if (uiControl != null && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(uiControl.gameObject);
    }

    public static IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);

            yield return null;
        }
    }
}