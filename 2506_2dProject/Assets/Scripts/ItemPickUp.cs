using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] float pickUpRadius = .5f;
    [SerializeField] float attractRadius = 3f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] ItemType itemType;

    Transform player;

    private void Start()
    {
        player = GameManager.Instance.CurrentPlayer?.transform;
    }

    private void Update()
    {
        var player = GameManager.Instance.CurrentPlayer;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < attractRadius)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        if (distance < pickUpRadius )
        {
            ApplyEffectToPlayer(player);
            Destroy(gameObject);
        }
    }

    void ApplyEffectToPlayer(Player player)
    {
        switch (itemType)
        {
            case ItemType.Heal:

                break;

            case ItemType.Experience:

                break;

            case ItemType.Magnetic:

                break;

            case ItemType.Ban:

                break;
        }
    }
}

public enum ItemType
{
    Heal,
    Experience,
    Magnetic,
    Ban,
}
