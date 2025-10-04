using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MidAutumnMainSceneController : MainController
{
    [SerializeField] StarLightTransformer starLightTransformer;
    [SerializeField] Text txtComplete;	
    protected bool isAnimCompleted = false;


    protected override async void ShowVictoryUI()
    {
        if (!isAnimCompleted)
        {

            isAnimCompleted = true;
            starLightTransformer.DoTransformToStarLight();
            await UniTask.Delay(2000);
            txtComplete.gameObject.SetActive(true);
        }
    }
}