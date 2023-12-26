using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System;

public class FacebookSdkTest : MonoBehaviour
{
    public Text LogText;

    public Button LoginButton;
    public Button LogoutButton;
    public Button PayButton;
    public Button ShareButton;

    void Awake()
    {
        // 初始化
        FB.Init(() =>
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                Debug.Log("Init Ok, AppId: " + FB.AppId);
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        });
    }

    void Start()
    {
        LogText.text = "";
        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            LogText.text += condition + "\n";
        };

        LoginButton.onClick.AddListener(DoLogin);
        LogoutButton.onClick.AddListener(DoLogout);
        PayButton.onClick.AddListener(DoPay);
        ShareButton.onClick.AddListener(DoShare);
    }

    private void DoLogin()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("You have logined!");
            return;
        }

        if (null != AccessToken.CurrentAccessToken &&
            AccessToken.CurrentAccessToken.ExpirationTime > System.DateTime.Now)
        {
            // 快速登录
            FB.Android.RetrieveLoginStatus((result) =>
            {
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.Log("Error: " + result.Error);
                }
                else if (result.Failed)
                {
                    Debug.Log("Failure: Access Token could not be retrieved");
                }
                else
                {
                    Debug.Log("Success: " + result.AccessToken.UserId);
                }
            });
        }
        else
        {
            // 登录
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, (result) =>
            {
                if (FB.IsLoggedIn)
                {
                    var aToken = AccessToken.CurrentAccessToken;
                    Debug.Log(aToken.UserId);
                    foreach (var perm in aToken.Permissions)
                    {
                        Debug.Log(perm);
                    }
                }
                else
                {
                    Debug.Log("User cancelled login");
                }
            });
        }
    }

    private void DoLogout()
    {
        FB.LogOut();
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Logout Successfully");
        }
        else
        {
            Debug.Log("Logout Failed");
        }
    }

    private void DoPay()
    {
        FB.Canvas.Pay("diamond", callback: (result) => { Debug.Log(result); });
    }

    public void DoShare()
    {
        FB.ShareLink(new Uri("https://play.google.com/store/apps/details?id=com.github.android"),
            callback: (result) =>
            {
                if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
                {
                    Debug.Log("ShareLink Error: " + result.Error);
                }
                else if (!string.IsNullOrEmpty(result.PostId))
                {
                    // Print post identifier of the shared content
                    Debug.Log(result.PostId);
                }
                else
                {
                    // Share succeeded without postID
                    Debug.Log("ShareLink success!");
                }
            });
    }
}