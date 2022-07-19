using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;


public class FirebaseDatabaseManager : MonoBehaviour
{
    public static FirebaseDatabaseManager instance;

    private DatabaseReference dbReference;

    private string playerDisplayName;
    private string playerCharacterID;
    private string remotePlayerDisplayName;
    private string remotePlayerCharacterID;

    public PlayerData localPlayerData;
    public PlayerData remotePlayerData;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        FirebaseManager.instance.OnFirebaseDepedenciesResolved += FirebaseManager_OnFirebaseDependenciesResolved;
    }

    private void FirebaseManager_OnFirebaseDependenciesResolved(object sender, EventArgs e)
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log(dbReference);
    }

    public void LoadData()
    {
        FirebaseUser user = FirebaseManager.instance.user;
        if (user == null) return;

        StartCoroutine(FetchData(user.UserId));
    }

    private IEnumerator FetchData(string _key)
    {
        var dbTask = dbReference.Child("users").Child(_key).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.IsFaulted || dbTask.IsCanceled)
        {
            Debug.Log("Data Fetching Faulted");
            LoginUIManager.instance.LoginScreen();
        }
        else if (dbTask.Result.Value == null)
        {
            Debug.Log("no data");
            LoginUIManager.instance.LoginScreen();
        }
        else
        {
            DataSnapshot snapshot = dbTask.Result;

            playerDisplayName = snapshot.Child("username").Value.ToString();
            Debug.Log(playerDisplayName);

            playerCharacterID = snapshot.Child("characterid").Value.ToString();
            Debug.Log(playerCharacterID);

            localPlayerData = new PlayerData
            {
                playerDisplayerName = playerDisplayName,
                playerCharacterId = int.Parse(playerCharacterID)
            };

            GameManager.instance.LoadScene(1);
        }
    }

    internal IEnumerator LoadRemotePlayerData(string nickName, object v)
    {
        throw new NotImplementedException();
    }

    public void SetDisplayName(string displayName)
    {
        if(displayName == "")
        {
            LoginUIManager.instance.DisplayNameOutput("Please enter a name");
            return;
        }

        playerDisplayName = displayName;
        StartCoroutine(SaveData("username", playerDisplayName));
    }

    public void SetSelectedCharacter(int characterid)
    {
        playerCharacterID = characterid.ToString();
        StartCoroutine(SaveData("characterid", playerCharacterID));
    }

    private IEnumerator SaveData(string _key, string _value)
    {
        FirebaseUser user = FirebaseManager.instance.user;
        if (user == null)
        {
            yield break;
        }

        var dbTask = dbReference.Child("users").Child(user.UserId).Child(_key).SetValueAsync(_value);
        yield return new WaitUntil(() => dbTask.IsCompleted);

        if(_key == "username")
        {
            if (dbTask.IsFaulted)
            {
                LoginUIManager.instance.DisplayNameOutput("Name Already Exists");

            }
            else if (dbTask.IsCanceled)
            {
                LoginUIManager.instance.DisplayNameOutput("Name Submission Cancelled");
            }
            else
            {
                dbReference.Child("usernames").Child(_value).SetValueAsync(user.UserId);
                Debug.Log("Data submission success");
                LoginUIManager.instance.CharacterSelectionScreen();
            }
        }
        else if(_key == "characterid")
        {
            if (dbTask.IsFaulted)
            {
                LoginUIManager.instance.CharacterSelectionOutput("Faulted");

            }
            else if (dbTask.IsCanceled)
            {
                LoginUIManager.instance.CharacterSelectionOutput("Submission Cancelled");
            }
            else
            {
                Debug.Log("Data submission success");

                localPlayerData = new PlayerData
                {
                    playerDisplayerName = playerDisplayName,
                    playerCharacterId = int.Parse(playerCharacterID)
                };

                GameManager.instance.LoadScene(1);
            }
        }
    }

    public IEnumerator LoadRemotePlayerData(string _key,Action callback)
    {
        var dbTask = dbReference.Child("usernames").Child(_key).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if(dbTask.Exception != null)
        {
            Debug.LogError(dbTask.Exception.Message);   
            yield break;
        }

        DataSnapshot snapshot = dbTask.Result;
        string userId = snapshot.Value.ToString();

        var dbTask1 = dbReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask1.IsCompleted);

        if (dbTask1.IsFaulted || dbTask1.IsCanceled)
        {
            Debug.Log("Remote Player Data Fetching Faulted");
        }
        else if (dbTask1.Result.Value == null)
        {
            Debug.Log("no data for remote player");
        }
        else
        {
            DataSnapshot snapshot1 = dbTask1.Result;

            remotePlayerDisplayName = snapshot1.Child("username").Value.ToString();
            remotePlayerCharacterID = snapshot1.Child("characterid").Value.ToString();

            remotePlayerData = new PlayerData
            {
                playerDisplayerName = remotePlayerDisplayName,
                playerCharacterId = int.Parse(remotePlayerCharacterID)
            };

            callback.Invoke();
        }
    }
}
