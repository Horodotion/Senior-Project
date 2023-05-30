using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(menuName = "Decision/Movement Dicision")]
public class MovementDecision : WeightedDecision
{
    public int taunt, meleeAttack, rangedAttack, takingCover, wait, teleport, teleBindPlayer , laser, turrets, mines, orbWalk;

    //public int[] decisions = new int[6];
    private void OnValidate()
    {
        decisions = new int[] { taunt, meleeAttack, rangedAttack, takingCover, wait, teleport, teleBindPlayer, laser, turrets, mines, orbWalk };
    }

    //StateDecistion[] movementDecistion = new StateDecistion[5];
    public MovementDecision() : base(new int[Enum.GetValues(typeof(BossState)).Length - 1])
    {
        //decisions = new int[8];
    }
    public MovementDecision(MovementDecision mD): base(new int[Enum.GetValues(typeof (BossState)).Length - 1])
    {
        //decisions = new int[7];
        for (int i = 0; i < decisions.Length; i++)
        {
            decisions[i] = mD.decisions[i];
        }
    }
    /*
    public MovementDecision(int tC, int mA, int rA, int t, int tel, int las)
    {
        decisions = new int[] { tC, mA, rA, t, tel, las };
    }
    */
    
    public BossState GiveTheNextRandomDicision()
    {
        return (BossState)(GetRandomIndex() + 1);
    }

    public void DisplayLog()
    {
        Debug.Log("Taking Cover: " + decisions[0] + " Melee Attack: " + decisions[1] + " Ranged Attack: " + decisions[2] + " Taunt: " + decisions[3] + " Teleport: " + decisions[4] + " Laser: " + decisions[5]);
    }
}
