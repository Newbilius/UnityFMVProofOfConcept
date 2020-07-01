using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//todo порефакторить бы - много бойлерплейта
public class OptionsScreen : BaseGameScreen
{
    public Button SaveButton;
    public Button CancelButton;
    public ColorTextButton ResolutionButton;
    public ColorTextButton FullScreenButton;
    public ColorTextButton SubtitlesButton;
    private bool SubtitlesOn;
    private bool FullScreenOn;

    private Config.SelectedResolution[] Resolutions;
    private Config.SelectedResolution CurrentResolution => Resolutions[CurrentResolutionIndex];
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

        InitSaveButton();
        InitCancelButton();
        InitFullScreenButton();
        InitResolutionsButton();

        StartCoroutine("LeftRightButtonsPressed");
    }

    private void InitSaveButton()
    {
        SaveButton.onClick.AddListener(() =>
        {
            Config.SubtitlesOn = SubtitlesOn;
            Config.IsFullscreen = FullScreenOn;
            Config.Resolution = CurrentResolution;
            Screen.SetResolution(CurrentResolution.Width,
                CurrentResolution.Height,
                FullScreenOn);
            ScreensNavigator.CloseOptionsScreen();
        });
    }

    private void InitFullScreenButton()
    {
        FullScreenButton.onClick.AddListener(() =>
        {
            FullScreenOn = !FullScreenOn;
            SetFullscreenStateText();
        });
        SetFullscreenStateText();
    }

    private void InitCancelButton()
    {
        CancelButton.onClick.AddListener(ScreensNavigator.CloseOptionsScreen);
        SubtitlesButton.onClick.AddListener(() =>
        {
            SubtitlesOn = !SubtitlesOn;
            SetSubtitlesStateText();
        });
        SetSubtitlesStateText();
    }

    private void InitResolutionsButton()
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

        ResolutionButton.onClick.AddListener(() =>
        {
            CurrentResolutionIndex++;
            if (CurrentResolutionIndex > Resolutions.Length - 1)
                CurrentResolutionIndex = 0;
            SetResolutionText();
        });
        SetResolutionText();
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

    private IEnumerator LeftRightButtonsPressed()
    {
        while (true)
        {
            var value = InputActions.Main.LeftRightNavigation.ReadValue<Vector2>().normalized.x;
            if (Math.Abs(value) > 0)
            {
                if (EventSystem.current.currentSelectedGameObject == ResolutionButton.gameObject)
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
                }

                if (EventSystem.current.currentSelectedGameObject == FullScreenButton.gameObject)
                    FullScreenButton.onClick?.Invoke();

                if (EventSystem.current.currentSelectedGameObject == SubtitlesButton.gameObject)
                    SubtitlesButton.onClick?.Invoke();

                yield return new WaitForSeconds(0.15f);
            }
            else
                yield return null;
        }
    }

    private void SetSubtitlesStateText()
    {
        SubtitlesButton.Text = $"Субтитры: {(SubtitlesOn ? "Включены" : "Выключены")}";
    }

    private void SetFullscreenStateText()
    {
        FullScreenButton.Text = $"Полноэкранный режим: {(FullScreenOn ? "Да" : "Нет")}";
    }

    private void SetResolutionText()
    {
        ResolutionButton.Text = $"Разрешение {CurrentResolution.Width}x{CurrentResolution.Height}";
    }

    protected override void Update()
    {
        base.Update();
        if (IsActiveScreen())
            UIHelpers.ReturnSelectToControl(SubtitlesButton);
    }
}