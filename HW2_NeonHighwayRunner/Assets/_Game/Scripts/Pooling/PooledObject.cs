using UnityEngine;
namespace NeonHighway {
public class PooledObject : MonoBehaviour {
 [System.NonSerialized] public PoolManager owner;
 [System.NonSerialized] public string key;
 public void Release(){if(owner)owner.Return(this);}
}
}
