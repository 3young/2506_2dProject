using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    [SerializeField] GameObject sideHeartPrefab; 
    [SerializeField] Transform abilityParent; 
    [SerializeField] Sprite abilityIconSprite;

    private AbilityUIManager uiManager;
    private SideHeartAttackAbility sideAttack;

    private void Awake()
    {
        uiManager = UIManager.Instance.AbilityUI;
    }

    public void OnPlayerLevelUp()
    {
        uiManager.ShowAbilityOptions(new List<AbilityChoice> {
            new AbilityChoice {
                name = "Side Heart Attack",
                //description = "지정된 각도에서 자동 발사",
                icon = abilityIconSprite, 
                onSelect = () => {
                    if (sideAttack == null)
                    {
                        sideAttack = GameManager.Instance.CurrentPlayer.GetComponentInChildren<SideHeartAttackAbility>();
                    }

                    if (!sideAttack.enabled)
                    {
                        sideAttack.enabled = true;
                    }

                    sideAttack.LevelUp();
                }
            }
        });
    }
}