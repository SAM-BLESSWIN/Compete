using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

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
    [SerializeField]
    private GameObject displayNameUI;
    [SerializeField]
    private GameObject characterSelectionUI;

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

    [Space(5f)]
    [Header("NewUser Display Name UI References")]
    [SerializeField]
    private TMP_InputField displayName;
    [SerializeField]
    private TMP_Text displayNameOutput;

    [Space(5f)]
    [Header("NewUser Character UI References")]
    [SerializeField]
    private Button[] characterBtns;
    [SerializeField]
    private Characters characters;
    [SerializeField]
    private TMP_Text characterSelectionOutput;

    private int selectedCharacterID;

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
        displayNameUI.SetActive(false);
        characterSelectionUI.SetActive(false);
        ClearOutputs();
    }

    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
        displayNameOutput.text = "";
        characterSelectionOutput.text = "";
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

    public void DisplayNameScreen()
    {
        ClearUI();
        displayNameUI.SetActive(true);
    }

    public void CharacterSelectionScreen()
    {
        ClearUI();
        characterSelectionUI.SetActive(true);
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

    public void SetDisplayName()
    {
        FirebaseDatabaseManager.instance.SetDisplayName(displayName.text);
    }

    private void clearSelection()
    {
        foreach (Button btn in characterBtns)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    public void CharacterSelected(int index)
    {
        clearSelection();
        selectedCharacterID = index;
        characterBtns[index].GetComponent<Image>().color = Color.yellow;
    }

    public void SetSelectedCharacter()
    {
        FirebaseDatabaseManager.instance.SetSelectedCharacter(selectedCharacterID);
    }

    public void LoginOutput(string output)
    {
        loginOutputText.text = output;
    }

    public void RegistrationOutput(string output)
    {
        registerOutputText.text = output;
    }

    public void DisplayNameOutput(string output)
    {
        displayNameOutput.text = output;
    }

    public void CharacterSelectionOutput(string output)
    {
        characterSelectionOutput.text = output;
    }
}
