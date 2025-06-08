using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropItem
{
    void AbsorbByPlayer(Player player);
    void AbsorbByBoss(FinalBoss boss);
}
