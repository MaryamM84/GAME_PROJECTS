using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace NeonHighway
{
    public enum GameState
    {
        Menu,Playing,Paused,GameOver
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }
        public RunnerSettings settings;
        public InputActionAsset controls;
        public PlayerCar player;
        public RoadGenerator road;
        public PoolManager pools;
        public RunnerUI ui;
        public AudioDirector audioDirector;
        public EffectsDirector effects;
        public bool menuScene;
        public GameState State
        {
            get;
            private set;
        }
        public float Elapsed
        {
            get;
            private set;
        }
        public float Distance
        {
            get;
            private set;
        }
        public float Fuel
        {
            get;
            private set;
        }
        public int Lives
        {
            get;
            private set;
        }
        public int Score=>Mathf.FloorToInt(Distance)+bonus;
        public int HighScore
        {
            get;
            private set;
        }
        public float Speed=>settings.Speed(Elapsed);
        int bonus;
        float immunity;
        InputAction left,right,pause;
        void Awake()
        {
            if(Instance&&Instance!=this)
            {
                Destroy(gameObject);
                return;
            }
            Instance=this;
            controls=Instantiate(controls);
            Time.timeScale=1;
            HighScore=PlayerPrefs.GetInt("NeonHighway.HighScore",0);
            left=controls.FindAction("Driving/Left",true);
            right=controls.FindAction("Driving/Right",true);
            pause=controls.FindAction("Driving/Pause",true);
            controls.Enable();
            State=GameState.Menu;
            Fuel=settings.maxFuel;
            Lives=3;
        }
        void Start()
        {
            road.ResetRoad();
            if(!menuScene)StartRun();
            ui.Refresh();
        }
        void Update()
        {
            if(pause.WasPressedThisFrame())
            {
                if(State==GameState.Playing)Pause();
                else if(State==GameState.Paused)Resume();
            }
            if(State!=GameState.Playing)return;
            if(left.WasPressedThisFrame())player.ChangeLane(-1);
            if(right.WasPressedThisFrame())player.ChangeLane(1);
            Elapsed+=Time.deltaTime;
            Distance+=Speed*Time.deltaTime;
            Fuel=Mathf.Max(0,Fuel-settings.fuelDrain*Time.deltaTime);
            immunity-=Time.deltaTime;
            if(Fuel<=0)EndRun();
        }
        public void StartRun()
        {
            Time.timeScale=1;
            Elapsed=Distance=0;
            bonus=0;
            Lives=3;
            Fuel=settings.maxFuel;
            immunity=0;
            player.ResetCar();
            road.ResetRoad();
            effects.ResetEffects();
            audioDirector.effectSource.Stop();
            State=GameState.Playing;
            ui.Refresh();
        }
        public void Play()
        {
            SceneManager.LoadScene("Gameplay");
        }
        public void Pause()
        {
            if(State!=GameState.Playing)return;
            State=GameState.Paused;
            Time.timeScale=0;
            ui.Refresh();
        }
        public void Resume()
        {
            if(State!=GameState.Paused)return;
            State=GameState.Playing;
            Time.timeScale=1;
            ui.Refresh();
        }
        public void Restart()
        {
            SaveHighScore();
            StartRun();
        }
        public void MainMenu()
        {
            SaveHighScore();
            Time.timeScale=1;
            SceneManager.LoadScene("MainMenu");
        }
        public void Quit()
        {
            SaveHighScore();
            Application.Quit();
        }
        public void Damage()
        {
            if(State!=GameState.Playing||immunity>0)return;
            Lives--;
            immunity=settings.damageCooldown;
            audioDirector.Play("Crash");
            effects.Burst(player.transform.position+Vector3.up, new Color(1,.3f,.05f),48);
            player.cameraRig.Shake();
            ui.Flash();
            if(Lives<=0)EndRun();
        }
        public void AddCoin(Vector3 position)
        {
            bonus+=settings.coinScore;
            audioDirector.Play("Coin");
            effects.Burst(position,Color.yellow,18);
        }
        public void AddFuel(Vector3 position)
        {
            Fuel=Mathf.Clamp(Fuel+settings.fuelPickup,0,settings.maxFuel);
            audioDirector.Play("Fuel");
            effects.Burst(position,Color.green,28);
        }
        public void EndRun()
        {
            if(State!=GameState.Playing)return;
            SaveHighScore();
            State=GameState.GameOver;
            Time.timeScale=0;
            ui.Refresh();
        }
        public void SaveHighScore()
        {
            if(Score>HighScore)
            {
                HighScore=Score;
                PlayerPrefs.SetInt("NeonHighway.HighScore",HighScore);
                PlayerPrefs.Save();
            }
        }
        void OnApplicationQuit()
        {
            SaveHighScore();
        }
        void OnDestroy()
        {
            if(Instance==this)
            {
                controls.Disable();
                Destroy(controls);
                Instance=null;
                Time.timeScale=1;
            }
        }
    }
}
