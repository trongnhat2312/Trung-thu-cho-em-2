using System; 
using UnityEngine;
using UnityEngine.UI;


namespace TreasureHunt.Popup
{

    public class RewardPlacePopup : MonoBehaviour
    {
        private Action OnClosePopupListener;

        public string[] words = new string[]
            {
            "Xuân",
            "An, Khang",
            "Đức, Tài, Như, Ý",
            "Niên",
            "Thịnh, Vượng",
            "Phúc, Thọ, Vô, Biên"
            };

        [SerializeField] Text PlaceNum;
        [SerializeField] private Button btnOk;
        [SerializeField] private GameObject objRewardPlace1;
        [SerializeField] private GameObject objRewardPlace2;
        [SerializeField] private GameObject objRewardPlace3;
        [SerializeField] private GameObject objRewardPlace4;
        [SerializeField] private GameObject objRewardPlace5;

        private void Start()
        {
            btnOk.onClick.AddListener(OKButtonChucmung);
        }

        public void ShowPopup(int pIdPlace, Action pCallback)
        {
            OnClosePopupListener = pCallback;
            SetupUI(pIdPlace);
            gameObject.SetActive(true);
        }

        private void HidePopup()
        {
            OnClosePopupListener?.Invoke();
            gameObject.SetActive(false);
        }

        public void SetupUI(int pIdPlace)
        {
            //PlaceNum.text = "SỐ "  + (StaticParamClass.CheckinPlace + 1);
            PlaceNum.text = WordOfPlace(pIdPlace);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.checkIn);

            bool isPlace1 = pIdPlace == (int)PlaceID.Place_01_Place1;
            bool isPlace2 = pIdPlace == (int)PlaceID.Place_02_Place2;
            bool isPlace3 = pIdPlace == (int)PlaceID.Place_03_Place3;
            bool isPlace4 = pIdPlace == (int)PlaceID.Place_04_Place4;
            bool isPlace5 = pIdPlace == (int)PlaceID.Place_05_Place5;

            objRewardPlace1.SetActive(isPlace1);
            objRewardPlace2.SetActive(isPlace2);
            objRewardPlace3.SetActive(isPlace3);
            objRewardPlace4.SetActive(isPlace4);
            objRewardPlace5.SetActive(isPlace5);
        }

        public string WordOfPlace(int placeId)
        {
            int wordPos = placeId - 1;
            return words[wordPos];
        }
        private void OKButtonChucmung()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

            HidePopup();
        }

    }

}
