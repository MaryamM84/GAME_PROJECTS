using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using NeonHighway;
public class GameplayTests {
 GameManager g;Keyboard keyboard;int originalHigh;bool hadHigh;
 InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
 InputSettings.BackgroundBehavior previousBackgroundInput;
 [UnitySetUp] public IEnumerator Setup(){hadHigh=PlayerPrefs.HasKey("NeonHighway.HighScore");originalHigh=PlayerPrefs.GetInt("NeonHighway.HighScore");SceneManager.LoadScene("Gameplay");yield return null;yield return null;g=GameManager.Instance;
 previousEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
 previousBackgroundInput = InputSystem.settings.backgroundBehavior;
 InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
 InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
 keyboard=InputSystem.AddDevice<Keyboard>();}
 [UnityTearDown] public IEnumerator Cleanup(){Time.timeScale=1;if(keyboard!=null)InputSystem.RemoveDevice(keyboard);
 InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInput;
 InputSystem.settings.backgroundBehavior = previousBackgroundInput;
 g.SaveHighScore();if(hadHigh)PlayerPrefs.SetInt("NeonHighway.HighScore",originalHigh);else PlayerPrefs.DeleteKey("NeonHighway.HighScore");PlayerPrefs.Save();yield return null;}
 IEnumerator Press(Key key){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;}
 [UnityTest] public IEnumerator DrivingInputAndLaneBoundaries(){Assert.AreEqual(3,g.Lives);Assert.AreEqual(GameState.Playing,g.State);float z=g.player.transform.position.z;yield return Press(Key.A);yield return new WaitForSeconds(.4f);Assert.AreEqual(-1,g.player.Lane);Assert.Less(g.player.transform.position.x,-3);yield return Press(Key.LeftArrow);Assert.AreEqual(-1,g.player.Lane);yield return Press(Key.D);yield return Press(Key.RightArrow);yield return new WaitForSeconds(.4f);Assert.AreEqual(1,g.player.Lane);Assert.Greater(g.player.transform.position.z,z);}
 [UnityTest] public IEnumerator EscapePauseFreezesAndResumes(){yield return Press(Key.Escape);Assert.AreEqual(GameState.Paused,g.State);float score=g.Score,fuel=g.Fuel,z=g.player.transform.position.z;yield return new WaitForSecondsRealtime(.25f);Assert.AreEqual(score,g.Score);Assert.AreEqual(fuel,g.Fuel);Assert.AreEqual(z,g.player.transform.position.z);yield return Press(Key.Escape);Assert.AreEqual(GameState.Playing,g.State);yield return new WaitForSeconds(.15f);Assert.Greater(g.Score,score);}
 [UnityTest] public IEnumerator ActualObstacleCollisionAndCooldown(){var position=g.player.transform.position;var obstacle=g.pools.Rent("Stationary",position);yield return new WaitForSeconds(.1f);Assert.AreEqual(2,g.Lives);Assert.IsFalse(obstacle.activeSelf);g.Damage();Assert.AreEqual(2,g.Lives);yield return new WaitForSeconds(g.settings.damageCooldown+.05f);g.Damage();Assert.AreEqual(1,g.Lives);}
 [UnityTest] public IEnumerator CoinAndFuelTriggerAndReset(){float fuel=g.Fuel;yield return new WaitForSeconds(.3f);Assert.Less(g.Fuel,fuel);int score=g.Score;var coin=g.pools.Rent("Coin",g.player.transform.position+Vector3.up);yield return new WaitForSeconds(.08f);Assert.GreaterOrEqual(g.Score,score+g.settings.coinScore);Assert.IsFalse(coin.activeSelf);var can=g.pools.Rent("Fuel",g.player.transform.position+Vector3.up);yield return new WaitForSeconds(.08f);Assert.IsFalse(can.activeSelf);Assert.LessOrEqual(g.Fuel,g.settings.maxFuel);Assert.Greater(g.Fuel,g.settings.maxFuel-1);}
 [UnityTest] public IEnumerator RestartButtonAndGameOverPersistence(){g.AddCoin(Vector3.zero);g.EndRun();Assert.AreEqual(GameState.GameOver,g.State);Assert.IsTrue(g.ui.gameOver.activeSelf);Assert.GreaterOrEqual(PlayerPrefs.GetInt("NeonHighway.HighScore"),g.Score);var button=g.ui.gameOver.GetComponentsInChildren<Button>().First(b=>b.name=="DRIVE AGAIN");button.onClick.Invoke();yield return null;Assert.AreEqual(GameState.Playing,g.State);Assert.AreEqual(3,g.Lives);Assert.Less(g.Score,10);Assert.Greater(g.Fuel,99);}
 [UnityTest]
 public IEnumerator PauseRestartSavesRecordAndClearsRun()
 {
     for (int attempt = 0; attempt < 3; attempt++)
     {
         for (int coin = 0; coin <= g.HighScore / g.settings.coinScore; coin++)
             g.AddCoin(g.player.transform.position);
         int record = g.Score;
         g.Pause();
         var restart = g.ui.pause.GetComponentsInChildren<Button>().First(b => b.name == "RESTART");
         restart.onClick.Invoke();

         Assert.AreEqual(record, PlayerPrefs.GetInt("NeonHighway.HighScore"));
         Assert.AreEqual(0, g.Score);
         Assert.AreEqual(0, g.player.Lane);
         Assert.AreEqual(3, g.Lives);
         Assert.AreEqual(g.settings.maxFuel, g.Fuel);
         Assert.AreEqual(1, Time.timeScale);
         Assert.IsTrue(g.effects.bursts.All(burst => burst.particleCount == 0));
         yield return null;
     }
 }

 [UnityTest]
 public IEnumerator ExtendedRoadStaysContiguousAtHighSpeed()
 {
     int created = g.pools.CreatedCount;
     Time.timeScale = 8;
     float stopAt = Time.realtimeSinceStartup + 25;
     while (Time.realtimeSinceStartup < stopAt)
     {
         if (g.Fuel < 75) g.AddFuel(g.player.transform.position);
         foreach (var item in g.pools.GetComponentsInChildren<Interactable>())
             item.GetComponent<PooledObject>().Release();
         yield return null;

         var road = g.pools.GetComponentsInChildren<PooledObject>()
             .Where(item => item.key == "Road").OrderBy(item => item.transform.position.z).ToArray();
         Assert.AreEqual(9, road.Length);
         for (int i = 1; i < road.Length; i++)
             Assert.AreEqual(g.settings.segmentLength, road[i].transform.position.z - road[i - 1].transform.position.z, .01f);
     }
     Assert.Greater(g.Speed, 40);
     Assert.Greater(g.Distance, 5000);
     Assert.AreEqual(created, g.pools.CreatedCount);
     float earlyDensity = (1 + g.settings.DoubleObstacleChance(0)) / g.settings.RowSpacing(0);
     float lateDensity = (1 + g.settings.DoubleObstacleChance(g.Elapsed)) / g.settings.RowSpacing(g.Elapsed);
     Assert.Greater(lateDensity, earlyDensity);
 }

 [UnityTest] public IEnumerator FuelEmptyEndsGame(){float old=g.settings.fuelDrain;g.settings.fuelDrain=10000;yield return new WaitForSecondsRealtime(.1f);g.settings.fuelDrain=old;Assert.AreEqual(GameState.GameOver,g.State);Assert.AreEqual(0,g.Fuel);}
 [UnityTest] public IEnumerator RecyclingReusesAndResetsInstances(){int created=g.pools.CreatedCount;var coin=g.pools.Rent("Coin",new Vector3(0,1,1000));var p=coin.GetComponent<PooledObject>();p.Release();Assert.IsFalse(coin.activeSelf);for(int i=0;i<60;i++){var c=g.pools.Rent("Coin",new Vector3(0,1,1000));Assert.IsTrue(c.activeSelf);c.GetComponent<PooledObject>().Release();}Assert.AreEqual(created,g.pools.CreatedCount);float previous=g.road.transform.position.z;g.player.Teleport(new Vector3(0,.05f,650));yield return null;yield return null;Assert.AreEqual(9,g.road.ActiveSegments);Assert.IsTrue(g.pools.GetComponentsInChildren<PooledObject>().Where(x=>x.key=="Road").All(x=>x.transform.position.z+g.settings.segmentLength>=g.player.transform.position.z-26));}
 [UnityTest] public IEnumerator OriginRebaseKeepsDistanceAndRoad(){float distance=g.Distance;g.player.Teleport(new Vector3(0,.05f,5100));yield return null;yield return null;Assert.Less(g.player.transform.position.z,500);Assert.Greater(g.Distance,distance);Assert.AreEqual(9,g.road.ActiveSegments);}
 [UnityTest] public IEnumerator MainMenuButtonLoadsCorrectScene(){g.Pause();g.ui.pause.GetComponentsInChildren<Button>().First(b=>b.name=="MAIN MENU").onClick.Invoke();yield return null;yield return null;g=GameManager.Instance;Assert.AreEqual(GameState.Menu,g.State);Assert.IsTrue(g.ui.menu.activeSelf);Assert.AreEqual(1,Time.timeScale);Assert.AreEqual(1,UnityEngine.Object.FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length);}
}


