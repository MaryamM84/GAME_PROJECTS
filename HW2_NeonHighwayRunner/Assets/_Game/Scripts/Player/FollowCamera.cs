using UnityEngine;
namespace NeonHighway { public class FollowCamera : MonoBehaviour {
 public Transform target;public Vector3 offset=new(0,5.6f,-10.5f);float shake;
 public void Shake(){shake=.45f;}
 public void Snap(){shake=0;if(target){transform.position=target.position+offset;transform.LookAt(target.position+new Vector3(0,1.1f,16));}}
 void LateUpdate(){if(!target)return;var g=GameManager.Instance;if(g&&g.State==GameState.Paused)return;float dt=Time.deltaTime;var desired=target.position+offset;desired.x=target.position.x*.35f;transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-10*dt));transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(target.position+new Vector3(-target.position.x*.45f,1.1f,17)-transform.position),1-Mathf.Exp(-8*dt));if(shake>0){transform.position+=Random.insideUnitSphere*(shake*.28f);shake=Mathf.Max(0,shake-dt);}}
} }
