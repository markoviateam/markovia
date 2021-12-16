using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

public class MainMenu : MonoBehaviour {
    private Dictionary<Species, GameObject> speciesPrefabs;
    private int grassQty = 0, chickenQty = 0, foxQty = 0;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject popup;

    public void PlayGame() {
        if (speciesPrefabs != null) {
            foreach (var pair in speciesPrefabs)
                AgentSpawner.AddSpecies(pair.Key, pair.Value);
        }
        AgentSpawner.AddSpeciesQuantity(grassQty, chickenQty, foxQty);
    }

    public void DesertBiome() {
        AgentSpawner.isDesert = true;
        LoadScene();
    }
    public void PlainsBiome() {
        AgentSpawner.isDesert = false;
        LoadScene();
    }

    private void LoadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Settings() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void GoBack() {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    
    private void StartFromFile(string path) {
        speciesPrefabs = new Dictionary<Species, GameObject>();
        GameObject prefab;
        GameData savedData;
        try {
            if (!JsonManager.ReadFromJson(path, out savedData)) {
                return;
            }
        }
        catch (ArgumentException) {
            popup.GetComponentInChildren<TMP_Text>().text = "Invalid json";
            ShowPopup();
            return;
        }

        JsonManager.initializationData = savedData;

        string[] agents = savedData.getAgentList().Split('\n');
        foreach (string agent in agents) {
            if (string.Compare(agent, "Grass", StringComparison.Ordinal) == 0) {
                if (grassQty == 0) {
                    prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                    speciesPrefabs.Add(Species.Grass, prefab);
                }
                grassQty++;
            }
            else if (string.Compare(agent, "Fox", StringComparison.Ordinal) == 0) {
                if (foxQty == 0) {
                    prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                    speciesPrefabs.Add(Species.Fox, prefab);
                }
                foxQty++;
            }
            else if (string.Compare(agent, "Chicken", StringComparison.Ordinal) == 0) {
                if (chickenQty == 0) {
                    prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                    speciesPrefabs.Add(Species.Chicken, prefab);
                }
                chickenQty++;
            }
        }

        PlayGame();
        // TODO: Add biome to file.
        LoadScene();
    }

    public void SaveFileIndex(int index) {
        fileIndex = index;
    }

    public async void LoadServerFile() {
        if (fileList.Count == 0)
            return;

        await FTPManager.GetFile(fileList[fileIndex]);
        var path = Application.dataPath + "/Save/tempLoad.json";
        StartFromFile(path);
    }
    
    public void LoadLocalFile() {
        var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        if (path == null || path.Length == 0 || string.Compare(path[0], string.Empty, StringComparison.Ordinal) == 0) {
            popup.GetComponentInChildren<TMP_Text>().text = "You must choose a json file";
            ShowPopup();
        }

        StartFromFile(path[0]);
    }

    private void ShowPopup() {
        popup.SetActive(true);
    }

    private TMP_Dropdown fileDropdown;
    public GameObject loadingText;
    public GameObject fileDropdownGameObject;
    private List<string> fileList = new List<string>();
    private int fileIndex = 0;

    public async void Awake() {
        popup.SetActive(false);
        loadingText.GetComponent<TMP_Text>().text = "Loading...";
        fileDropdownGameObject.GetComponent<TMP_Dropdown>().ClearOptions();
        try {
            await FTPManager.GetFilesName(fileList);
        } catch (Exception exception) {
            if (popup != null) {
                popup.GetComponentInChildren<TMP_Text>().text = "Couldn't connect to server! Check your connection.";
                ShowPopup();
            }
        }

        if (loadingText == null || fileDropdownGameObject == null)
            return;
        loadingText.GetComponent<TMP_Text>().text = "";
        fileDropdown = fileDropdownGameObject.GetComponent<TMP_Dropdown>();
        fileDropdown.ClearOptions();
        fileDropdown.AddOptions(fileList);
        fileDropdown.RefreshShownValue();
    }
}