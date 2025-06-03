using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatHPUI : MonoBehaviour
{
    [SerializeField] Image fillCatHpBar;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform followTarget;
    [SerializeField] Vector3 offset;

    private Coroutine hideCoroutine;

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    private void LateUpdate()
    {
        if(followTarget != null)
        {
            transform.position = followTarget.position + offset;

            float z = followTarget.eulerAngles.z;

            if (z > 90 && z < 270)
            {
                transform.rotation = Quaternion.Euler(0, 0, z);
                transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, z);
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }

    public void SetFollowTarget(Transform target, Vector3 offset)
    {
        followTarget = target;
        this.offset = offset;
    }

    public void UpdateHP(float currentHp, float maxHp)
    {
        fillCatHpBar.fillAmount = 1 - (currentHp / maxHp);

        Show();

        if(hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay(1.5f));
    }

    private void Show()
    {
        if(canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
}
