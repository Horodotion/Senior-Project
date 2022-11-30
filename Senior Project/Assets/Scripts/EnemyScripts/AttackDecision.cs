using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decision/Attack Dicision")]
public class AttackDecision : Dicision
{
    public int iceAttack;
    public int fireAttack;
    public AttackDecision()
    {
    }
    public AttackDecision(int iA, int fA)
    {
        iceAttack = iA;
        fireAttack = fA;
    }
    public void AddDicision(AttackDecision d)
    {
        this.iceAttack += d.iceAttack;
        this.fireAttack += d.fireAttack;
    }
    public int AddAllDicision()
    {
        return iceAttack + fireAttack;
    }
    public bool GiveTheNextRandomDicision() //Return true if the output is ice, and false if it's fire.
    {
        int index = Random.Range(1, AddAllDicision());
        switch (index)
        {
            case int x when x > 0 && x <= iceAttack:
                return true;
            case int x when x > iceAttack && x <= AddAllDicision():
                return false;
        }
        Debug.Log("Out of bounds in GiveTheNextRandomDicision() for attack type");
        return false;
    }
}
