// using System.Collections.Generic;
// using UnityEngine;

// public class OrderScorer : MonoBehaviour
// {
//     [SerializeField] List<ThemeDefinition> themeDefinitions;

//     public int ScoreOrder(OrderRequest order)
//     {
//         int score = 0;

//         // Theme match (0-50 pts)
//         if (GameManager.Instance.CurrentTheme == order.theme)
//             score += 50;

//         // Trinkets
//         foreach (var trinket in bagManager.CurrentTrinkets)
//         {
//             if (order.lovedTrinkets.Contains(trinket)) score += 10;
//             if (order.hatedTrinkets.Contains(trinket)) score -= 5;
//         }

//         // Colors
//         score += ScoreColors(order);

//         return Mathf.Clamp(score, 0, 100);
//     }

//     private int ScoreColors(OrderRequest order)
//     {
//         int colorScore = 0;
//         var theme = themeDefinitions.Find(t => t.theme == order.theme);

//         // get current bag layer colors directly from BagManager
//         var bagColors = new List<Color>
//         {
//             bagManager.Layer0Color,
//             bagManager.Layer1Color,
//             bagManager.Layer2Color
//         };

//         foreach (var color in bagColors)
//         {
//             foreach (var range in theme.lovedRanges)
//                 if (range.Contains(color)) colorScore += 5;

//             foreach (var range in theme.hatedRanges)
//                 if (range.Contains(color)) colorScore -= 3;
//         }

//         return Mathf.Clamp(colorScore, -10, 20);
//     }

//     public static string GetRating(int score) => score switch
//     {
//         >= 90 => "Perfect!",
//         >= 70 => "Great!",
//         >= 50 => "Good enough!",
//         >= 30 => "They'll live with it...",
//         _ => "Yikes!"
//     };
// }