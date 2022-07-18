using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Google;
using System;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    public FirebaseAuth auth;
    public FirebaseUser user;

    private string webClientId = "934155155290-f6bkouti3iqpfrf631b71h5mo3lncfo1.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(CheckAndFixDependencies());

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private IEnumerator CheckAndFixDependencies()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        DependencyStatus dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
    }

    // Handle initialization of the necessary firebase modules:
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        StartCoroutine(CheckAutoLogin());
    }

    // Track state changes of the auth object.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user) /*Checks whether the user has changed or not*/
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.DisplayName); 
                GameManager.instance.LoadScene(0);
            }

            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.DisplayName);  
            }
        }
    }

    #region AutoLogin
    private IEnumerator CheckAutoLogin()
    {
        if (user!=null)
        {
            var task = user.ReloadAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            AutoLogin();
        }
        else
        {
            LoginUIManager.instance.LoginScreen();
            yield return null;
        }
    }

    private void AutoLogin()
    {
        Debug.Log($"Firebase user logged in : {user.DisplayName} , {user.UserId}");
        GameManager.instance.LoadScene(1);
    }

    #endregion

    #region LoginValidation
    public IEnumerator ValidateLogin(string email,string password)
    {
        Credential credential = EmailAuthProvider.GetCredential(email, password);
        var task = auth.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)task.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";
            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Please Enter Your Email";
                    break;
                case AuthError.MissingPassword:
                    output = "Please Enter Your Password";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorrect Password";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist";
                    break;
            }
            LoginUIManager.instance.LoginOutput(output);
        }
        else
        {
            Debug.Log($"Firebase user logged in : {user.DisplayName} , {user.UserId}");
            GameManager.instance.LoadScene(1);
        }
    }
    #endregion

    #region registration
    public IEnumerator ValidateRegisteration(string username,string email,string password,string confirmPassword)
    {
        if(username == "")
        {
            LoginUIManager.instance.RegistrationOutput("Please Enter a Username");
            yield return null;
        }
        else if(password == "")
        {
            LoginUIManager.instance.RegistrationOutput("Please Enter a password");
            yield return null;
        }
        else if (password.Length < 8)
        {
            LoginUIManager.instance.RegistrationOutput("Password must be minimum of 8 Characters");
            yield return null;
        }
        else if(password != confirmPassword)
        {
            LoginUIManager.instance.RegistrationOutput("Password mismatch");
            yield return null;
        }
        else
        {
            var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)task.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown Error, Please Try Again";
                switch (error)
                {
                    case AuthError.InvalidEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email Already in use";
                        break;
                }
                LoginUIManager.instance.RegistrationOutput(output);
            }
            else
            {
                StartCoroutine(UpdateUser(username));
            }
        }
    }

    private IEnumerator UpdateUser(string username)
    {
        UserProfile profile = new UserProfile()
        {
            DisplayName = username,
        };
        var task = user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            user.DeleteAsync();
            FirebaseException firebaseException = (FirebaseException)task.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown Error, Please Try Again";
            switch (error)
            {
                case AuthError.Cancelled:
                    output = "Update User Cancelled";
                    break;
                case AuthError.SessionExpired:
                    output = "Session Expired";
                    break;
            }
            LoginUIManager.instance.LoginOutput(output);
        }
        else
        {
            Debug.Log($"Firebase user created Successfully : {user.DisplayName} , {user.UserId}");
            GameManager.instance.LoadScene(1);
        }
    }
    #endregion

    #region GoogleSignin
    public IEnumerator SignInUsingGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        var task = GoogleSignIn.DefaultInstance.SignIn();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            StartCoroutine(OnAuthenticationFinished(task));
        }
    }

    private IEnumerator OnAuthenticationFinished(Task<GoogleSignInUser> _task)
    {
        Credential credential = GoogleAuthProvider.GetCredential(_task.Result.IdToken, null);
        var task = auth.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled)
        {
            Debug.LogError("SignInWithCredentialAsync was canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("Sign InWithCredentialAsync encountered an error: " + task.Exception);
        }
        else
        {
            user = task.Result;
            Debug.Log($"Firebase user created Successfully : {user.DisplayName} , {user.UserId}");
            GameManager.instance.LoadScene(1);
        }
    }
    #endregion

    public void LogOut()
    {
        auth.SignOut();

        if(GoogleSignIn.DefaultInstance != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}