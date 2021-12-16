using UnityEngine;
using UnityEngine.UI;

public class AllSpeciesHolder : MonoBehaviour {
    [SerializeField] private GameObject speciesHolderPrefab;
    
    [SerializeField]
    private SerializableDictionary<Species, Texture> speciesIcon = new SerializableDictionary<Species, Texture>();

    [SerializeField]
    private SerializableDictionary<Species, Texture> speciesIconDesert = new SerializableDictionary<Species, Texture>();

    [SerializeField] private Texture waterLogo;
    [SerializeField] private Texture waterLogoDesert;

    [SerializeField] private Texture treeLogo;
    [SerializeField] private Texture treeLogoDesert;

    [SerializeField] private Canvas canvas;
    [SerializeField] private WorldController controller;

    void Start() {
        if (AgentSpawner.isDesert) {
            speciesIcon = speciesIconDesert;
            waterLogo = waterLogoDesert;
            treeLogo = treeLogoDesert;
        }

        var i = 0;
        foreach (var x in speciesIcon) {
            var go = Instantiate(speciesHolderPrefab,
                transform.position + new Vector3(0, Mathf.Pow(-1f, i % 2) * ((i + 1) / 2) * 120 * canvas.scaleFactor),
                speciesHolderPrefab.transform.rotation, transform).transform.GetChild(0);
            var dad = go.GetComponent<DragAndDrop>();
            dad.agentSpawner = controller.GetComponent<AgentSpawner>();
            dad.wc = controller;
            dad.canvas = canvas;
            dad.species = x.Key;
            dad.isTree = false;

            dad.GetComponent<RawImage>().texture = x.Value;
            i++;
        }

        var gow = Instantiate(speciesHolderPrefab,
            transform.position + new Vector3(0, Mathf.Pow(-1f, i % 2) * ((i + 1) / 2) * 120 * canvas.scaleFactor),
            speciesHolderPrefab.transform.rotation, transform).transform.GetChild(0);
        var dadw = gow.GetComponent<DragAndDrop>();
        dadw.agentSpawner = controller.GetComponent<AgentSpawner>();
        dadw.wc = controller;
        dadw.canvas = canvas;
        dadw.isSpecies = false;
        dadw.isTree = false;

        dadw.GetComponent<RawImage>().texture = waterLogo;
        i++;

        gow = Instantiate(speciesHolderPrefab,
            transform.position + new Vector3(0, Mathf.Pow(-1f, i % 2) * ((i + 1) / 2) * 120 * canvas.scaleFactor),
            speciesHolderPrefab.transform.rotation, transform).transform.GetChild(0);
        dadw = gow.GetComponent<DragAndDrop>();
        dadw.agentSpawner = controller.GetComponent<AgentSpawner>();
        dadw.wc = controller;
        dadw.isTree = true;
        dadw.canvas = canvas;
        dadw.GetComponent<RawImage>().texture = treeLogo;
        i++;
    }
}