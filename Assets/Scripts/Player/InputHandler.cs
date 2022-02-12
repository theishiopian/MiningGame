namespace Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum InputType
    {
        KEYBOARD,
        SCREEN,
        CONTROLLER
    }

    public enum ButtonAction
    {
        NONE,
        DOWN,
        HELD,
        UP
    }
    /**
     * This component abstracts input from different types of input devices, to reduce clutter in the player controller. Essentially a simplified version of the
     * Unity input systems that only does what I need and is easier to work with.
     *
     * Currently only supports keyboard. TODO: support touch screen, and potentially controller?
     */
    public class InputHandler : MonoBehaviour
    {
        public InputType inputType = InputType.KEYBOARD;

        [Header("Keyboard Bindings")] 
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode mineKey = KeyCode.RightShift;
        public KeyCode buildKey = KeyCode.LeftShift;
        public KeyCode interactKey = KeyCode.E;

        public Vector2 InputVector { private set; get; }

        public Vector2 RawInputVector { private set; get; }

        public ButtonAction Jump { get; private set; }
        public ButtonAction Mine { get; private set; }
        public ButtonAction Build { get; private set; }
        public ButtonAction Interact { get; private set; }
        public ButtonAction Click { get; private set; }
        public Vector3 ClickPos { get; private set; }//used for mouse or tap

        public bool HasDirectionalInput()
        {
            return InputVector.sqrMagnitude > 0;
        }

        private Vector2 _input;
        private Vector2 _inputRaw;

        private ButtonAction GetKeyAction(KeyCode key)
        {
            if (Input.GetKeyDown(key)) return ButtonAction.DOWN;
            if (Input.GetKey(key)) return ButtonAction.HELD;
            if (Input.GetKeyUp(key)) return ButtonAction.UP;

            return ButtonAction.NONE;
        }
        
        private ButtonAction GetMouseAction(int button)
        {
            if (Input.GetMouseButtonDown(button)) return ButtonAction.DOWN;
            if (Input.GetMouseButton(button)) return ButtonAction.HELD;
            if (Input.GetMouseButtonUp(button)) return ButtonAction.UP;

            return ButtonAction.NONE;
        }

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

                    Jump = GetKeyAction(jumpKey);
                    Mine = GetKeyAction(mineKey);
                    Build = GetKeyAction(buildKey);
                    Click = GetMouseAction(0);
                    Interact = GetKeyAction(interactKey);

                    ClickPos = PlayerController.Instance.camera.ScreenToWorldPoint(Input.mousePosition);

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