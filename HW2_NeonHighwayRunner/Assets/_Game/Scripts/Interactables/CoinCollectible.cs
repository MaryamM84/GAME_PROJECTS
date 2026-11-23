using UnityEngine;
namespace NeonHighway { public class CoinCollectible : Interactable {
 protected override void Tick(float dt){transform.Rotate(0,120*dt,0,Space.World);var p=transform.position;p.y=origin.y+Mathf.Sin(Time.time*3+origin.z)*.15f;transform.position=p;}
 protected override void Interact(GameManager game){game.AddCoin(transform.position);}
} }
