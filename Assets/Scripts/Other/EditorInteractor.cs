using System;
using UnityEngine;

public class EditorInteractor : MonoBehaviour {
    [Tooltip("Asteroid Generator")]
    public bool asteroidSpawn;
    [Tooltip("Breakable Object")]
    public bool makeBreakable, forceBreak, forceShatter;

    void Update() {
        Attempt<AsteroidGenerator>(ref asteroidSpawn, c => c.GenerateAsteroid(false));
        Attempt<BreakableObject>(ref makeBreakable, c => c.FormatBreakable());
        Attempt<BreakableObject>(ref forceBreak, c => c.Break());
        Attempt<BreakableObject>(ref forceShatter, c => c.Shatter());
    }

    //TODO: OBJECTS NOT SHATTERING CORRECTLY AGAIN

    void Attempt<T>(ref bool b, Action<T> a) {
        if(b && GetComponent<T>() != null) {
            b = false;
            a(GetComponent<T>());
        }
    }
}