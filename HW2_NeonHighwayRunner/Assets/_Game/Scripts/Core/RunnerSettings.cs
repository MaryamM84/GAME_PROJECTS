using UnityEngine;
namespace NeonHighway {
[CreateAssetMenu(menuName="Neon Highway/Runner Settings")]
public class RunnerSettings : ScriptableObject {
 public float laneWidth=3.4f, segmentLength=60, roadWidth=11.2f;
 public float startSpeed=24, maxSpeed=52, acceleration=.18f;
 public float maxFuel=100, fuelDrain=2.1f, fuelPickup=32;
 public float damageCooldown=1.6f, laneChangeSpeed=14;
 public int coinScore=100;
 public float Speed(float seconds)=>Mathf.Min(maxSpeed,startSpeed+seconds*acceleration);
 public float Difficulty(float seconds)=>Mathf.Clamp01(seconds/150);
 public float RowSpacing(float seconds)=>Speed(seconds) * Mathf.Lerp(2.8f, 1.9f, Difficulty(seconds));

 public float DoubleObstacleChance(float seconds)
 {
     // Set obstacle density per metre while keeping the reaction time between rows.
     float density = Mathf.Lerp(.016f, .019f, Difficulty(seconds));
     return Mathf.Clamp01(RowSpacing(seconds) * density - 1f);
 }
}
}
