using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoFollowTransform : MonoBehaviour
{
    [System.Serializable]
    public class Direct3D
    {
        public bool x = true;
        public bool y = true;
        public bool z = true;
    }
    [SerializeField] Transform target;
    [SerializeField] Direct3D directFollow;
    [SerializeField] float lerp = 1.0f;

    [SerializeField] bool rotateFollow = false;

    private void LateUpdate()
    {
        UpdateFollow();
    }

    public void UpdateFollow()
    {
        if (target != null)
        {
            var pos = transform.position;
            var targetPos = target.position;
            if (lerp >= 1.0f)
            {
                if (directFollow.x)
                    pos.x = targetPos.x;

                if (directFollow.y)
                    pos.y = targetPos.y;

                if (directFollow.z)
                    pos.z = targetPos.z;
            }
            else
            {
                if (directFollow.x)
                    pos.x = Mathf.Lerp(pos.x, targetPos.x, lerp);

                if (directFollow.y)
                    pos.y = Mathf.Lerp(pos.y, targetPos.y, lerp);

                if (directFollow.z)
                    pos.z = Mathf.Lerp(pos.z, targetPos.z, lerp);
            }

            transform.position = pos;

            if (rotateFollow)
            {
                transform.rotation = target.rotation;
            }
        }
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(AutoFollowTransform))]
public class AutoFollowTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AutoFollowTransform myScript = (AutoFollowTransform)target;

        GUILayout.Label("");
        GUILayout.Label("Update Follow");

        if (GUILayout.Button("Update"))
        {
            myScript.UpdateFollow();
        }

        GUILayout.Label("");
    }
}
#endif
