using System.Collections.Generic;
using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
    [SerializeField] GameObject optionButtonPrefab;
    [SerializeField] Transform optionsContainer;

    private readonly List<AbilityOptionButton> pooledButtons = new();


    public void ShowAbilityOptions(List<AbilityChoice> choices)
    {
        gameObject.SetActive(true);

        while (pooledButtons.Count < choices.Count)
        {
            var obj = Instantiate(optionButtonPrefab, optionsContainer);
            var button = obj.GetComponent<AbilityOptionButton>();
            pooledButtons.Add(button);
        }

        for (int i = 0; i < pooledButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                pooledButtons[i].gameObject.SetActive(true);
                pooledButtons[i].Setup(choices[i]);
            }
            else
            {
                pooledButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}