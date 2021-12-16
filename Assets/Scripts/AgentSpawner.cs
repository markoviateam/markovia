using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using SFB;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour, ISerializable {
    public static Boolean isDesert = false;

    private static Dictionary<Species, GameObject> speciesPrefabsStatic = new Dictionary<Species, GameObject>();
    private Dictionary<Species, List<Vector3>> NonMovableAgentsPositions = new Dictionary<Species, List<Vector3>>();
    private Dictionary<Species, HashSet<Agent>> InGameAgents = new Dictionary<Species, HashSet<Agent>>();

    public SerializableDictionary<Species, GameObject> speciesPrefabs =
        new SerializableDictionary<Species, GameObject>();

    public SerializableDictionary<Species, GameObject> speciesPrefabsDesert =
        new SerializableDictionary<Species, GameObject>();

    public GameObject treePrefab;
    public GameObject treePrefabDesert;
    public Dictionary<Species, HashSet<Agent>> gameAgents => InGameAgents;
    public GameObject popup;
    public GameObject cloud;

    private Dictionary<Species, List<int>> valueLists = new Dictionary<Species, List<int>>() {
        {Species.Chicken, new List<int>()},
        {Species.Fox, new List<int>()},
        {Species.Grass, new List<int>()}
    };

    private List<int> averageChickenSpeed = new List<int>();
    private static bool readFromFile = false;
    private static int grassQuantity, chickenQuantity, foxQuantity;

    void Awake() {
        if (isDesert) {
            speciesPrefabs = speciesPrefabsDesert;
            treePrefab = treePrefabDesert;
        }
    }

    void Start() {
        foreach (var s in speciesPrefabs)
            if (s.Value.GetComponent<NotMovableAgent>() != null)
                NonMovableAgentsPositions.Add(s.Key, new List<Vector3>());

        foreach (var s in speciesPrefabs.Keys)
            InGameAgents.Add(s, new HashSet<Agent>());

        StartCoroutine(AddDataPoint());
    }

    public static void AddSpecies(Species species, GameObject gameObject) {
        readFromFile = true;
        speciesPrefabsStatic.Add(species, gameObject);
    }

    public static void AddSpeciesQuantity(int grassQty, int chickenQty, int foxQty) {
        grassQuantity = grassQty;
        chickenQuantity = chickenQty;
        foxQuantity = foxQty;
    }

    public void SetSpecies() {
        if (readFromFile) {
            for (int i = 0; i < grassQuantity; i++) {
                speciesPrefabsStatic.TryGetValue(Species.Grass, out var selectedPrefab);
                if (selectedPrefab != null) {
                    Vector3 randomVector = new Vector3(Random.Range(-90f, 90f), 40, Random.Range(-90f, 90f));
                    Physics.Raycast(randomVector, Vector3.down, out var hit);
                    Quaternion rot = Quaternion.LookRotation(hit.normal, Vector3.forward);
                    GameObject reference = Instantiate(selectedPrefab, hit.point + hit.normal * 0.05f, rot, transform);
                    reference.GetComponent<Agent>().stats = SpeciesFactory.NewAgentStats(Species.Grass);
                    reference.GetComponent<Agent>().worldController = GetComponent<WorldController>();
                    InGameAgents.TryGetValue(Species.Grass, out var x);
                    x.Add(reference.GetComponent<Agent>());
                }
            }

            for (int i = 0; i < chickenQuantity; i++) {
                speciesPrefabsStatic.TryGetValue(Species.Chicken, out var selectedPrefab);
                if (selectedPrefab != null) {
                    Vector3 randomVector = new Vector3(Random.Range(-90f, 90f), 40, Random.Range(-90f, 90f));
                    Physics.Raycast(randomVector, Vector3.down, out var hit);
                    Debug.Log(hit.point.ToString());
                    GameObject reference = Instantiate(selectedPrefab, hit.point + hit.normal * 0.05f,
                        selectedPrefab.transform.rotation, transform);
                    reference.GetComponent<Agent>().stats = SpeciesFactory.NewAgentStats(Species.Chicken);
                    reference.GetComponent<Agent>().worldController = GetComponent<WorldController>();
                    InGameAgents.TryGetValue(Species.Chicken, out var x);
                    x.Add(reference.GetComponent<Agent>());
                }
            }

            for (int i = 0; i < foxQuantity; i++) {
                speciesPrefabsStatic.TryGetValue(Species.Fox, out var selectedPrefab);
                if (selectedPrefab != null) {
                    Vector3 randomVector = new Vector3(Random.Range(-90f, 90f), 40, Random.Range(-90f, 90f));
                    Physics.Raycast(randomVector, Vector3.down, out var hit);
                    Quaternion rot = Quaternion.LookRotation(hit.normal, Vector3.forward);
                    GameObject reference = Instantiate(selectedPrefab, hit.point + hit.normal * 0.05f, rot, transform);
                    reference.GetComponent<Agent>().stats = SpeciesFactory.NewAgentStats(Species.Fox);
                    reference.GetComponent<Agent>().worldController = GetComponent<WorldController>();
                    InGameAgents.TryGetValue(Species.Fox, out var x);
                    x.Add(reference.GetComponent<Agent>());
                }
            }
        }
    }

    public void AddSpecies(Species species, RaycastHit hit) {
        speciesPrefabs.TryGetValue(species, out var selectedPrefab);
        if (selectedPrefab != null) {
            NavMeshHit closestHit;
            Vector3 closestHitPosition = hit.point + hit.normal * 0.05f;
            if (NavMesh.SamplePosition(hit.point + hit.normal * 0.05f, out closestHit, 500, 1)) {
                closestHitPosition = closestHit.position;
            }

            GameObject reference = Instantiate(selectedPrefab, closestHitPosition, Quaternion.identity, transform);
            reference.GetComponent<Agent>().stats = SpeciesFactory.NewAgentStats(species);
            reference.GetComponent<Agent>().worldController = GetComponent<WorldController>();
            InGameAgents.TryGetValue(species, out var x);
            x.Add(reference.GetComponent<Agent>());
        }
    }

    public HashSet<Agent> GetSpecies(Species species) {
        InGameAgents.TryGetValue(species, out var ansSet);
        return ansSet;
    }

    public HashSet<Agent> GetChickens() {
        InGameAgents.TryGetValue(Species.Chicken, out var ansSet);
        return ansSet;
    }

    public void Died(Agent agent, Species species) {
        InGameAgents.TryGetValue(species, out var set);
        set.Remove(agent);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        int id = 0;
        foreach (KeyValuePair<Species, HashSet<Agent>> entry in InGameAgents) {
            foreach (Agent obj in entry.Value) {
                string idStr = id.ToString();
                info.AddValue("Species" + idStr, entry.Key);
                id++;
            }
        }
    }

    private GameData GetData() {
        GameData currentData = new GameData();
        foreach (KeyValuePair<Species, HashSet<Agent>> entry in InGameAgents) {
            foreach (Agent obj in entry.Value) {
                currentData.addAgent(obj.gameObject);
            }
        }

        Terraformation terrain;
        if (GameObject.Find("Terraformer") != null &&
            (terrain = GameObject.Find("Terraformer").GetComponent<Terraformation>()) != null) {
            currentData.addVertices(terrain.GetVertices());
            currentData.addTriangles(terrain.GetTriangles());
        }

        return currentData;
    }

    private string filePath;

    public void WriteData() {
        GameData currentData = GetData();

        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "json");
        if (string.Compare(path, string.Empty, StringComparison.Ordinal) == 0) {
            popup.GetComponentInChildren<TMPro.TMP_Text>().text = "You must enter a name";
            popup.SetActive(true);
            return;
        }

        popup.GetComponentInChildren<TMPro.TMP_Text>().text = "File is read-only!";
        bool saveToJson = JsonManager.SaveToJson(path, currentData);
        if (!path.EndsWith(".json")) {
            var newPath = path + ".json";
            File.Move(path, newPath);
            path = newPath;
        }

        filePath = path;
        if (saveToJson)
            cloud.SetActive(true);
        else
            popup.SetActive(true);
    }

    public async void SaveInServer() {
        if (filePath != null) {
            cloud.GetComponentInChildren<TMPro.TMP_Text>().text =
                "File saved successfully! Do you want to save it in the cloud?";
            await FTPManager.PostFile(filePath);
        }
    }

    private IEnumerator AddDataPoint() {
        while (true) {
            foreach (var x in valueLists) {
                InGameAgents.TryGetValue(x.Key, out var set);
                valueLists.TryGetValue(x.Key, out var list);
                if (set == null) {
                    list.Add(0);
                    continue;
                }

                list.Add(set.Count);
                if (x.Key.Equals(Species.Chicken)) {
                    double speedSum = 0;
                    foreach (var k in set) {
                        speedSum += k.GetComponent<Agent>().stats.GetAttribute(Attribute.Speed);
                    }

                    int i = (int) (speedSum * 100.0 / (1.0 * set.Count));
                    averageChickenSpeed.Add(i);
                }
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public List<int> FetchDataPoints(Species species) {
        valueLists.TryGetValue(species, out var list);
        return list;
    }

    public List<int> FetchAverageChickenSpeedList() {
        return averageChickenSpeed;
    }

    public List<int> FetchChickenSizeDataPoints() {
        var sizes = new List<int>();
        for (int i = 0; i < 10; i++)
            sizes.Add(0);
        InGameAgents.TryGetValue(Species.Chicken, out var chickSet);
        foreach (var c in chickSet) {
            int i = (int) (c.GetComponent<Agent>().stats.GetAttribute(Attribute.Size) / 0.1);
            if (i < 10) sizes[i]++;
        }

        return sizes;
    }

    public void Reproduce(Agent ag1, Agent ag2, Species species) {
        AgentStats ags = SpeciesFactory.NewAgentStats(ag1.stats, ag2.stats, species);
        ags.SetNeed(Need.ReproductiveUrge, 0f);
        speciesPrefabs.TryGetValue(species, out var selectedPrefab);
        GameObject reference = Instantiate(selectedPrefab, ag1.transform.position, ag1.transform.rotation, transform);
        reference.GetComponent<Agent>().stats = ags;
        reference.GetComponent<Agent>().worldController = ag1.worldController;
        InGameAgents.TryGetValue(species, out var x);
        x.Add(reference.GetComponent<Agent>());
    }

    private int trees = 0;
    public void AsexualReproduce(Agent ag1, Species species) {
        if (ag1.gameObject.CompareTag("Tree") && trees > 15)
            return;
        
        AgentStats ags = SpeciesFactory.NewAgentStats(ag1.stats, ag1.stats, species);
        speciesPrefabs.TryGetValue(species, out var selectedPrefab);
        Vector3 pos = ag1.transform.position;

        Vector3 randomVector = new Vector3(pos.x + Random.Range(-3f, 3f), 40, pos.z + Random.Range(-3f, 3f));
        if (ag1.gameObject.CompareTag("Tree")) 
            randomVector = new Vector3(pos.x + Random.Range(-10f, 10f), 40, pos.z + Random.Range(-10f, 10f));

        var raycastHit = Physics.Raycast(randomVector, Vector3.down, out var hit);
        if (raycastHit) {
            NavMeshHit closestHit;
            Vector3 closestHitPosition = hit.point + hit.normal * 0.05f;
            if (NavMesh.SamplePosition(hit.point + hit.normal * 0.05f, out closestHit, 500, 1)) {
                closestHitPosition = closestHit.position;
            }

            GameObject reference = Instantiate(ag1.gameObject, closestHitPosition, ag1.transform.rotation, transform);
            reference.GetComponent<Agent>().stats = ags;
            reference.GetComponent<Agent>().worldController = ag1.worldController;
            if (ag1.gameObject.CompareTag("Tree")) {
                trees++;
                return;
            }

            InGameAgents.TryGetValue(species, out var x);
            x.Add(reference.GetComponent<Agent>());
        }
    }

    public void AddTree(RaycastHit hit) {
        NavMeshHit closestHit;
        Vector3 closestHitPosition = hit.point + hit.normal * 0.05f;
        if (NavMesh.SamplePosition(hit.point + hit.normal * 0.05f, out closestHit, 500, 1)) {
            closestHitPosition = closestHit.position;
        }

        GameObject reference = Instantiate(treePrefab, closestHitPosition, Quaternion.identity, transform);
        reference.GetComponent<Agent>().stats = SpeciesFactory.NewAgentStats(Species.Grass);
        reference.GetComponent<Agent>().worldController = GetComponent<WorldController>();
    }
}