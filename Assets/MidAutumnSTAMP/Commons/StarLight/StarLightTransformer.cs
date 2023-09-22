using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class StarLightTransformer : MonoBehaviour
{
    [SerializeField]
    Image fogBg;

    [SerializeField]
    List<Transform> beforeParts;

    [SerializeField]
    List<Transform> beforePartRenderers;

    [SerializeField]
    Transform starLightObject;

    [SerializeField]
    List<ParticleSystem> transformCollectAuraEffects;

    [SerializeField]
    List<ParticleSystem> transformExpEffect;

    [Header("--- Debug ---")]
    [SerializeField] float collectDuration = 3.0f;
    [SerializeField] float targetRotate = 360 * 3;
    [SerializeField] float targetSize = 0.1f;
    [SerializeField] Ease collectEase = Ease.InOutQuad;
    [SerializeField] float explodeRotDuration = 1.5f;
    [SerializeField] float explodeZoomDuration = 1.5f;
    [SerializeField] float explodeRotate = 360 * 3;
    [SerializeField] Ease explodeRotEase = Ease.InOutQuad;
    [SerializeField] Ease explodeZoomEase = Ease.InOutQuad;

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBeforeTransform();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DoTransformToStarLight();
        }
	}

	public void ResetBeforeTransform()
    {
		SoundBase.Instance.GetComponent<AudioSource>().Stop();
		foreach (var part in beforeParts)
        {
            part.localScale = Vector3.one;
            part.localRotation = Quaternion.identity;
            part.gameObject.SetActive(true);
        }
        foreach (var part in beforePartRenderers)
        {
            part.gameObject.SetActive(true);
        }
        starLightObject.gameObject.SetActive(false);
        fogBg.gameObject.SetActive(false);
    }

	public async void DoTransformToStarLight()
    {
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.starPartsCombine);
		fogBg.gameObject.SetActive(true);
        SetAlpha(fogBg, 0);
        await UniTask.DelayFrame(1);

        fogBg.DOFade(0.5f, 0.25f).SetEase(Ease.InQuad);

        // collect aura
        foreach (var particle in transformCollectAuraEffects)
        {
            var main = particle.main;
            main.loop = true;
            particle.Play();
        }

        DOTween.To(() => 0.0f, (f) =>
        {
            foreach (var part in beforeParts)
            {
                part.localScale = Vector3.one * Mathf.Lerp(1.0f, targetSize, f);
                float rot = Mathf.Lerp(0, targetRotate, f);// + Rand01 * 0.02f);
                part.localRotation = Quaternion.Euler(0, 0, rot);

			}
        }, 1.0f, collectDuration).SetEase(collectEase);

        await UniTask.DelayFrame(1);
        await UniTask.Delay((int)(collectDuration * 1000) - 300);

		SoundBase.Instance.GetComponent<AudioSource>().Stop();
		SoundBase.Instance.GetComponent<AudioSource>().loop = true;
		SoundBase.Instance.GetComponent<AudioSource>().clip = SoundBase.Instance.ChiecDenOngSaoPiano;
		SoundBase.Instance.GetComponent<AudioSource>().Play();

		starLightObject.gameObject.SetActive(true);
        starLightObject.localScale = Vector3.one * 0.01f;

        await UniTask.Delay(150);
        foreach (var part in beforePartRenderers)
        {
            part.gameObject.SetActive(false);
        }

        foreach (var particle in transformCollectAuraEffects)
        {
            StopParticle(particle);
        }

        // explode
        foreach (var particle in transformExpEffect)
        {
            particle.Play();
        }

        DOTween.To(() => 0.0f, (f) =>
        {
            starLightObject.localRotation = Quaternion.Euler(0, f * explodeRotate, 0);
        }, 1.0f, explodeRotDuration).SetEase(explodeRotEase);

        starLightObject.DOScale(1.0f, explodeZoomDuration * 0.5f).SetEase(explodeZoomEase);
    }


    void StopParticle(ParticleSystem particle)
    {
        var main = particle.main;
        main.loop = false;
    }

    void SetAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    float Rand01 => Random.Range(0.0f, 1.0f);
}
