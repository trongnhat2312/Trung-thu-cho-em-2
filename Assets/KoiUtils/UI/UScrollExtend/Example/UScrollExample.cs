using UnityEngine;
using System.Collections;
using Koi.UI;

namespace Koi.UI.Example
{
    public class UScrollExample : MonoBehaviour
    {

        [SerializeField]
        ScrollRectController horizontalScroll;

        [SerializeField]
        ScrollRectController verticalScroll;

        // Use this for initialization
        void Start()
        {
            horizontalScroll.AddReachCellListener(OnHorizontalReachCell);
            verticalScroll.AddReachCellListener(OnVerticalReachCell);
        }

        // Update is called once per frame
        void Update()
        {

        }


        void OnHorizontalReachCell(int id)
        {
            Debug.Log(string.Format("HorizontalScroll: reach cell[{0}]", id));
        }

        void OnVerticalReachCell(int id)
        {
            Debug.Log(string.Format("VerticalScroll: reach cell[{0}]", id));
        }
    }
}