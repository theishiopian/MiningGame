namespace Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public delegate void ButtonEvent(bool pressed);

    public enum InputType
    {
        KEYBOARD,
        SCREEN,
        CONTROLLER
    }

    /**
 * This component abstracts input from different types of input devices, to reduce clutter in the player controller. Essentially a simplified version of the
 * Unity input systems that only does what I need and is easier to work with.
 *
 * Currently only supports keyboard. TODO: support touch screen, and potentially controller?
 */
    public class InputHandler : MonoBehaviour
    {
        #region SINGLETON

        //This code ensure only one instance of this class exists, and provides a static component reference to that instance.
        public static InputHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError("Duplicate Singleton: " + gameObject.name + "! Removing...");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        public InputType inputType = InputType.KEYBOARD;

        [Header("Keyboard Bindings")] public KeyCode jumpKey = KeyCode.Space;
        public KeyCode mineKey = KeyCode.LeftShift;

        public Vector2 InputVector { private set; get; }

        public Vector2 RawInputVector { private set; get; }

        public bool JumpHeld { private set; get; }

        public bool MineHeld { private set; get; }

        public bool HasInput()
        {
            return InputVector.sqrMagnitude > 0;
        }

        private Vector2 _input;

        private Vector2 _inputRaw;

        // Update is called once per frame
        private void Update()
        {
            switch (inputType)
            {
                default:
                {
                    _input.x = Input.GetAxis("Horizontal");
                    _input.y = Input.GetAxis("Vertical");

                    _inputRaw.x = Input.GetAxisRaw("Horizontal");
                    _inputRaw.y = Input.GetAxisRaw("Vertical");

                    InputVector = _input;
                    RawInputVector = _inputRaw;

                    JumpHeld = Input.GetKey(jumpKey);

                    MineHeld = Input.GetKey(mineKey);

                    break;
                }
                case InputType.SCREEN:
                {
                    throw new NotImplementedException();
                    break;
                }
                case InputType.CONTROLLER:
                {
                    throw new NotImplementedException();
                    break;
                }
            }
        }
    }
}