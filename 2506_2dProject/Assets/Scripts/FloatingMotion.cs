using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMotion : MonoBehaviour
{
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float floatHeight = 0.25f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
