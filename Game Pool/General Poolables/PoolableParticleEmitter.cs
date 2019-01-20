using SDE.GamePool;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PoolableParticleEmitter : MonoBehaviour, IPoolable
{
    public bool DisableOnCompletion;
    
    public ParticleSystem System { get; private set; }
    
    public void OnSpawned()
    {
        System.Play();
        enabled = DisableOnCompletion;
    }
    private void LateUpdate()
    {
        gameObject.SetActive(System.isPlaying);
    }

    public void OnCreated()
    {
        System = GetComponent<ParticleSystem>();
        enabled = false;
    }
}