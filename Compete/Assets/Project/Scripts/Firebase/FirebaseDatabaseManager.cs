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

    public string PlayerDisplayName { get; private set; } 
    
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

    void Start()
    {
        // Get the root reference location of the database.
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoadData()
    {
        StartCoroutine(FetchData("username"));
    }

    private IEnumerator FetchData(string _key)
    {
        FirebaseUser user = FirebaseManager.instance.user;
        if (user == null)
        {
            yield break;
        }

        var dbTask = dbReference.Child("users").Child(user.UserId).Child(_key).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Result.Value == null)
        {
            Debug.Log("no data");
        }
        else
        {
            DataSnapshot snapshot = dbTask.Result;
            PlayerDisplayName = snapshot.Value.ToString();

            Debug.Log(PlayerDisplayName);
        }
    }

    public void SetDisplayName(string displayName)
    {
        if(displayName == "")
        {
            LoginUIManager.instance.DisplayNameOutput("Please enter a name");
            return;
        }

        PlayerDisplayName = displayName;
        StartCoroutine(SaveData("username", PlayerDisplayName));
    }

    private IEnumerator SaveData(string _key,string _value)
    {
        FirebaseUser user = FirebaseManager.instance.user;
        if (user == null)
        {
            yield break;
        }

        var dbTask = dbReference.Child("users").Child(user.UserId).Child(_key).SetValueAsync(_value);
        yield return new WaitUntil(() =>dbTask.IsCompleted);

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
            LoginUIManager.instance.CharacterSelectionScreen();
        }

    }

}
