using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;

public class GrassController : NotMovableAgent
{
    private double timeToReproduce;
    void Start() {
        InvokeRepeating("reproduce", 10/WorldController.TickSpeed, 10/WorldController.TickSpeed);
    }
    
    public override void drink() {
        throw new System.NotImplementedException();
    }

    public override void eat() {
        throw new System.NotImplementedException();
    }

    public override void sleep() {
        throw new System.NotImplementedException();
    }

    public override void seeAround() {
        throw new System.NotImplementedException();
    }

    public override GameObject getBestWaterPosition()
    {
        throw new NotImplementedException();
    }

    public override GameObject getBestFoodPosition()
    {
        throw new NotImplementedException();
    }

    private double getLambdaRate() {
        return 1; 
    }

    public override void reproduce()
    {
        float distSon = (float)(new Exponential(getLambdaRate()).Sample());
        float angSon = Random.Range(0f, (float)(2*Math.PI));

        Vector3 sonPos = new Vector3(transform.position.x+distSon*((float)Math.Cos(angSon)), transform.position.y, transform.position.z+distSon*((float)Math.Sin(angSon)));
        Instantiate(this.gameObject, sonPos, this.transform.rotation); 
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString(){
        return ((int) Species.Chicken).ToString();
    }
}