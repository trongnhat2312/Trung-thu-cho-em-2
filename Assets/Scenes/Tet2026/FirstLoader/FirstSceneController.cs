using Cysharp.Threading.Tasks;
using Koi.Scene;
using TreasureHunt.FirstLoad;
using UnityEngine;

public class FirstSceneController : BaseSceneController
{
    [SerializeField] private GameObject prefabFirstLoader;

    public GameObject objFirstLoader;

    private async void Start()
    {
        Application.targetFrameRate = 60;
        //Debug.LogError("FirstSceneController: Start()");
        await UniTask.DelayFrame(3);
        if (!setupDataMark)
        {
            SetupData("");
        }
    } 

    bool setupDataMark = false;
    public override void SetupData(string jsonData)
    { 
        if(setupDataMark)
        {
            //Debug.LogError("FirstSceneController: Dont need SetupData");
            return;
        }
        setupDataMark = true; 
        //Debug.LogError("FirstSceneController: SetupData");
        InitSceneRootObj(jsonData); 
    }

    private void InitSceneRootObj(string jsonData)
    {
        //Debug.LogError("FirstSceneController: InitSceneRootObj()");
        if (objFirstLoader != null)
        {
            //Debug.LogError("FirstSceneController: destroy immediate objFirstLoader");
            DestroyImmediate(objFirstLoader);
        }
        //Debug.LogError("FirstSceneController: Instantiate objFirstLoader");
        objFirstLoader = Instantiate(prefabFirstLoader, transform);
        FirstSceneLoader firstSceneLoader = objFirstLoader.GetComponent<FirstSceneLoader>();
        firstSceneLoader.AddOnCompletedFirstLoadListener(LoadNextScene);//Force addListener after call setup data 
        //Debug.LogError("FirstSceneController: SetupDataLoaded");
        firstSceneLoader.SetupDataLoaded(jsonData);
    }

    private void LoadNextScene()
    { 
        //Debug.LogError("FirstSceneController: LoadNextScene()");
        // GameLoadManager.Instance.ChangeScene(sceneName: SceneConst.S2_MAINCENE);
    }
}
