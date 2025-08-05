using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public delegate void DelegateInput2D(Vector2 value);
public delegate void DelegateInputFloat(float value);
public delegate void DelegateInputPress(bool value);
public delegate void DelegateInputAction();

public class UserInputManager : MonoBehaviour
{
    public static event DelegateInput2D OnMoveInput;
    public static event DelegateInputPress OnJumpInput;
    public static event DelegateInputPress OnDashInput;
    public static event DelegateInputPress OnAttackInput;
    public static event DelegateInputPress OnInteractiveInput;
    public static event DelegateInputPress OnOptionInput;
    public static event DelegateInputPress OnWorldmapInput;

    public static UserInputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새로 생성된 것을 파괴
        }
    }

    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        OnMoveInput?.Invoke(input);
    }

    private void OnJump(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnJumpInput?.Invoke(isPressed);
    }

    private void OnDash(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnDashInput?.Invoke(isPressed);
    }

    private void OnAttack(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnAttackInput?.Invoke(isPressed);
    }

    private void OnInteractive(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnInteractiveInput?.Invoke(isPressed);
    }


    private void OnOption(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnOptionInput?.Invoke(isPressed);
    }

    private void OnWorldmap(InputValue value)
    {
        bool isPressed = value.isPressed;
        OnWorldmapInput?.Invoke(isPressed);
    }

}
