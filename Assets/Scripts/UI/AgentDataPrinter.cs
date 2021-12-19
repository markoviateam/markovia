using MathNet.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class AgentDataPrinter : MonoBehaviour {
    public GameObject agentData;
    public GameObject agentSpecies;
    public GameObject sleep;
    public GameObject hunger;
    public GameObject rep;
    public GameObject thirst;
    public GameObject age;
    [SerializeField] private Slider sleepSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider repSlider;
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Slider ageSlider;
    private Agent agent;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Time.timeScale = 1;
        } else if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                var selection = hit.transform;
                if (selection.gameObject.CompareTag("Printable") && selection != null) {
                    agent = selection.gameObject.GetComponent<Agent>();
                    DisplayData();
                }
            }
        }
    }

    public void HideData() {
        if (agent != null)
            SaveData();
        agentData.SetActive(false);
        Time.timeScale = 1;
    }

    private void DisplayData() {
        agentSpecies.GetComponent<Text>().text = agent.ToString() switch {
            "Fox" => "Secondary consumer",
            "Chicken" => "Primary consumer",
            "Grass" => "Producer",
            _ => agentSpecies.GetComponent<Text>().text
        };
        
        ageSlider.value = (float) agent.GetAge() / 100f;
        repSlider.value = (float) agent.stats.GetNeed(Need.ReproductiveUrge);
        age.GetComponent<Text>().text = "Age: " + agent.GetAge().Round(0);
        rep.GetComponent<Text>().text = "Rep. Urge: " + agent.stats.GetNeed(Need.ReproductiveUrge).Round(5);
        if (agent.ToString() != "Grass") {
            sleepSlider.value = (float) agent.stats.GetNeed(Need.Sleep);
            hungerSlider.value = (float) agent.stats.GetNeed(Need.Hunger);
            thirstSlider.value = (float) agent.stats.GetNeed(Need.Thirst);
            sleep.GetComponent<Text>().text = "Sleep: " + agent.stats.GetNeed(Need.Sleep).Round(5);
            hunger.GetComponent<Text>().text = "Hunger: " + agent.stats.GetNeed(Need.Hunger).Round(5);
            thirst.GetComponent<Text>().text = "Thirst: " + agent.stats.GetNeed(Need.Thirst).Round(5);
        } else {
            sleep.GetComponent<Text>().enabled = false;
            hunger.GetComponent<Text>().enabled = false;
            thirst.GetComponent<Text>().enabled = false;
            sleepSlider.gameObject.SetActive(false);
            hungerSlider.gameObject.SetActive(false);
            thirstSlider.gameObject.SetActive(false);
        }
        agentData.SetActive(true);
        Time.timeScale = 0;
    }

    private void SaveData() {
        float age = ageSlider.value * 100f;
        float rep = repSlider.value;
        agent.stats.SetNeed(Need.ReproductiveUrge, rep);
        agent.SetAge(age);
        if (agent.ToString() != "Grass") {
            float sleep = sleepSlider.value;
            float hunger = hungerSlider.value;
            float thirst = thirstSlider.value;
            agent.stats.SetNeed(Need.Hunger, hunger);
            agent.stats.SetNeed(Need.Thirst, thirst);
            agent.stats.SetNeed(Need.Sleep, sleep);
        } else {
            sleep.GetComponent<Text>().enabled = true;
            hunger.GetComponent<Text>().enabled = true;
            thirst.GetComponent<Text>().enabled = true;
            sleepSlider.gameObject.SetActive(true);
            hungerSlider.gameObject.SetActive(true);
            thirstSlider.gameObject.SetActive(true);
        }
    }

    public void AgeSlider(float newAge) {
        age.GetComponent<Text>().text = "Age: " + (newAge * 100f).Round(0);
    }
    public void SleepSlider(float newSleep) {
        sleep.GetComponent<Text>().text = "Sleep: " + newSleep.Round(5);
    }
    public void HungerSlider(float newHunger) {
        hunger.GetComponent<Text>().text = "Hunger: " + newHunger.Round(5);
    }
    public void ThirstSlider(float newThirst) {
        thirst.GetComponent<Text>().text = "Thirst: " + newThirst.Round(5);
    }
    public void RepSlider(float newRep) {
        rep.GetComponent<Text>().text = "Rep. Urge: " + newRep.Round(5);
    }
}