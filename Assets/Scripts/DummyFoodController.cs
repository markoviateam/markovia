using System.Collections;
using UnityEngine;

public class DummyFoodController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;
        StartCoroutine(rotate());
    }

    IEnumerator rotate()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, Space.World);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
