using System;
using System.Collections.Generic;
using UnityEngine;
namespace NeonHighway {
[Serializable] public class PoolEntry { public string key; public GameObject prefab; public int capacity=24; }
public class PoolManager : MonoBehaviour {
 public PoolEntry[] entries;
 readonly Dictionary<string,Queue<PooledObject>> free=new();
 readonly Dictionary<string,PoolEntry> definitions=new();
 readonly HashSet<PooledObject> active=new();
 public int CreatedCount {get; private set;}
 public int ActiveCount=>active.Count;
 void Awake(){ foreach(var e in entries){ free[e.key]=new(); definitions[e.key]=e; for(int i=0;i<e.capacity;i++) Create(e); } }
 PooledObject Create(PoolEntry e){ var g=Instantiate(e.prefab,transform); var p=g.GetComponent<PooledObject>(); p.owner=this;p.key=e.key;g.SetActive(false);free[e.key].Enqueue(p);CreatedCount++;return p; }
 public GameObject Rent(string key,Vector3 position){ if(free[key].Count==0)Create(definitions[key]);var p=free[key].Dequeue();p.transform.SetPositionAndRotation(position,Quaternion.identity);active.Add(p);p.gameObject.SetActive(true);return p.gameObject; }
 public void Return(PooledObject p){ if(!active.Remove(p))return;p.gameObject.SetActive(false);p.transform.SetParent(transform);free[p.key].Enqueue(p); }
 public void ShiftOrigin(float offset){foreach(var p in active){p.transform.position-=Vector3.forward*offset;var item=p.GetComponent<Interactable>();if(item)item.ShiftOrigin(offset);}}
 public void ReturnAll(){ foreach(var p in new List<PooledObject>(active))Return(p); }
}
}

