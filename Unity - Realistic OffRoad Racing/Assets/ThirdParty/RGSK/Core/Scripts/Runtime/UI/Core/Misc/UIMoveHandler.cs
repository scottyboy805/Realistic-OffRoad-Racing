using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RGSK
{
    public class UIMoveHandler : MonoBehaviour, IMoveHandler
    {
        [SerializeField] UnityEvent OnMoveLeft;
        [SerializeField] UnityEvent OnMoveRight;
        [SerializeField] UnityEvent OnMoveUp;
        [SerializeField] UnityEvent OnMoveDown;

        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    {
                        OnMoveLeft.Invoke();
                        break;
                    }

                case MoveDirection.Right:
                    {
                        OnMoveRight.Invoke();
                        break;
                    }

                case MoveDirection.Up:
                    {
                        OnMoveUp.Invoke();
                        break;
                    }

                case MoveDirection.Down:
                    {
                        OnMoveDown.Invoke();
                        break;
                    }
            }

            AudioManager.Instance?.PlayUIHoverSound();
        }
    }
}