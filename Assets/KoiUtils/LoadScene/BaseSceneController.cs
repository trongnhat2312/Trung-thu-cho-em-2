using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koi.Scene
{
    public class BaseSceneController : MonoBehaviour
    {

        [SerializeField]
        protected bool useDontDestroyOnload = true;

        private void Awake()
        {
        }

        public virtual void SetupData(string jsonData)
        {

        }

        public virtual void DoDontDestroyOnload()
        {
            GameObject.DontDestroyOnLoad(this);
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        public virtual void OnHideScene()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnActiveScene()
        {
            gameObject.SetActive(true);
        }


        public virtual bool IsDontDestroyOnload => useDontDestroyOnload;
    }
}
