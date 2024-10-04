/*
* @Author: AiUU
* @Description: 输入控制器
* @AkanyaTech.Tinder
*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore.Common
{
    public sealed class InputController : MonoBehaviour
    {
        public Vector2 moveInput { get; private set; }

        public bool isDashing { get; private set; }

        public bool isJumping { get; private set; }

        public bool isSpecial { get; private set; }

        public void GetMoveInput(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        public void GetRunInput(InputAction.CallbackContext ctx)
        {
            isDashing = ctx.ReadValueAsButton();
        }

        public void GetSpecialInput(InputAction.CallbackContext ctx)
        {
            isSpecial = ctx.ReadValueAsButton();
        }
    }
}