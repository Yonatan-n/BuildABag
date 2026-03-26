using UnityEngine;


[CreateAssetMenu(fileName = "BagModel", menuName = "Bags/Bag Model")]
public class BagModel : ScriptableObject
{
    public string bagName;
    public Sprite image;
    public Sprite image2; // outline, can't edit
    public Sprite image3; // placeholder for now
    public BagTheme theme;
}
