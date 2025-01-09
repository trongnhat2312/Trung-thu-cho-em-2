using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Koi.Scene
{
    public class BaseSceneLoadManager : MonoBehaviour
    {
        [SerializeField] string sceneRootTag = "scene_root";

        [SerializeField] BaseFadeLoad fadeLoad;

        List<GameObject> listCurSceneRoot = new List<GameObject>();

        bool onChangingScene = false;


        #region change scene
        public virtual void ChangeScene(string sceneName, bool gcCollect = true, float durationFadeIn = 0.5f, float durationFadeOut = 0.5f, string data = null, Action fadeInCallback = null)
        {
            if (!onChangingScene)
            {
                onChangingScene = true;
                StartCoroutine(IChangeScene(sceneName, gcCollect, durationFadeIn, durationFadeOut, data, fadeInCallback));
            }
            else
            {
                Debug.LogError("Changing Scene, can not change to scene " + sceneName);
            }
        }

        protected virtual IEnumerator IChangeScene(string sceneName, bool gcCollect, float durationFadeIn, float durationFadeOut, string data, Action fadeInCallback)
        {
            FadeIn(durationFadeIn);

            yield return new WaitForSecondsRealtime(durationFadeIn);

            try
            {
                fadeInCallback?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError($"FadeInCallback process Exception: {exception.Message}");
            }

            UnloadCurScene(gcCollect);
            yield return new WaitForEndOfFrame();

            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            while (!loadSceneAsync.isDone)
            {
                yield return null;
                fadeLoad.SetupPercent(loadSceneAsync.progress);
            }

            SetupNewScene(sceneName, data);

            yield return null;

            FadeOut(durationFadeOut);

            yield return new WaitForSecondsRealtime(durationFadeOut);


            onChangingScene = false;
        }

        #endregion




        #region with addtiveScene


        public virtual void LoadAdditiveScene(string sceneName, Action<GameObject> callback, bool useFade = false, float durationFadeIn = 0.5f, float durationFadeOut = 0.5f, string data = null)
        {
            StartCoroutine(ILoadAdditiveScene(sceneName, callback, useFade, durationFadeIn, durationFadeOut, data));
        }

        protected virtual IEnumerator ILoadAdditiveScene(string sceneName, Action<GameObject> callback, bool useFade, float durationFadeIn, float durationFadeOut, string data)
        {
            if (useFade)
            {
                FadeIn(durationFadeIn);
                yield return new WaitForSecondsRealtime(durationFadeIn);
            }

            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadSceneAsync.isDone)
            {
                yield return null;
                fadeLoad.SetupPercent(loadSceneAsync.progress);
            }

            bool haveNewSceneRoot = SetupNewScene(sceneName, data);

            yield return null;

            if (useFade)
            {
                FadeOut(durationFadeOut);
            }

            if (haveNewSceneRoot && callback != null)
            {
                callback(listCurSceneRoot[listCurSceneRoot.Count - 1]);
            }
        }


        public virtual void RemoveAddedScene(GameObject sceneRoot, bool useFade = false, float durationFadeIn = 0.5f, float durationFadeOut = 0.5f, bool gcCollect = true)
        {
            StartCoroutine(IRemoveAddedScene(sceneRoot, useFade, durationFadeIn, durationFadeOut, gcCollect));
        }

        protected virtual IEnumerator IRemoveAddedScene(GameObject sceneRoot, bool useFade, float durationFadeIn, float durationFadeOut, bool gcCollect)
        {
            if (useFade)
            {
                FadeIn(durationFadeIn);
                yield return new WaitForSecondsRealtime(durationFadeIn);
            }

            var loadSceneAsync = SceneManager.UnloadSceneAsync(sceneRoot.scene);
            while (!loadSceneAsync.isDone)
            {
                yield return null;
            }

            if (gcCollect)
            {
                System.GC.Collect();
            }

            if (useFade)
            {
                FadeOut(durationFadeOut);
            }
        }

        #endregion




        #region process rootScene

        protected virtual bool SetupNewScene(string sceneName, string data)
        {
            bool haveNewRoot = false;
            GameObject[] listRoot = GameObject.FindGameObjectsWithTag(sceneRootTag);
            foreach (GameObject curSceneRoot in listRoot)
            {
                if (curSceneRoot.name == sceneName && !IsExistSceneRoot(curSceneRoot))
                {
                    listCurSceneRoot.Add(curSceneRoot);
                    BaseSceneController sceneController = curSceneRoot.GetComponent<BaseSceneController>();
                    if (sceneController != null)
                    {
                        try
                        {
                            sceneController.SetupData(data);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError($"Setup NewScene Data Exception: {data}");
                        }
                    }
                    haveNewRoot = true;
                }
            }
            return haveNewRoot;
        }

        protected virtual bool IsExistSceneRoot(GameObject root)
        {
            foreach (GameObject pGameObject in listCurSceneRoot)
            {
                if (pGameObject == root)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void UnloadCurScene(bool gcCollect)
        {
            for (int i = 0; i < listCurSceneRoot.Count; i++)
            {
                if (listCurSceneRoot[i] != null)
                {
                    GameObject.Destroy(listCurSceneRoot[i]);
                }
            }
            listCurSceneRoot.Clear();
            if (gcCollect)
            {
                System.GC.Collect();
            }
        }

        #endregion




        public virtual void FadeIn(float duration)
        {
            fadeLoad.FadeIn(duration);
        }


        public virtual void FadeOut(float duration)
        {
            fadeLoad.FadeOut(duration);
        }

    }

}
