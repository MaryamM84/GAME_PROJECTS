using UnityEngine;
namespace NeonHighway { public class EffectsDirector : MonoBehaviour {
 public ParticleSystem[] bursts;public ParticleSystem launch,firework;public Transform player;int index;float timer=2,explode=-1;Vector3 sky;Color fireColor;
 public void ResetEffects()
 {
     timer = 2;
     explode = -1;
     index = 0;
     foreach (var burst in bursts) burst.Clear();
     launch.Clear();
     firework.Clear();
 }
 public void Burst(Vector3 position,Color color,int amount){var p=bursts[index++%bursts.Length];p.transform.position=position;var main=p.main;main.startColor=color;p.Emit(amount);}
 void Update(){var g=GameManager.Instance;if(!g||g.State==GameState.Paused||g.State==GameState.GameOver)return;timer-=Time.deltaTime;if(timer<=0){timer=Random.Range(4f,7f);sky=player.position+new Vector3(Random.Range(-48f,48f),Random.Range(26f,42f),Random.Range(85f,140f));fireColor=Color.HSVToRGB(Random.value,.65f,1);launch.transform.position=sky-Vector3.up*24;var main=launch.main;main.startColor=fireColor;launch.Emit(14);explode=1.2f;}if(explode>=0){explode-=Time.deltaTime;if(explode<0){firework.transform.position=sky;var main=firework.main;main.startColor=fireColor;firework.Emit(100);}}}
} }
