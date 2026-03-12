using UnityEngine;
using System.Collections.Generic;

public enum BagTheme { Girly, Goth, FairyLight, Y2K, Sportsy }
public enum TrinketType { Metal, Sticker, Cloth, Keychain }

[System.Serializable]
public class OrderRequest
{
    public BagTheme theme;
    public string customerName;
    public string LoveText;// "Loves: metal
    public string HateText; //  Hates: bright colors"
    public List<TrinketType> lovedTrinkets;
    public List<TrinketType> hatedTrinkets;
}