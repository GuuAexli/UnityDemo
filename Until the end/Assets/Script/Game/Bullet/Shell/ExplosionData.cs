using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="explosionData",menuName ="Explosion")]
public class ExplosionData : ScriptableObject
{
    public GameObject explosionPrefab;//‘§÷∆ÃÂ
    [Range(0,100)]public float range;//∑∂Œß
    public float damage;//…À∫¶
    public float penetration;//¥©…Ó
    public float delay;//—”≥Ÿ
    public float fear;//ø÷æÂ
    public AudioClip clip;

}
