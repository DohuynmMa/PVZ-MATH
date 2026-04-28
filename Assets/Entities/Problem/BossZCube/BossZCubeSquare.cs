using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZCubeSquare : MonoBehaviour
{
    public Vector3 randomDirection;
    private void Start()
    {
        randomDirection = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
        transform.localScale = new Vector3(1,1,1) * Random.Range(0.3f,1.8f);
        if(GetComponent<Animator>() != null) GetComponent<Animator>().speed = Random.Range(0.5f, 1.5f);
    }
    private void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.Rotate(Time.deltaTime * randomDirection.x,Time.deltaTime * randomDirection.y, Time.deltaTime * randomDirection.z, Space.Self);
    }
}
