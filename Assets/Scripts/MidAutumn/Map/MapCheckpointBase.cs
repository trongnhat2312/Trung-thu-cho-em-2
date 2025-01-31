using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class MapCheckpointBase : MonoBehaviour
{
    [SerializeField] Image checkpointItem;   // bonus item of checkpoint
    [SerializeField] Image checkpointNumber;
    [SerializeField] Text checkpointName;

    void Start()
    {
        
    }

    public virtual void SetActiveState(bool isActive)
    {
        if (isActive)
        {
            // active item
            if (checkpointItem != null)
            {
                checkpointItem.color = Color.white;
            }

            // active number
            if (checkpointNumber != null)
            {
                checkpointNumber.GetComponent<UIEffect>().effectFactor = 0;
            }

            var baseUIEffect = GetComponent<UIEffect>();
            if (baseUIEffect != null)
            {
                baseUIEffect.enabled = false;
            }
        }
    }
}
