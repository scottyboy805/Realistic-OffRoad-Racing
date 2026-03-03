//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ALIyerEdon;


namespace ALIyerEdon
{
	public enum Quality_Level
    {
		VeryLow,Low,Medium,High,Ultra
    }
	public class MainUtility : MonoBehaviour
	{
		public bool showCursor = true;
		public int targetFPS = 120;
		public float fadeDelay = 1f;

		// Instantiate car at start
		public CarSelect carSelect;

		public AudioSource clickSound;

		public GameObject Loading, exitMenu, developerOptions;

		public Text totalScoresInfo;

		public UI_Selection mainMenuUI;

        public int startingScore = 1400;
        public int cheatCoins = 30000;

		// Enable or diable buttons icon based on the control type
		public GameObject[] gamepadButtons;
		public GameObject[] keyboardButtons;

		void Awake()
		{
			AudioListener.volume = 1f;
			
			Time.timeScale = 1f;

			PlayerPrefs.SetInt("Target FPS", targetFPS);

			Application.targetFrameRate = targetFPS;

#if UNITY_EDITOR
            Cursor.visible = true;
#else
            Cursor.visible = showCursor;
#endif

            // Is game first run?   3 & 1 => true    0 => false
            if (PlayerPrefs.GetInt("FirstRun") != 3)
			{
				PlayerPrefs.SetInt("OriginalX", Screen.width);
				PlayerPrefs.SetInt("OriginalY", Screen.height);

				// Set starting color for the first car (car0)
				PlayerPrefs.SetInt("CarColor0", 0);

				// Enable right position ui info display
				PlayerPrefs.SetString("Local_Position", "Off");
				PlayerPrefs.SetString("Side_UI", "On");

				// Open the first car (car0)
				PlayerPrefs.SetInt("Car0", 3);

				// Open the first level (level0)
				PlayerPrefs.SetInt("Level0", 3);
				PlayerPrefs.SetInt("Weather0", 3);
				PlayerPrefs.SetInt("Weather1", 3);
				PlayerPrefs.SetInt("Weather2", 3);

				// Set none  awards for all levels
				PlayerPrefs.SetInt("Award_Level_0", 3);
				PlayerPrefs.SetInt("Award_Level_1", 3);
				PlayerPrefs.SetInt("Award_Level_2", 3);
				PlayerPrefs.SetInt("Award_Level_3", 3);
				PlayerPrefs.SetInt("Award_Level_4", 3);
				PlayerPrefs.SetInt("Award_Level_5", 3);
				PlayerPrefs.SetInt("Award_Level_6", 3);
				PlayerPrefs.SetInt("Award_Level_7", 3);

				// Player first time starting the game score
				PlayerPrefs.SetInt("TotalScores", startingScore);

				// Set default settings for graphics
				PlayerPrefs.SetString("Display_FPS", "Off");
				PlayerPrefs.SetString("Dynamic_Camera", "Off");
				PlayerPrefs.SetString("Wheel_Smoke", "On");
				PlayerPrefs.SetString("Balloons", "Off");

				// Quality settings
				PlayerPrefs.SetString("SSR", "On");
				PlayerPrefs.SetString("DOF", "Off");
				PlayerPrefs.SetString("MotionBlur", "On");
				PlayerPrefs.SetString("AO", "On");
				PlayerPrefs.SetString("SSGI", "Off");
				PlayerPrefs.SetString("MusicVolume", "Medium");
				PlayerPrefs.SetString("CarSFX", "High");
				PlayerPrefs.SetString("DifficultyLevel", "High"); // Low:Easy , Medium:Medium , High:Hard


				// Disable the first running the game settings
				PlayerPrefs.SetInt("FirstRun", 3);
			}

            if (totalScoresInfo)
                totalScoresInfo.text = "Total Coins : " + PlayerPrefs.GetInt("TotalScores").ToString();
        }

        void Update()
		{
			// Cheat coins
			if (Keyboard.current != null)
			{
				if (Keyboard.current.xKey.ReadValue() > 0 &&
				Keyboard.current.nKey.ReadValue() > 0 &&
				Keyboard.current.pKey.ReadValue() > 0)
				{
					if (!cheatActivated)
						StartCoroutine(Cheat_Coins());
				}
				// developer options mode
				if (Keyboard.current.qKey.ReadValue() > 0 &&
					Keyboard.current.wKey.ReadValue() > 0 &&
					Keyboard.current.eKey.ReadValue() > 0)
				{
					developerOptions.SetActive(true);

				}
			}
            // Exit with back button
            if (mainMenuUI.enabled == true)
			{
				#region Exit
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonEast.wasPressedThisFrame)
					{
                        if (FindFirstObjectByType<FadeMode>())
                            FindFirstObjectByType<FadeMode>().Do_Fade();

                        mainMenuUI.enabled = false;

						exitMenu.SetActive(!exitMenu.activeSelf);
					}
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.escapeKey.wasPressedThisFrame)
						{
							if (FindFirstObjectByType<FadeMode>())
								FindFirstObjectByType<FadeMode>().Do_Fade();

							mainMenuUI.enabled = true;

							exitMenu.SetActive(!exitMenu.activeSelf);
						}
					}
				}
				#endregion
			}

			#region Delete SaveData

			if (Gamepad.current != null)
			{
				if (Gamepad.current.selectButton.wasPressedThisFrame)
				{
					#if UNITY_EDITOR
					PlayerPrefs.DeleteAll();
					Debug.Log("PlayerPrefs.DeleteAll ();");
					#endif
				}
			}
			else
			{
				if (Keyboard.current != null)
				{
					if (Keyboard.current.hKey.wasPressedThisFrame)
					{
#if UNITY_EDITOR
						PlayerPrefs.DeleteAll();
						Debug.Log("PlayerPrefs.DeleteAll ();");
#endif
					}
				}
			}
			#endregion
		}

		public void Exit()
		{
			Application.Quit();
		}

		public void LoadLevel(string name)
		{
			Loading.SetActive(true);
			SceneManager.LoadSceneAsync(name);
		}

		public void OpenURL(string val)
		{
			Application.OpenURL(val);
		}

		public void Click_Sound()
		{
			if (clickSound)
				clickSound.PlayOneShot(clickSound.clip);
		}

		public void Disable_UI_Selection(UI_Selection target)
        {
			target.enabled = false;
        }

		public void Enable_UI_Selection(UI_Selection target)
		{
			target.enabled = true;
		}
		public void Disable_CarUI_Selection(Car_UI_Selection target)
		{
			target.enabled = false;
		}

		public void Enable_CarUI_Selection(Car_UI_Selection target)
		{
			target.enabled = true;
		}

		// Update controls icon tips (keyaboard keys or joystick nums (Xbox Controller))
		public void Change_ControlType(Control_Type controlTips)
		{
			// Select control icon tips
			if (controlTips == Control_Type.Keyboard)
			{
				foreach (GameObject gp in gamepadButtons)
					gp.SetActive(false);

				foreach (GameObject kb in keyboardButtons)
					kb.SetActive(true);
			}

			if (controlTips == Control_Type.Gamepad)
			{
				foreach (GameObject gp in gamepadButtons)
					gp.SetActive(true);

				foreach (GameObject kb in keyboardButtons)
					kb.SetActive(false);
			}
		}


        public void SetTrue(GameObject target)
        {
            StartCoroutine(Delay_True(target));
        }

        public void SetFalse(GameObject target)
        {
            StartCoroutine(Delay_False(target));
        }

        public void ToggleObject(GameObject target)
        {
            StartCoroutine(Delay_Toggle(target));
        }
        //____________________________________________
        IEnumerator Delay_False(GameObject target)
		{
            yield return new WaitForSeconds(fadeDelay);
			target.SetActive(false);
        }
        IEnumerator Delay_True(GameObject target)
        {
            if (FindFirstObjectByType<FadeMode>())
                FindFirstObjectByType<FadeMode>().Do_Fade();

            yield return new WaitForSeconds(fadeDelay);
            target.SetActive(true);
        }
        IEnumerator Delay_Toggle(GameObject target)
        {
            if (FindFirstObjectByType<FadeMode>())
                FindFirstObjectByType<FadeMode>().Do_Fade();

            yield return new WaitForSeconds(fadeDelay);
            target.SetActive(!target.activeSelf);
        }

		bool cheatActivated;

		IEnumerator Cheat_Coins()
		{
            PlayerPrefs.SetInt("TotalScores", PlayerPrefs.GetInt("TotalScores") + cheatCoins);
			totalScoresInfo.text = PlayerPrefs.GetInt("TotalScores").ToString();
            FindFirstObjectByType<LevelSelect>().TotalScores.text = PlayerPrefs.GetInt("TotalScores").ToString();
            FindFirstObjectByType<CarSelect>().TotalScores.text = PlayerPrefs.GetInt("TotalScores").ToString();
			Click_Sound();
            cheatActivated = true;
            yield return new WaitForSeconds(5f);
			cheatActivated = false;
        }

		public void Developer_Comands()
		{
			if (developerOptions.GetComponent<InputField>().text == "delete save")
			{
                Click_Sound();
                PlayerPrefs.DeleteAll();
				Application.Quit();
			}
		}
    }	
}