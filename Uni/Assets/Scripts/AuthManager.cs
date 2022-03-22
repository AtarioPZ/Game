using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    //Firebase Variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    //Login Variables
    [Header("Login")]
    public TMP_InputField UserName;
    public TMP_InputField UserPassword;
    public TMP_Text warningLogin;
    public TMP_Text ConfirmLogin;

    //Register Variables

    public void Awake()
    {
        //Check necessary dependencies for Firebase present on system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if(dependencyStatus == DependencyStatus.Available)
            {
                //if available, initialize
                StartFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void StartFirebase()
    {
        Debug.Log("Setting up firebase auth");

        //set auth instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    //Login
    public void BtnLogin()
    {
        StartCoroutine(Login(UserName.text, UserPassword.text));

    }

    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if(LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed";
            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "User not found";
                    break;
            }
        }
        else
        {
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully {0} ({1})", user.DisplayName, user.Email);
            SceneManager.LoadScene("Main_Lobby");
        }
    }
}
