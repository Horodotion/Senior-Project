using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(menuName = "Decision/Movement Dicision")]
public class MovementDecision : WeightedDecision
{

    
    [System.Serializable]
    public class BossStateInt: StateInt
    {
        public BossState bossState;
    }
    public Enum thisEnum;
    public BossStateInt[] bossInt;
    //public int taunt, meleeAttack, rangedAttack, takingCover, wait, teleport, teleBindPlayer , laser, turrets, mines, orbWalk, mobs, ambushed, dead, setArmor;

    //public int[] decisions = new int[6];
    private void OnValidate()
    {
        //decisions = new int[] { taunt, meleeAttack, rangedAttack, takingCover, wait, teleport, teleBindPlayer, laser, turrets, mines, orbWalk, mobs, ambushed, dead, setArmor };
        //decisions = new int[Enum.GetValues(typeof(BossState)).Length];
        
        for (int i = 0; i < bossInt.Length; i++)
        {
            for (int j = 0; j < decisions.Length; j++)
            {
                if ((int)bossInt[i].bossState == j)
                {
                    decisions[j] = bossInt[i].GetValue();
                }
                bool isWithin = false;
                for (int h = 0; h < bossInt.Length; h++)
                {
                    if ((int)bossInt[h].bossState == j)
                    {
                        isWithin = true;
                    }
                }
                if (!isWithin)
                {
                    decisions[j] = 0;
                }
            }
        }
        //DisplayLog();
        
    }

    //StateDecistion[] movementDecistion = new StateDecistion[5];
    public MovementDecision() : base(new int[Enum.GetValues(typeof(BossState)).Length]) // int[Enum.GetValues(typeof(BossState)).Length - 1])
    {
        //decisions = new int[8];
    }
    public MovementDecision(MovementDecision mD): base(new int[Enum.GetValues(typeof (BossState)).Length])
    {
        //decisions = new int[7];
        for (int i = 0; i < Enum.GetValues(typeof(BossState)).Length; i++)
        {
            decisions[i] = mD.decisions[i];
        }
    }
    public MovementDecision(BossState bossState) : base(new int[Enum.GetValues(typeof(BossState)).Length]) // int[Enum.GetValues(typeof(BossState)).Length - 1])
    {
        decisions[(int)bossState] = 1;
        //decisions = new int[8];
    }
    public BossState GetTheNextRandomDicision()
    {
        return (BossState)(GetRandomIndex());
    }

    public void DisplayLog()
    {
        string temp = name + ": ";
        temp += Enum.GetNames(typeof(BossState))[0] + ": " + decisions[0];
        for (int j = 1; j < decisions.Length; j++)
        {
            temp += ", " + Enum.GetNames(typeof(BossState))[j] + ": " + decisions[j];
        }
        Debug.Log(temp);
    }
}
