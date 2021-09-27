using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

public static class SpeciesFactory {
    
    private static readonly Dictionary<Species, SortedDictionary<Attribute, double>> spec_atts = new Dictionary<Species, SortedDictionary<Attribute, double>>() {
        { Species.Chicken, new SortedDictionary<Attribute, double>() {
            {Attribute.Speed, 0.5f},
            {Attribute.Size, 0.5f}
        } },
        { Species.Grass, new SortedDictionary<Attribute, double>() {
            {Attribute.Size, 0.5f}
        } },
        { Species.Fox, new SortedDictionary<Attribute, double>() {
            {Attribute.Speed, 0.5f},
            {Attribute.Size, 0.5f}
        } }
    };
    
    private static readonly Dictionary<Species, SortedSet<Need>> spec_needs = new Dictionary<Species, SortedSet<Need>>() {
        { Species.Chicken, new SortedSet<Need>() { Need.Sleep, Need.Hunger, Need.ReproductiveUrge } },
        { Species.Grass, new SortedSet<Need>() { Need.ReproductiveUrge } },
        { Species.Fox, new SortedSet<Need>() { Need.Sleep, Need.Hunger, Need.ReproductiveUrge } }
    };

    private static readonly Dictionary<Species, SortedSet<State>> spec_states = new Dictionary<Species, SortedSet<State>>() {    
        { Species.Chicken, new SortedSet<State>() { State.LookForFood, State.LookForWater, State.Stealth, State.Idle } },
        { Species.Grass, new SortedSet<State>() { State.Idle } },
        { Species.Fox, new SortedSet<State>() { State.LookForFood, State.LookForWater, State.Stealth, State.Idle } }
    };
    
    private static readonly Dictionary<Species, Matrix<double>> default_weights;
    
    public static AgentStats NewAgentStats(Species species) {
        
        spec_needs.TryGetValue(species, out var aux_needs);
        spec_atts.TryGetValue(species, out var aux_att);
        spec_states.TryGetValue(species, out var aux_state);
        default_weights.TryGetValue(species, out var aux_mat);

        SortedDictionary<Need, double> needs_aux = new SortedDictionary<Need, double>();
        foreach (Need need in aux_needs) 
            needs_aux.Add(need, 0);
        
        SortedDictionary<Attribute, double> atts_aux = new SortedDictionary<Attribute, double>(); 
        foreach (KeyValuePair<Attribute, double> kvp in aux_att)
            atts_aux.Add(kvp.Key, kvp.Value);

        return new AgentStats(atts_aux, needs_aux, aux_state, aux_mat);
    }

    public static AgentStats NewAgentStats(AgentStats p1, AgentStats p2) {
        return null;
    }
}