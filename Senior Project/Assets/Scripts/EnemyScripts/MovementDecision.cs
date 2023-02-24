using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Decision/Movement Dicision")]
public class MovementDecision : WeightedDecision
{
    
    public int takingCover;
    public int meleeAttack;
    public int rangedAttack;
    public int taunt;
    public int teleport;
    public int wait;
    public int laser;

    //public int[] decisions = new int[6];
    private void OnValidate()
    {
        decisions = new int[] { takingCover, meleeAttack, rangedAttack, taunt, teleport, wait, laser };
    }

    //StateDecistion[] movementDecistion = new StateDecistion[5];
    public MovementDecision()
    {
        decisions = new int[7];
    }
    public MovementDecision(MovementDecision mD)
    {
        decisions = new int[7];
        for (int i = 0; i < decisions.Length; i++)
        {
            decisions[i] = mD.decisions[i];
        }
    }
    
    public MovementDecision(int tC, int mA, int rA, int t, int tel, int las)
    {
        decisions = new int[] { tC, mA, rA, t, tel, las };
    }

    
    public BossState GiveTheNextRandomDicision()
    {
        return (BossState)(GetRandomIndex() + 1);
    }

    public void DisplayLog()
    {
        Debug.Log("Taking Cover: " + decisions[0] + " Melee Attack: " + decisions[1] + " Ranged Attack: " + decisions[2] + " Taunt: " + decisions[3] + " Teleport: " + decisions[4] + " Laser: " + decisions[5]);
    }
}
