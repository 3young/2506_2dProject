using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOptionButton : MonoBehaviour
{
    [SerializeField] TMP_Text txtName;
    //[SerializeField] TMP_Text txtDescription;
    [SerializeField] Button button;
    [SerializeField] Image iconImage;


    private AbilityChoice currentChoice;

    public void Setup(AbilityChoice choice)
    {
        currentChoice = choice;
        txtName.text = choice.name;
        //txtDescription.text = choice.description;

        if (iconImage != null && choice.icon != null)
        {
            iconImage.sprite = choice.icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            choice.onSelect.Invoke();
            UIManager.Instance.AbilityUI.Hide(); 
        });
    }
}