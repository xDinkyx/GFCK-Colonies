using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Faction
{
    public List<GameObject> Population = new List<GameObject>();
    public List<GameObject> MilitaryUnits = new List<GameObject>();

    public int MaxPopulation = 10;
}
