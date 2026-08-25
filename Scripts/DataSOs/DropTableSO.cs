using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DropTableSO", menuName = "Scriptable Objects/DropTableSO")]
public class DropTableSO : ScriptableObject
{
    [SerializeField]
    public List<DropRow> rows;
}
[Serializable]
public class DropRow
{
    public string itemID;
    public DropItemAmountType dropType;
    public int amount_value1;
    public int amount_value2;
}
public enum DropItemAmountType
{
    BtwTwoValue,
    FixedValue
}