using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    [HideInInspector]
    public Unit from;
    public float range = 1f;
    public void DoAttack(Unit who) {
        from = who;
        List<Unit> targets = GetTargetsInRange();
        for (int i = 0; i < targets.Count; i++) {
            Unit unit = targets[i];
            unit.GetHurt(from);
        }
    }
    List<Unit> GetTargetsInRange() {
        Unit[] units = FindObjectsOfType<Unit>();
        List<Unit> unitsInRange = new List<Unit>();
        Vector2 attackPosition = transform.position;
        for (int i = 0; i < units.Length; i++) {
            Unit unit = units[i];
            if (unit.group == from.group) { continue; }
            Vector2 unitPosition = unit.transform.position;
            float distance = Vector2.Distance(attackPosition, unitPosition);
            if (distance <= range) { unitsInRange.Add(unit); }
        }
        return unitsInRange;
    }
}
