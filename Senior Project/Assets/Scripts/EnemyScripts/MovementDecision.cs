using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decision/Movement Dicision")]
public class MovementDecision : Dicision
{
    public int takingCover;
    public int meleeAttack;
    public int rangedAttack;
    public int taunt;
    public MovementDecision()
    {
    }
    public MovementDecision(MovementDecision mD)
    {
        takingCover = mD.takingCover;
        meleeAttack = mD.meleeAttack;
        rangedAttack = mD.rangedAttack;
        taunt = mD.taunt;
    }
    public MovementDecision(int tC, int mA, int rA, int t)
    {
        takingCover = tC;
        meleeAttack = mA;
        rangedAttack = rA;
        taunt = t;
    }
    public void AddDicision(MovementDecision d)
    {
        this.takingCover += d.takingCover;
        this.meleeAttack += d.meleeAttack;
        this.rangedAttack += d.rangedAttack;
        this.taunt += d.taunt;
    }
    public MovementDecision AddDicisionWithOutput(MovementDecision d)
    {
        return new MovementDecision(
                this.takingCover + d.takingCover,
                this.meleeAttack + d.meleeAttack,
                this.rangedAttack + d.rangedAttack,
                this.taunt + d.taunt);
    }
    public int AddAllDicision()
    {
        return takingCover + meleeAttack + rangedAttack + taunt;
    }

    public BossState GiveTheNextRandomDicision()
    {
        if (takingCover < 0) takingCover = 0;
        if (meleeAttack < 0) meleeAttack = 0;
        if (rangedAttack < 0) rangedAttack = 0;
        if (taunt < 0) taunt = 0;
        int index = Random.Range(1, AddAllDicision());
        switch (index)
        {
            case int x when x > 0 && x <= takingCover:
                return BossState.takingCover;
            case int x when x > takingCover && x <= takingCover + meleeAttack:
                return BossState.meleeAttack;
            case int x when x > takingCover + meleeAttack && x <= takingCover + meleeAttack + rangedAttack:
                return BossState.rangedAttack;
            case int x when x > takingCover + meleeAttack + rangedAttack && x <= AddAllDicision():
                return BossState.taunt;
        }
        Debug.Log("Out of bounds in CalculateNextDicision() for movement");
        return BossState.takingCover;
    }

    public void DisplayLog()
    {
        Debug.Log("Taking Cover: " + takingCover + " Melee Attack: " + meleeAttack + " Ranged Attack: " + rangedAttack + " Taunt: " + taunt);
    }
}
