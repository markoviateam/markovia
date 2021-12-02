using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using MathNet.Numerics.Distributions;
using Random = UnityEngine.Random;

public static class SpeciesFactory {

    private static readonly Dictionary<Species, SortedDictionary<Attribute, double>> spec_atts = new Dictionary<Species, SortedDictionary<Attribute, double>>() {
        { Species.Chicken, new SortedDictionary<Attribute, double>() {
            {Attribute.Speed, 0.5f},
            {Attribute.Size, 0.5f},
            {Attribute.Color, 0.5f},
        } },
        { Species.Grass, new SortedDictionary<Attribute, double>() {
            {Attribute.Size, 0.5f}
        } },
        { Species.Fox, new SortedDictionary<Attribute, double>() {
            {Attribute.Speed, 0.5f},
            {Attribute.Size, 0.5f}
        } }
    };

    private static readonly Dictionary<Species, SortedSet<Distance>> spec_distances =
        new Dictionary<Species, SortedSet<Distance>>()
        {
            {Species.Chicken, new SortedSet<Distance>() {Distance.ToWater, Distance.ToFood}},
            {Species.Grass, new SortedSet<Distance>() { }},
            {Species.Fox, new SortedSet<Distance>() {Distance.ToWater, Distance.ToFood}}
        };
    
    private static readonly Dictionary<Species, SortedSet<Need>> spec_needs = new Dictionary<Species, SortedSet<Need>>() {
        { Species.Chicken, new SortedSet<Need>() { Need.Sleep, Need.Hunger, Need.ReproductiveUrge, Need.Thirst } },
        { Species.Grass, new SortedSet<Need>() { Need.ReproductiveUrge } },
        { Species.Fox, new SortedSet<Need>() { Need.Sleep, Need.Hunger, Need.ReproductiveUrge, Need.Thirst } }
    };

    private static readonly Dictionary<Species, SortedSet<State>> spec_states = new Dictionary<Species, SortedSet<State>>() {    
        //{ Species.Chicken, new SortedSet<State>() { State.LookForFood, State.LookForWater, State.Sleep, State.Idle, State.Wander } },
        { Species.Chicken, new SortedSet<State>() { State.LookForFood, State.LookForWater, State.Sleep, State.Wander, State.Reproduce } },
        { Species.Grass, new SortedSet<State>() { State.Sleep } },
        { Species.Fox, new SortedSet<State>() { State.LookForFood, State.LookForWater, State.Sleep, State.Wander, State.Reproduce } }
    };

    private static readonly Dictionary<Species, float> spec_mutability = new Dictionary<Species, float>() {
        { Species.Chicken, 0.4f}, 
        { Species.Grass, 0.4f}, 
        { Species.Fox, 0.4f}
    };
    
    private static readonly Dictionary<Species, Matrix<double>> default_weights = new Dictionary<Species, Matrix<double>>() {
        {
            Species.Chicken, Matrix<double>.Build.DenseOfArray(new double[,]{
                {-0.1, 0, 0, 0, 0.03, 0, 0.75, 0, 0},
                {-0.1, 0, 0, 0.03, 0, 0, 0, 0, 0.75},
                {0, 0, 0, 0, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 0, 0.1, 0.1, 0.1, 0.1},
                {0, 0, 0, 0, 0, 0, 0, 1, 0}
                
            })
        }
    };
    
    public static AgentStats NewAgentStats(Species species) {
        spec_needs.TryGetValue(species, out var baseNeeds);
        spec_atts.TryGetValue(species, out var baseAtts);
        spec_distances.TryGetValue(species, out var baseDists);
        spec_states.TryGetValue(species, out var baseStates);

        // Matrix<double> weights = Matrix<double>.Build.Random(baseStates.Count, baseAtts.Count + baseNeeds.Count);
        // Matrix<double> weights = Matrix<double>.Build.Random(baseStates.Count, baseAtts.Count + baseNeeds.Count + baseDists.Count, new ContinuousUniform(0f,1f));
        default_weights.TryGetValue(species, out var weights);
        
        SortedDictionary<Need, double> needsAux = new SortedDictionary<Need, double>();
        if (baseNeeds != null)
            foreach (Need need in baseNeeds)
                needsAux.Add(need, 0);

                SortedDictionary<Distance, double> distsAux = new SortedDictionary<Distance, double>();
        if (baseDists != null)
            foreach (Distance distance in baseDists)
                distsAux.Add(distance, 0);

        SortedDictionary<Attribute, double> attsAux = new SortedDictionary<Attribute, double>();
        if (baseAtts != null)
            foreach (KeyValuePair<Attribute, double> kvp in baseAtts)
                attsAux.Add(kvp.Key, kvp.Value);
        
        return new AgentStats(attsAux, needsAux, distsAux, baseStates, weights);//aux_mat);
    }

    public static AgentStats NewAgentStats(AgentStats p1, AgentStats p2, Species species) {
        
        // Get a reference to species' States
        spec_states.TryGetValue(species, out var baseStates);

        // Fill up needs in zero 
        spec_needs.TryGetValue(species, out var baseNeeds);
        SortedDictionary<Need, double> needsAux = new SortedDictionary<Need, double>();
        if (baseNeeds != null)
            foreach (Need need in baseNeeds)
                needsAux.Add(need, 0);
        
        // Fill attributes from parents. TODO: ameliorate random distribution. Species should contain a MUTABILITY constant value in a separate dictionary 
        SortedDictionary<Attribute, double> attsAux = new SortedDictionary<Attribute, double>();
        foreach (KeyValuePair<Attribute, double> kvp in p1.Atts) {
            spec_mutability.TryGetValue(species, out var mutability);
            if (Random.Range(0f, 1f) < mutability) {
                var t = Random.Range(0f, 1f); 
                attsAux.Add(kvp.Key, t);
            } else {
                var p = Random.Range(0f, 1f);
                var mean = p * p1.GetAttribute(kvp.Key) + (1 - p) * p2.GetAttribute(kvp.Key);
                var finalAtt = Math.Min(Math.Max(Normal.Sample(mean, 0.01f), 0f), 1f) ; 
                attsAux.Add(kvp.Key, finalAtt);
            }
        }
        
        spec_distances.TryGetValue(species, out var baseDists);
        
        SortedDictionary<Distance, double> distsAux = new SortedDictionary<Distance, double>();
        if (baseDists != null)
            foreach (Distance distance in baseDists)
                distsAux.Add(distance, 0);

        // Mix-and-match reactance matrix
        return new AgentStats(attsAux, needsAux, distsAux, baseStates, null);//aux_mat);
        
    }
}