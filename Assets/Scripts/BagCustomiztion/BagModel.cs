using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BagVariant
{
    public string variantName;
    public Sprite thumbnail;
    public Sprite baseLayer;
    public Sprite accentLayer;
    public Sprite zipperLayer;
}

[CreateAssetMenu(fileName = "BagModel", menuName = "Bags/Bag Model")]
public class BagModel : ScriptableObject
{
    public string bagName;
    public Sprite thumbnail;
    public List<BagVariant> variants;
}
