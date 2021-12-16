using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour {
    #region TickSpeed

    public static float tickSpeed = 1.0f;
    public static float TickSpeed => tickSpeed;

    #endregion

    public SerializableDictionary<Inorganic, GameObject> inorganicPrefabs =
        new SerializableDictionary<Inorganic, GameObject>();

    public SerializableDictionary<Inorganic, GameObject> inorganicPrefabsDesert =
        new SerializableDictionary<Inorganic, GameObject>();

    private Dictionary<Inorganic, List<GameObject>> inorganicReferences = new Dictionary<Inorganic, List<GameObject>>();

    private void Start() {
        if (AgentSpawner.isDesert) 
            inorganicPrefabs = inorganicPrefabsDesert;

        foreach (var s in inorganicPrefabs)
            inorganicReferences.Add(s.Key, new List<GameObject>());
    }

    public List<GameObject> GetWaterReferences() {
        inorganicReferences.TryGetValue(Inorganic.Water, out var x);
        return x;
    }

    public void NewWater(RaycastHit hit) {
        inorganicPrefabs.TryGetValue(Inorganic.Water, out var selectedPrefab);
        NavMeshHit closestHit;
        Vector3 closestHitPosition = hit.point + hit.normal * 0.05f;
        if (NavMesh.SamplePosition(hit.point + hit.normal * 0.05f, out closestHit, 500, 1)) {
            closestHitPosition = closestHit.position;
        }

        Quaternion rot = Quaternion.LookRotation(hit.normal, Vector3.forward);
        if (AgentSpawner.isDesert)
            rot = Quaternion.identity;
        GameObject reference = Instantiate(selectedPrefab, closestHitPosition, rot, this.transform);
        inorganicReferences.TryGetValue(Inorganic.Water, out var x);
        x.Add(reference);
    }

    public void SetWaters() {
        for (int i = 0; i < 25; i++) {
            Vector3 randomVector = new Vector3(Random.Range(-90f, 90f), 40, Random.Range(-90f, 90f));
            var raycastHit = Physics.Raycast(randomVector, Vector3.down, out var hit);
            if (raycastHit)
                NewWater(hit);
        }
    }
}