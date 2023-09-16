using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebGLSupport
{
    [RequireComponent(typeof(InputField))]
    public class WebGLInputBridge : MonoBehaviour
    {
        //[SerializeField]
        //InputField inputField;

        //[SerializeField]
        private WebGLSupport.WebGLInput webGLInput;

        private void OnDisable()
        {
            //CheckToCleanWebGLInputComponent();
        }

        private void OnEnable()
        {
            CheckToCleanWebGLInputComponent();
            webGLInput = gameObject.AddComponent<WebGLSupport.WebGLInput>();
        }

        void CheckToCleanWebGLInputComponent()
        {
            if (webGLInput == null)
            {
                webGLInput = GetComponent<WebGLSupport.WebGLInput>();
            }
            if (webGLInput != null)
            {
                GameObject.DestroyImmediate(webGLInput);
            }
        }
    }
}