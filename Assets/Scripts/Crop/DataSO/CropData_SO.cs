using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropData_SO", menuName ="Crop/CropDataList")]
public class CropData_SO : ScriptableObject
{
    public List<CropDetails> cropDetailsList;
}
