using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    public static LoginUIManager instance;

    [Header("UI References")]
    [SerializeField]
    private GameObject checkingForAccountUI;
    [SerializeField]
    private GameObject loginUI;
    [SerializeField]
    private GameObject registerUI;

    [Space(5f)]
    [Header("Login UI References")]
    [SerializeField]
    private TMP_InputField loginEmail;
    [SerializeField]
    private TMP_InputField loginPassword;
    [SerializeField]
    private TMP_Text loginOutputText;

    [Space(5f)]
    [Header("Register NewUser UI References")]
    [SerializeField]
    private TMP_InputField registerUsername;
    [SerializeField]
    private TMP_InputField registerEmail;
    [SerializeField]
    private TMP_InputField registerPassword;
    [SerializeField]
    private TMP_InputField registerConfirmPassword;
    [SerializeField]
    private TMP_Text registerOutputText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void ClearUI()
    {
        checkingForAccountUI.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        ClearOutputs();
    }

    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }

    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void Login()
    {
        StartCoroutine(FirebaseManager.instance.ValidateLogin(loginEmail.text, loginPassword.text));
    }

    public void Register()
    {
        StartCoroutine(FirebaseManager.instance.ValidateRegisteration(registerUsername.text,
            registerEmail.text,registerPassword.text,registerConfirmPassword.text));
    }

    public void SignInWithGoogle()
    {
        StartCoroutine(FirebaseManager.instance.SignInUsingGoogle());
    }

    public void LoginOutput(string output)
    {
        loginOutputText.text = output;
    }

    public void RegistrationOutput(string output)
    {
        registerOutputText.text = output;
    }
}
