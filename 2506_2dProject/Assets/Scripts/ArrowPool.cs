using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] int poolSize = 20;

    private Queue<Arrow> arrowPool = new Queue<Arrow>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var bullet = Instantiate(arrowPrefab, transform);
            bullet.gameObject.SetActive(false);
            arrowPool.Enqueue(bullet);
        }
        DontDestroyOnLoad(gameObject);
    }

    public Arrow GetArrow()
    {
        if (arrowPool.Count > 0)
        {
            var arrow = arrowPool.Dequeue();
            arrow.gameObject.SetActive(true);
            return arrow;
        }
        else
        {
            var arrow = Instantiate(arrowPrefab, transform);
            return arrow;
        }
    }

    public void ReturnArrow(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
        arrowPool.Enqueue(arrow);
    }
}
