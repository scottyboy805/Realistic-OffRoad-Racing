//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ALIyerEdon;
using UnityEngine.InputSystem;

namespace ALIyerEdon
{
	public enum Control_Type
    {
		Keyboard,Gamepad
    }

	public class InputSystem : MonoBehaviour
	{
		// Enable or diable buttons icon based on the control type
		[Header("UI Icons")]
		[Space(5)]
		public GameObject[] gamepadUI;
		public GameObject[] keyboardUI;

		EasyCarController controller;

		float motorInput;
		float steerInput;
		bool handBrake;

		[HideInInspector] public bool canStartRace;
		[HideInInspector] public bool finishedRace;
		[HideInInspector] public bool raceIsStarted;
		[HideInInspector] public bool canControl = false;

		Start_Cutscene startCutscene;

		IEnumerator Start()
		{
			if(FindFirstObjectByType<Start_Cutscene>())
				startCutscene = FindFirstObjectByType<Start_Cutscene>();

            yield return new WaitForEndOfFrame();

			controller = GameObject.FindGameObjectWithTag("Player")
				.GetComponent<EasyCarController>();

			GameObject.FindGameObjectWithTag("Player")
				.GetComponent<Car_AI>().enabled = false;
		}

        // Update controls icon tips (keyaboard keys or joystick nums (Xbox Controller))
        public void Change_ControlType(Control_Type controlTips)
        {
            // Select control icon tips
            if (controlTips == Control_Type.Keyboard)
            {
                foreach (GameObject gp in gamepadUI)
                    gp.SetActive(false);

                foreach (GameObject kb in keyboardUI)
                    kb.SetActive(true);
            }

            if (controlTips == Control_Type.Gamepad)
            {
                foreach (GameObject gp in gamepadUI)
                    gp.SetActive(true);

                foreach (GameObject kb in keyboardUI)
                    kb.SetActive(false);
            }
        }

        void Update()
		{
			
			#region Start Race
			// Start race button
			if (canStartRace && !raceIsStarted)
			{
				// Start race
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonSouth.wasPressedThisFrame)
						FindFirstObjectByType<Race_Manager>().StartRace_Button();
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.enterKey.wasPressedThisFrame)
						{
							FindFirstObjectByType<Race_Manager>().StartRace_Button();
						}
					}
				}

				// Exit Race
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonEast.wasPressedThisFrame)
						FindFirstObjectByType<Pause_Menu>().Exit();
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.escapeKey.wasPressedThisFrame)
							FindFirstObjectByType<Pause_Menu>().Exit();
					}
				}
			}

            #endregion

            #region Finish Race
            // Finish race button
            if (finishedRace)
            {
                // Restart race
                if (Gamepad.current != null)
                {
                    if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                        FindFirstObjectByType<Pause_Menu>().Restart();
                }
                else
                {
					if (Keyboard.current != null)
					{
						if (Keyboard.current.enterKey.wasPressedThisFrame)
						{
							FindFirstObjectByType<Pause_Menu>().Restart();
						}
					}
                }

                // Exit Race
                if (Gamepad.current != null)
                {
                    if (Gamepad.current.buttonEast.wasPressedThisFrame)
                        FindFirstObjectByType<Pause_Menu>().Exit();
                }
                else
                {
					if (Keyboard.current != null)
					{
						if (Keyboard.current.escapeKey.wasPressedThisFrame)
							FindFirstObjectByType<Pause_Menu>().Exit();
					}
                }
            }

            #endregion

            #region Skip Cutscene
            if (startCutscene && startCutscene.canSkip && !startCutscene.skipped
				&& FindFirstObjectByType<InputSystem>().canStartRace != true)
			{
				// Gamepad select button to skip cutscene
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonSouth.wasPressedThisFrame)
					{
						startCutscene.skipped = true;
						startCutscene.Skip_Cutscene();
						canStartRace = true;
					}
				}
				else
				{
					// Keyboard tab key to skip cutscene
					if (Keyboard.current != null)
					{
						if (Keyboard.current.enterKey.wasPressedThisFrame)
						{
							startCutscene.skipped = true;
							startCutscene.Skip_Cutscene();
							canStartRace = true;
						}
					}
				}
			}
			#endregion

			if (controller && canControl)
			{
				#region Throttle
				// Throttle input
				if (Gamepad.current != null)
				{
					motorInput =
								 Gamepad.current.rightTrigger.ReadValue() +
								 (-Gamepad.current.leftTrigger.ReadValue());
				}
				else
				{
					if (Keyboard.current != null)
					{
						motorInput = Keyboard.current.wKey.ReadValue()
								 + (-Keyboard.current.sKey.ReadValue());
					}
				}
				#endregion

				#region Steer
				// Steer input
				if (Gamepad.current != null)
				{
					steerInput = Gamepad.current.leftStick.ReadValue().x;
				}
				else
				{
					if (Keyboard.current != null)
					{
						steerInput = (-Keyboard.current.aKey.ReadValue()) +
									Keyboard.current.dKey.ReadValue();
					}
				}
				#endregion

				#region Handbrake
				// Hand brake
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonEast.ReadValue() > 0)
						handBrake = true;
					else
						handBrake = false;
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.spaceKey.ReadValue() > 0)
							handBrake = true;
						else
							handBrake = false;
					}
				}
				#endregion

				#region Camera Switch
				// Camera switch
				if (Gamepad.current != null)
				{
					if (Gamepad.current.buttonNorth.wasPressedThisFrame)
						FindFirstObjectByType<CameraSwitch>().NextCamera();
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.cKey.wasPressedThisFrame)
							FindFirstObjectByType<CameraSwitch>().NextCamera();
					}
				}
				#endregion

				#region Pause
				// Pause
				if (Gamepad.current != null)
				{
					if (Gamepad.current.startButton.wasPressedThisFrame)
						FindFirstObjectByType<Pause_Menu>().Pause();
				}
				else
				{
					if (Keyboard.current != null)
					{
						if (Keyboard.current.escapeKey.wasPressedThisFrame)
							FindFirstObjectByType<Pause_Menu>().Pause();
					}
				}
				#endregion

				controller.Move(motorInput, steerInput, handBrake);
			}
        }

		public void LoadLevel(string name)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
		}

		public void Pause_Game()
		{
			FindFirstObjectByType<Pause_Menu>().Pause();
		}

		public void Switch_Camera()
		{
			FindFirstObjectByType<CameraSwitch>().NextCamera();
		}
	}
}