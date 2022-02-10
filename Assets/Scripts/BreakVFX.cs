using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakVFX : MonoBehaviour
{
    #region SINGLETON

    public static BreakVFX Instance
    {
        get;
        private set;
    }
    
    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Duplicate Singleton: "+gameObject.name+"! Removing...");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    private ParticleSystem _particles;
    private ParticleSystem.MainModule _main;
    private ParticleSystem.EmitParams _parameters;

    // Start is called before the first frame update
    private void Start()
    {
        _particles = GetComponent<ParticleSystem>();
        _main = _particles.main;
        _parameters = new ParticleSystem.EmitParams
        {
            applyShapeToPosition = true
        };
    }

    public void PlayVFX(Vector2 position, Color colorA, Color colorB)
    {
        _parameters.position = position;
        _main.startColor = new ParticleSystem.MinMaxGradient(colorA, colorB);
        _particles.Emit(_parameters, 100);
    }
}
