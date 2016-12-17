using UnityEngine;
using System.Collections;

public class Sense : MonoBehaviour
{

    public Aspect.aspect aspectName = Aspect.aspect.Player;
    public float detectionRate = 1.0f;

    protected float elapsedTime = 0.0f;

    protected virtual void Initialize() { }
    protected virtual void UpdateSense() { }

    void Start()
    {
        elapsedTime = 0.0f;
        Initialize();
    }

    void Update()
    {
        UpdateSense();
    }
}