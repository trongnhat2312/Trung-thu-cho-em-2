using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Koi.UI.Example
{
    public class TestLoadChild : MonoBehaviour
    {

        List<RectTransform> listChild = new List<RectTransform>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadChild();
            }
        }


        void LoadChild()
        {
            var array = transform.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform child in array)
            {
                listChild.Add(child);
            }
        }
    }
}
