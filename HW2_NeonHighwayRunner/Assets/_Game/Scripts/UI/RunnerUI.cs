using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace NeonHighway { public class RunnerUI : MonoBehaviour {
 public GameObject menu,hud,pause,gameOver;public TMP_Text speed,score,high,lives,fuelLabel,finalScore;public Image fuelFill,damageFlash;float flash;
 public void Refresh(){var g=GameManager.Instance;menu.SetActive(g.State==GameState.Menu);hud.SetActive(g.State!=GameState.Menu);pause.SetActive(g.State==GameState.Paused);gameOver.SetActive(g.State==GameState.GameOver);finalScore.text=$"SCORE  {g.Score:N0}\nBEST  {g.HighScore:N0}";}
 public void Flash(){flash=.55f;}
 void Update(){var g=GameManager.Instance;if(!g)return;speed.text=$"{g.Speed*3.6f:000} <size=18>KM/H</size>";score.text=$"{g.Score:N0}";high.text=$"BEST  {Mathf.Max(g.HighScore,g.Score):N0}";lives.text=$"LIVES  {new string('|',g.Lives)}";fuelLabel.text=$"FUEL  {g.Fuel:00}%";fuelFill.fillAmount=Mathf.MoveTowards(fuelFill.fillAmount,g.Fuel/g.settings.maxFuel,Time.unscaledDeltaTime*.6f);fuelFill.color=g.Fuel<25?Color.Lerp(new Color(1,.08f,.2f),new Color(1,.6f,.1f),Mathf.PingPong(Time.unscaledTime*3,1)):new Color(.1f,1,.75f);flash=Mathf.Max(0,flash-Time.unscaledDeltaTime);damageFlash.color=new Color(1,.05f,.18f,flash*.4f);}
} }
