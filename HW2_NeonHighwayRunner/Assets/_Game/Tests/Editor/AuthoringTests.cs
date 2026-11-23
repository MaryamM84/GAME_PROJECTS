using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using NeonHighway;
public class AuthoringTests {
 const string Root="Assets/_Game/";
 [Test] public void FourTypesInheritAbstractInteractable(){Assert.IsTrue(typeof(Interactable).IsAbstract);foreach(var type in new[]{typeof(StationaryCarObstacle),typeof(MovingCarObstacle),typeof(CoinCollectible),typeof(FuelCollectible)})Assert.IsTrue(type.IsSubclassOf(typeof(Interactable)));}
 [Test] public void DifficultyCappedAndReactionTimeFair(){var s=AssetDatabase.LoadAssetAtPath<RunnerSettings>(Root+"Settings/RunnerSettings.asset");Assert.Greater(s.Speed(100),s.Speed(0));Assert.AreEqual(s.maxSpeed,s.Speed(99999));for(int t=0;t<300;t++)Assert.GreaterOrEqual(s.RowSpacing(t)/s.Speed(t),1.89f);}
 [Test] public void ScenesAndReferencesComplete(){foreach(var name in new[]{"MainMenu","Gameplay"}){var scene=EditorSceneManager.OpenScene(Root+"Scenes/"+name+".unity");var g=UnityEngine.Object.FindFirstObjectByType<GameManager>();Assert.IsNotNull(g);foreach(var f in typeof(GameManager).GetFields().Where(f=>typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)))Assert.IsNotNull(f.GetValue(g),name+" / "+f.Name);Assert.AreEqual(5,g.pools.entries.Length);Assert.IsNotNull(g.ui.fuelFill.sprite);foreach(var go in scene.GetRootGameObjects())foreach(var t in go.GetComponentsInChildren<Transform>(true))Assert.AreEqual(0,GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject),t.name);}}
 [Test] public void NewInputBindingsAssigned(){var s=AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(Root+"Input/RunnerControls.inputactions");Assert.AreEqual(2,s.FindAction("Driving/Left").bindings.Count);Assert.AreEqual(2,s.FindAction("Driving/Right").bindings.Count);Assert.AreEqual("<Keyboard>/escape",s.FindAction("Driving/Pause").bindings[0].path);}
 [Test] public void BloomSavedAndNeonEmissive(){var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root+"Settings/NightVolume.asset");Assert.IsTrue(profile.TryGet<Bloom>(out var bloom));Assert.Greater(bloom.intensity.value,0);Assert.IsTrue(AssetDatabase.Contains(bloom));var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Neon Cyan.mat");Assert.IsTrue(mat.IsKeywordEnabled("_EMISSION"));Assert.Greater(mat.GetColor("_EmissionColor").maxColorComponent,1);}
 [Test] public void PoolPrefabsAndAudioValid(){foreach(var name in new[]{"RoadSegment","StationaryCarObstacle","MovingCarObstacle","CoinCollectible","FuelCollectible"}){var p=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/"+name+".prefab");Assert.IsNotNull(p.GetComponent<PooledObject>());Assert.IsNotNull(p.GetComponentInChildren<Collider>());}foreach(var name in new[]{"Music","Engine","Crash","Coin","Fuel"})Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<AudioClip>(Root+"Audio/"+name+".wav"));}
 [Test] public void BuildIncludesBothScenes(){CollectionAssert.AreEqual(new[]{Root+"Scenes/MainMenu.unity",Root+"Scenes/Gameplay.unity"},EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray());}
}
