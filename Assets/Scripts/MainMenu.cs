using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    private Dictionary<Species, GameObject> speciesPrefabs = new Dictionary<Species, GameObject>();
    
    public void PlayGame() {
        foreach (var pair in speciesPrefabs)
            AgentSpawner.AddSpecies(pair.Key, pair.Value);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AddFox() {
        if (speciesPrefabs.ContainsKey(Species.Fox)) {
            speciesPrefabs.Remove(Species.Fox);
        } else {
            GameObject prefab = (GameObject) Resources.Load("Fox", typeof(GameObject));
            speciesPrefabs.Add(Species.Fox, prefab);
            // AgentSpawner.AddSpecies(Species.Fox, prefab);
        }
    }
    
    public void AddChicken() {
        if (speciesPrefabs.ContainsKey(Species.Chicken)) {
            speciesPrefabs.Remove(Species.Chicken);
        } else {
            GameObject prefab = (GameObject) Resources.Load("Chicken", typeof(GameObject));
            speciesPrefabs.Add(Species.Chicken, prefab);
            // AgentSpawner.AddSpecies(Species.Chicken, prefab);
        }
    }
    
    public void AddGrass() {
        if (speciesPrefabs.ContainsKey(Species.Grass)) {
            speciesPrefabs.Remove(Species.Grass);
        } else {
            GameObject prefab = (GameObject) Resources.Load("Grass", typeof(GameObject));
            speciesPrefabs.Add(Species.Grass, prefab);
            // AgentSpawner.AddSpecies(Species.Grass, prefab);
        }
    }

    public void StartFile() {
        speciesPrefabs = new Dictionary<Species, GameObject>();
        GameObject prefab;
        if (!File.Exists(Application.dataPath + "/Scripts/AgentSpawnerFile.json")) 
            return;
        
        GameData savedData;
        if(!JsonManager.ReadFromJson("AgentSpawnerFile.json", out savedData))
            return; //TODO Mirar de hacer algo
        string[] agents = savedData.getAgentList().Split('\n');

        foreach(string agent in agents) {
            if(agent.CompareTo("Grass") == 0){
                prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                speciesPrefabs.Add(Species.Grass, prefab);
            }
            else if(agent.CompareTo("Fox") == 0){
                prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                speciesPrefabs.Add(Species.Fox, prefab);
            }
            else if(agent.CompareTo("Chicken") == 0){
                prefab = (GameObject) Resources.Load(agent, typeof(GameObject));
                speciesPrefabs.Add(Species.Chicken, prefab);
            }
        }
        PlayGame();
    }
        

}
