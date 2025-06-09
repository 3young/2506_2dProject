using System.Collections.Generic;
using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
    [SerializeField] GameObject optionButtonPrefab;
    [SerializeField] Transform optionsContainer;

    public void ShowAbilityOptions(List<AbilityChoice> choices)
    {
        gameObject.SetActive(true);

        foreach (Transform child in optionsContainer)
        { 
            Destroy(child.gameObject); 
        }

        foreach (var choice in choices)
        {
            var obj = Instantiate(optionButtonPrefab, optionsContainer);
            var button = obj.GetComponent<AbilityOptionButton>();
            button.Setup(choice);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}