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
        localPlayerData = new PlayerData();
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

            localPlayerData = new PlayerData
            {
                playerDisplayerName = snapshot.Child("username").Value.ToString(),
                playerCharacterId = int.Parse(snapshot.Child("characterid").Value.ToString()),
                playerTotalWins = int.Parse(snapshot.Child("totalwins").Value.ToString())
            };

            GameManager.instance.LoadScene(1);
        }
    }

    public void SetDisplayName(string displayName)
    {
        if(displayName == "")
        {
            LoginUIManager.instance.DisplayNameOutput("Please enter a name");
            return;
        }

        StartCoroutine(SaveData("username", displayName));
        UpdateTotalWins(0);
    }

    public void SetSelectedCharacter(int characterid)
    {
        StartCoroutine(SaveData("characterid", characterid.ToString()));
    }

    public void UpdateTotalWins(int wins)
    {
        StartCoroutine(SaveData("totalwins", wins.ToString()));
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

        if (_key == "totalwins")
        {
            if (dbTask.IsFaulted || dbTask.IsCanceled)
            {
                Debug.Log("Updating totalwins faulted");
            }
            else
            {
                Debug.Log("Totalwins Data submission success");
                localPlayerData.playerTotalWins = int.Parse(_value);
            }
        }
        else if (_key == "username")
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
                Debug.Log("Displayname Data submission success");
                localPlayerData.playerDisplayerName = _value;
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
                Debug.Log("Characterid Data submission success");
                localPlayerData.playerCharacterId = int.Parse(_value);
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

            remotePlayerData = new PlayerData
            {
                playerDisplayerName = snapshot1.Child("username").Value.ToString(),
                playerCharacterId = int.Parse(snapshot1.Child("characterid").Value.ToString()),
                playerTotalWins = int.Parse(snapshot1.Child("totalwins").Value.ToString())
            };

            callback.Invoke();
        }
    }
}
