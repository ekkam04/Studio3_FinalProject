using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Ekkam
{
    public class UIManager : MonoBehaviour
    {
        public GameObject inventoryUI;
        public GameObject playerUI;
        public GameObject shopUI;
        public GameObject shopInteractionBlocker;
        public GameObject pickUpPrompt;
        public GameObject targetLockPrompt;

        public TMP_Text coinsText;
        public TMP_Text tokensText;

        public GameObject explorationReticle;
        public GameObject combatReticle;
        public GameObject scanReticle;

        public GameObject objectivesUI;

        public GameObject dialogUI;
        public TMP_Text dialogText;
        public GameObject dialogSkipButton;
        private int initialDialogLetterDelay = 20;
        private int dialogLetterDelay;
        public bool showingDialog;
        public GameObject dialogCamera;

        public Button nextButton;
        public Button[] optionButtons;

        public GameObject towerUI;
        public TMP_Text towerNameText;
        public Slider towerProgressSlider;
        
        public GameObject areaPopupUI;
        public TMP_Text areaPopupText;
        
        public GameObject scanPanel;
        public TMP_Text scanPanelText;
        public TMP_Text scanPanelDescription;
        
        public Volume vignetteVolume;
        public Vignette vignette;
        
        public GameObject mainMenuUI;
        public GameObject mainMenuVCam;
        
        public Button tutorialButton;
        public Button prologueButton;
        public Button theTowerButton;
        public Button theDeceptionButton;
        public Button theGarageButton;

        Player player;

        private void Start()
        {
            player = Player.Instance;
            pickUpPrompt.SetActive(false);
            dialogUI.SetActive(false);
        }

        private void Update()
        {
            coinsText.text = player.coins.ToString();
            tokensText.text = player.tokens.ToString();
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (shopUI.activeSelf && !dialogUI.activeSelf)
                {
                    CloseShopUI();
                }
            }

            // if (Input.GetKeyDown(KeyCode.H))
            // {
            //     OpenShopUI();
            // }
        }

        public async void ShowDialog(string dialog, bool showNextButton = false)
        {
            if (shopUI.activeSelf)
            {
                shopInteractionBlocker.SetActive(true);
            }

            showingDialog = true;
            inventoryUI.SetActive(false);
            if (!dialogCamera.activeSelf) dialogCamera.SetActive(true);
            dialogSkipButton.SetActive(true);
            dialogLetterDelay = initialDialogLetterDelay;
            dialogText.text = "";
            dialogUI.SetActive(true);
            foreach (var letter in dialog)
            {
                dialogText.text += letter;
                await Task.Delay(dialogLetterDelay);
            }
            dialogSkipButton.SetActive(false);

            showingDialog = false;
        }

        public void HideDialog()
        {
            dialogUI.SetActive(false);
            dialogText.text = "";
            if (!shopUI.activeSelf)
            {
                GameManager.Instance.ResumeGame();
                inventoryUI.SetActive(true);
                dialogCamera.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                shopInteractionBlocker.SetActive(false);
            }
        }

        public void HideAllOptions()
        {
            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
        
        public void OnDialogSkip()
        {
            dialogLetterDelay = 0;
        }

        public void OpenShopUI()
        {
            GameManager.Instance.PauseGame();
            shopUI.SetActive(true);
            dialogCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            inventoryUI.SetActive(false);
            playerUI.SetActive(false);
            objectivesUI.SetActive(false);
            
            SoundManager.Instance.PlaySound("shop-open");
        }

        public void CloseShopUI()
        {
            GameManager.Instance.ResumeGame();
            shopUI.SetActive(false);
            dialogCamera.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            inventoryUI.SetActive(true);
            playerUI.SetActive(true);
            objectivesUI.SetActive(true);
            shopInteractionBlocker.SetActive(false);
            
            SoundManager.Instance.PlaySound("shop-close");
        }
        
        public void ShowAreaPopup(string areaName, float duration)
        {
            StartCoroutine(ShowAreaPopupCoroutine(areaName, duration));
        }
        
        IEnumerator ShowAreaPopupCoroutine(string areaName, float duration)
        {
            areaPopupText.text = areaName;
            areaPopupUI.SetActive(true);
            yield return new WaitForSeconds(duration);
            areaPopupUI.SetActive(false);
        }
        
        public void PulseVignette(Color color, float fadeInDuration, float fadeOutDuration)
        {
            StartCoroutine(PulseVignetteCoroutine(color, fadeInDuration, fadeOutDuration));
        }
        
        IEnumerator PulseVignetteCoroutine(Color color, float fadeInDuration, float fadeOutDuration)
        {
            vignetteVolume.profile.TryGet(out vignette);
            vignette.color.value = color;
            float startTime = Time.time;
            float endTime = startTime + fadeInDuration;
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeInDuration;
                vignette.intensity.value = Mathf.Lerp(0, 0.5f, t);
                yield return null;
            }
            startTime = Time.time;
            endTime = startTime + fadeOutDuration;
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeOutDuration;
                vignette.intensity.value = Mathf.Lerp(0.5f, 0, t);
                yield return null;
            }
            vignette.intensity.value = 0;

        }
        
        public void HideAllUI()
        {
            inventoryUI.SetActive(false);
            playerUI.SetActive(false);
            objectivesUI.SetActive(false);
        }
        
        public void ShowAllUI()
        {
            inventoryUI.SetActive(true);
            playerUI.SetActive(true);
            objectivesUI.SetActive(true);
        }
    }
}
