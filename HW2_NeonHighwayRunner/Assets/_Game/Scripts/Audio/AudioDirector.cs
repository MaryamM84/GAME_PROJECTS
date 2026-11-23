using UnityEngine;
namespace NeonHighway { public class AudioDirector : MonoBehaviour {
 public AudioClip music,engine,crash,coin,fuel;public AudioSource musicSource,engineSource,effectSource;
 void Start(){musicSource.clip=music;musicSource.loop=true;musicSource.volume=.22f;musicSource.Play();engineSource.clip=engine;engineSource.loop=true;engineSource.volume=.23f;engineSource.Play();}
 public void Play(string id){effectSource.PlayOneShot(id=="Crash"?crash:id=="Coin"?coin:fuel,.65f);}
 void Update(){var g=GameManager.Instance;if(!g)return;bool playing=g.State==GameState.Playing;engineSource.mute=!playing;engineSource.pitch=Mathf.Lerp(.8f,1.9f,Mathf.InverseLerp(g.settings.startSpeed,g.settings.maxSpeed,g.Speed));musicSource.pitch=g.State==GameState.Paused?.85f:1;}
} }
