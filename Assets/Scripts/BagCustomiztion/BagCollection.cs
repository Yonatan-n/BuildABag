using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BagCollection", menuName = "Bags/Bag Collection")]
public class BagCollection : ScriptableObject
{
    public List<BagModel> bags;
}