using Unity.FPS;
using UnityEngine;

namespace Unity.FPS
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float lookSensitivity = 1.0f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool invertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool invertXAxis = false;

        private PlayerCharacterController m_playerCharacterController;

        // Start is called before the first frame update
        void Start()
        {
            m_playerCharacterController = GetComponent<PlayerCharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public bool CanProcessInput() 
        {
            // Здесь можно добавлять условия типа && !gameIsEnd и т.д.
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput()) 
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput()) 
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        // Параметр чтобы можно было менять откуда принимаются данные мышь/стик/и тд.
        // сейчас только мышь
        float GetMouseLookAxis(string mouseInputName) 
        {
            if (CanProcessInput())
            {
                // Check if this look input is coming from the mouse
                //bool isGamepad = Input.GetAxis(stickInputName) != 0f;
                //float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);
                float i = Input.GetAxisRaw(mouseInputName);

                if (invertYAxis)
                    i *= -1f;

                i *= lookSensitivity;

                // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;

                return i;
            }
            return 0f;
        }

    }
}
