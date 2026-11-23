using UnityEngine;
namespace NeonHighway { public class MovingCarObstacle : Interactable {
 float age; protected override void OnEnable(){base.OnEnable();age=0;}
 protected override void Tick(float dt){age+=dt;var p=transform.position;p.z=origin.z+Mathf.Sin(age*.7f)*9;p.x=origin.x+Mathf.Sin(age*1.3f)*.42f;transform.position=p;transform.rotation=Quaternion.Euler(0,Mathf.Cos(age*1.3f)*3,0);}
 protected override void Interact(GameManager game){game.Damage();}
} }

