using UnityEngine;
namespace NeonHighway {
public abstract class Interactable : MonoBehaviour {
 protected bool consumed; protected Vector3 origin;
 protected virtual void OnEnable(){consumed=false;origin=transform.position;transform.rotation=Quaternion.identity;}
 protected virtual void Update(){var gm=GameManager.Instance;if(!gm||gm.State!=GameState.Playing)return;Tick(Time.deltaTime);if(transform.position.z<gm.player.transform.position.z-22)GetComponent<PooledObject>().Release();}
 protected virtual void Tick(float dt){}
 public void ShiftOrigin(float offset){origin-=Vector3.forward*offset;}
 protected abstract void Interact(GameManager game);
 void OnTriggerEnter(Collider other){var game=GameManager.Instance;if(consumed||!game||game.State!=GameState.Playing||!other.GetComponentInParent<PlayerCar>())return;consumed=true;Interact(game);GetComponent<PooledObject>().Release();}
}
}

