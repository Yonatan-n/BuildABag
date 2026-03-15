using UnityEngine;


[CreateAssetMenu(fileName = "BagModel", menuName = "Bags/Bag Model")]
public class BagModel : ScriptableObject
{
    public string bagName;
    public Sprite image;
    public BagTheme theme;
}
