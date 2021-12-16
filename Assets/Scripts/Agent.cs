using System.Collections;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization;

public abstract class Agent : MonoBehaviour, IAgentController, ISerializable {
    public AgentStats stats;
    private State currentState = State.Idle;
    private State nextState = State.Idle;
    private bool finished = true;
    private bool going = false;
    public WorldController worldController;
    private bool dead = false;
    private float age = 0;

    public void Start() {
        ResetCoroutines();
    }

    public void Update() {
        age += Time.deltaTime * WorldController.TickSpeed;
        int i = 0;
        if (!dead) {
            var needs = stats.Needs.ToDictionary(entry => entry.Key, entry => entry.Value);
            needs.Remove(Need.ReproductiveUrge);
            foreach (double value in needs.Values)
                if (value == 1f || age >= 100) {
                    Die();
                }
        }
    }

    public abstract void moveTo(Vector3 to);
    public abstract void moveTo(GameObject to);
    public abstract void runTo(Vector3 to);
    public abstract void drink();
    public abstract void eat();
    public abstract void sleep();
    public abstract void reproduce();

    public abstract void seeAround();

    public float SizeWithAge() {
        return 0.2f + 0.04f * age - 0.0007083f * age * (age - 20) + 0.00000885417f * age * (age - 20) * (age - 60) -
               0.0000000885417f * age * (age - 20) * (age - 60) * (age - 80);
    }

    public abstract Species GetSpecies();

    public double GetAge() {
        return age;
    }

    public void SetAge(float age) {
        this.age = age;
    }

    public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    public abstract GameObject getBestWaterPosition();
    public abstract Agent getBestFoodPosition();
    public abstract Agent findMate();

    public void ResetCoroutines() {
        finished = true;
        StopAllCoroutines();
        StartCoroutine(GetNextState());
        StartCoroutine(SolveState());
    }

    public void BeginSolvingState() {
        finished = false;
    }

    public void FinishSolvingState() {
        finished = true;
    }

    public bool IsSolving() {
        return !finished;
    }

    public bool IsGoing() {
        return going;
    }

    public void IsThere() {
        Agent bestFood = getBestFoodPosition();
        GameObject bestWater = getBestWaterPosition();
        if (bestFood == null)
            stats.SetDistance(Distance.ToFood, 0);
        else
            stats.SetDistance(Distance.ToFood, Vector3.Distance(transform.position, bestFood.transform.position) / 20);
        if (bestWater == null)
            stats.SetDistance(Distance.ToWater, 0);
        else
            stats.SetDistance(Distance.ToWater,
                Vector3.Distance(transform.position, bestWater.transform.position) / 20);
        going = false;
    }

    public void Going() {
        going = true;
    }

    public bool IsHere(Vector3 to) {
        var distance = transform.position - to;
        return Mathf.Abs(distance.x) < 0.8f && Mathf.Abs(distance.z) < 1.8f;
    }

    public IEnumerator GetNextState() {
        do {
            nextState = stats.NextState();
            yield return new WaitForSecondsRealtime(1f / WorldController.TickSpeed);
        } while (true);
    }

    public void Die() {
        worldController.GetComponent<AgentSpawner>().Died(this, GetSpecies());
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public IEnumerator SolveState() {
        while (true) {
            if (!nextState.Equals(currentState) || finished) {
                if (!finished) {
                    ResetCoroutines();
                }

                currentState = nextState;
                StartCoroutine(currentState.SolveState(this));
            }

            yield return null;
        }
    }
}