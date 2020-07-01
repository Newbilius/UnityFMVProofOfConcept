using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//todo удобства пользователя ради перемудрил и накопипастил - надо бы упростить
public class OptionsScreen : BaseGameScreen
{
    public Button SaveButton;
    public Button CancelButton;
    public ColorTextButton ResolutionButton;
    public ColorTextButton FullScreenButton;
    public ColorTextButton ChangeSubtitlesToggleButton;
    private bool SubtitlesOn;
    private bool FullScreenOn;

    private Config.SelectedResolution[] Resolutions;
    private int CurrentResolutionIndex;

    public override void OnEscape()
    {
        ScreensNavigator.CloseOptionsScreen();
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        SubtitlesOn = Config.SubtitlesOn;
        FullScreenOn = Config.IsFullscreen;

        SaveButton.onClick.AddListener(() =>
        {
            Config.SubtitlesOn = SubtitlesOn;
            Config.IsFullscreen = FullScreenOn;
            Config.Resolution = Resolutions[CurrentResolutionIndex];
            Screen.SetResolution(Resolutions[CurrentResolutionIndex].Width,
                Resolutions[CurrentResolutionIndex].Height,
                FullScreenOn);
            ScreensNavigator.CloseOptionsScreen();
        });

        CancelButton.onClick.AddListener(ScreensNavigator.CloseOptionsScreen);
        ChangeSubtitlesToggleButton.onClick.AddListener(() =>
        {
            SubtitlesOn = !SubtitlesOn;
            SetSubtitlesStateText();
        });
        SetSubtitlesStateText();

        FullScreenButton.onClick.AddListener(() =>
        {
            FullScreenOn = !FullScreenOn;
            SetFullscreenStateText();
        });
        SetFullscreenStateText();

        PrepareResolutionsButton();
        ResolutionButton.onClick.AddListener(() =>
        {
            CurrentResolutionIndex++;
            if (CurrentResolutionIndex > Resolutions.Length - 1)
                CurrentResolutionIndex = 0;
            SetResolutionText();
        });
        SetResolutionText();

        StartCoroutine("ChangeResolutionButtonsPressed");
    }

    private void PrepareResolutionsButton()
    {
        Resolutions = Screen.resolutions
            .Select(resolution => new Config.SelectedResolution
            {
                Width = resolution.width,
                Height = resolution.height
            })
            .ToArray()
            .Distinct(new ResolutionEqualityComparer())
            .ToArray();

        var currentResolution = Config.Resolution ?? new Config.SelectedResolution
        {
            Width = Screen.currentResolution.width,
            Height = Screen.currentResolution.height
        };
        CurrentResolutionIndex = Array.IndexOf(Resolutions, currentResolution);
        if (CurrentResolutionIndex == -1)
            CurrentResolutionIndex = 0;
    }

    private class ResolutionEqualityComparer : IEqualityComparer<Config.SelectedResolution>
    {
        public bool Equals(Config.SelectedResolution v1, Config.SelectedResolution v2)
        {
            if (v1 == null && v2 == null)
                return true;
            if (v1 == null)
                return false;
            if (v2 == null)
                return false;

            return Math.Abs(v1.Width - v2.Width) < 0.01f
                   && Math.Abs(v1.Height - v2.Height) < 0.01f;
        }

        public int GetHashCode(Config.SelectedResolution obj)
        {
            return obj.Width * 100 + obj.Height * 1000;
        }
    }

    private IEnumerator ChangeResolutionButtonsPressed()
    {
        while (true)
        {
            var value = InputActions.Main.LeftRightNavigation.ReadValue<Vector2>().normalized.x;
            if (EventSystem.current.currentSelectedGameObject == ResolutionButton.gameObject)
            {
                if (Math.Abs(value) > 0)
                {
                    if (value > 0)
                    {
                        CurrentResolutionIndex++;
                        if (CurrentResolutionIndex > Resolutions.Length - 1)
                            CurrentResolutionIndex = 0;
                    }

                    if (value < 0)
                    {
                        CurrentResolutionIndex--;
                        if (CurrentResolutionIndex < 0)
                            CurrentResolutionIndex = Resolutions.Length - 1;
                    }

                    SetResolutionText();
                    yield return new WaitForSeconds(0.15f);
                }
                else
                    yield return null;
            }
            else if (EventSystem.current.currentSelectedGameObject == FullScreenButton.gameObject)
            {
                if (Math.Abs(value) > 0)
                {
                    FullScreenButton.onClick?.Invoke();
                    yield return new WaitForSeconds(0.20f);
                }
                else
                    yield return null;
            }
            else if (EventSystem.current.currentSelectedGameObject == ChangeSubtitlesToggleButton.gameObject)
            {
                if (Math.Abs(value) > 0)
                {
                    ChangeSubtitlesToggleButton.onClick?.Invoke();
                    yield return new WaitForSeconds(0.20f);
                }
                else
                    yield return null;
            }
            else yield return null;
        }
    }

    private void SetSubtitlesStateText()
    {
        ChangeSubtitlesToggleButton.Text = $"Субтитры: {(SubtitlesOn ? "Включены" : "Выключены")}";
    }

    private void SetFullscreenStateText()
    {
        FullScreenButton.Text = $"Полноэкранный режим: {(FullScreenOn ? "Да" : "Нет")}";
    }

    private void SetResolutionText()
    {
        var resolution = Resolutions[CurrentResolutionIndex];
        ResolutionButton.Text = $"Разрешение {resolution.Width}x{resolution.Height}";
    }

    protected override void Update()
    {
        base.Update();
        if (IsActiveScreen())
            UIHelpers.ReturnSelectToControl(ChangeSubtitlesToggleButton);
    }
}