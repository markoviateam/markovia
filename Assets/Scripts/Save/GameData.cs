using System;
using UnityEngine;

public class GameData {
    public int agentCount;
    public string agentList = string.Empty;
    public int trianglesCount;
    public string triangles = string.Empty;
    public int verticesCount;
    public string vertices = string.Empty;
    public bool isDesert;

    public void addAgent(GameObject agent) {
        var agentData = agent.GetComponent<Agent>().ToString();
        if (string.Compare(agentList, string.Empty, StringComparison.Ordinal) != 0)
            agentList += "\n";
        agentList += agentData;
        agentCount++;
    }

    public void addTriangles(int[] triangles){
        trianglesCount = triangles.Length;
        foreach(int trig in triangles){
            if (string.Compare(this.triangles, string.Empty, StringComparison.Ordinal) != 0)
                this.triangles += "\n";
            this.triangles += trig.ToString();
        }
    }

    public void getTriangles(int[] triangles){
        string[] stringTrinagles = this.triangles.Split('\n');
        for(int i=0; i < trianglesCount; i++){
            triangles[i] = int.Parse(stringTrinagles[i]);
        }
    }

    public void addVertices(Vector3[] vertices){
        verticesCount = vertices.Length;
        foreach(Vector3 vertex in vertices){
            if (string.Compare(this.vertices, string.Empty, StringComparison.Ordinal) != 0)
                this.vertices += "\n";
            this.vertices += vertex.x.ToString() + "@" + vertex.y.ToString() + "@" + vertex.z.ToString();
        }
    }

    public void getVertices(Vector3[] vertices){
       string[] stringVertices = this.vertices.Split('\n');
       for(int i = 0; i < verticesCount; i++){
           string[] stringVertex = stringVertices[i].Split('@');
           Vector3 vertex = new Vector3(float.Parse(stringVertex[0]), float.Parse(stringVertex[1]), float.Parse(stringVertex[2]));
           vertices[i] = vertex;
       }
    }

    public string getAgentList() {
        return agentList;
    }
}