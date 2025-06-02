using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] MoveToTarget prefabEneymy;
    [SerializeField] float spawnTime = 1f;
    [SerializeField] Transform target;

    IEnumerator Start()
    {
        while(true)
        {
            var obj = Instantiate(prefabEneymy);
            var x = Random.Range(-4, 4);
            var y = Random.Range(-4, 4);
            obj.transform.position = new Vector3(x, y);
            obj.Target = target;

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
