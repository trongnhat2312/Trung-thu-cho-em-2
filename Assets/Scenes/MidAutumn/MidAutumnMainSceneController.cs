using UnityEngine;

public class MidAutumnMainSceneController : MainController
{
    [SerializeField] StarLightTransformer starLightTransformer;


    protected override void ShowVictoryUI()
    {
        starLightTransformer.DoTransformToStarLight();
    }
}