using System;
using System.Numerics;
using UnityEngine;

public class PlanetDisplacer : MonoBehaviour
{
    public Transform sun;
    public Transform[] planets;
    public float startingRadius = 4f;
    public float radiusStep = 4f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            float angle = i * 40f;
            float radius = startingRadius + i * radiusStep;

            float rad = angle * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(rad);
            float z = radius * Mathf.Sin(rad);

            planets[i].position = sun.position + new UnityEngine.Vector3(x, 0, z);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
