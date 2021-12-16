using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    public GameObject graphicsDropdown;
    public GameObject fullscreen;
    public GameObject resolutionDropdownGameObject;
    private TMPro.TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private int currentResolution = 0;
    
    public void Start() {
        int qualityLevel = QualitySettings.GetQualityLevel();
        graphicsDropdown.GetComponent<TMPro.TMP_Dropdown>().value = qualityLevel;
        fullscreen.GetComponent<Toggle>().isOn = Screen.fullScreen;
        resolutions = Screen.resolutions;
        resolutionDropdown = resolutionDropdownGameObject.GetComponent<TMPro.TMP_Dropdown>();
        resolutionDropdown.ClearOptions();
        List<String> options = new List<string>();
        var i = 0;
        foreach (var resolution in resolutions) {
            options.Add(resolution.width + " x " + resolution.height);
            if (resolution.width == Screen.width && resolution.height == Screen.height) {
                currentResolution = i;
            }
            i++;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }
    
    public void SetResolution(int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    public void QuitGame() {
        Application.Quit();
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("Menu");
    }
}
