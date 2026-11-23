using UnityEngine;
namespace NeonHighway { public class FuelCollectible : Interactable {
 protected override void Tick(float dt){transform.Rotate(0,45*dt,0,Space.World);}
 protected override void Interact(GameManager game){game.AddFuel(transform.position);}
} }
