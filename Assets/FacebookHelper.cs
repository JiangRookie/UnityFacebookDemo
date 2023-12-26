using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FacebookHelper
{
    public delegate void OnFBLoginFaild(bool isCancel, string errorInfo);

    public delegate void OnFBShareLinkSucced(string postId);

    public delegate void OnFBShareLinkFaild(bool isCancel, string errorInfo);

    public delegate void OnGotFBFriendInGame(string resultJsonStr);

    public delegate void OnGotFBMyInfo(string resultJsonStr);

    public delegate void OnGetFBInfoFaild();

    public delegate void OnFBInvitedSucceed(string resultJsonStr);

    private static string appLinkUrl;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init(Action action = null)
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    FBGetAPPLinkUrl();
                    action?.Invoke();
                }
                else
                {
                    Debug.Log("Failed to Initialize the Facebook SDK");
                }
            }, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            action?.Invoke();
        }
    }

    private static void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    public static void LoginResult(Action action)
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, (result) =>
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = AccessToken.CurrentAccessToken;

                if (action != null)
                {
                    action();
                }
            }
            else
            {
                Debug.Log("User cancelled login");
            }
        });
    }

    public static void Share()
    {
        // UpdateManager.Ins.verData.data.iosId = "1459965214";

//         if (PlayerManager.Ins.IsFaceBook)
//         {
// #if UNITY_ANDROID && !UNITY_EDITOR
        FB.ShareLink(new Uri("https://play.google.com/store/apps/details?id=com.github.android"),
            callback: ShareCallback);
// #elif UNITY_IOS && !UNITY_EDITOR
//             FB.ShareLink(new Uri("https://apps.apple.com/us/app/face-meme-emoji-gif-maker/id" + UpdateManager.Ins.verData.data.iosId), callback: ShareCallback);
// #endif
//         }
//         else
//         {
//             PlayerManager.Ins.FackBookInit(() =>
//             {
//                 PlayerManager.Ins.Player.Inquire();
// #if UNITY_ANDROID && !UNITY_EDITOR
//            FB.ShareLink(new Uri("https://play.google.com/store/apps/details?id=" + UpdateManager.Ins.verData.data.googleId), callback: ShareCallback);
// #elif UNITY_IOS && !UNITY_EDITOR
//            FB.ShareLink(new Uri("https://apps.apple.com/us/app/face-meme-emoji-gif-maker/id" + UpdateManager.Ins.verData.data.iosId), callback: ShareCallback);
// #endif
//             });
//         }
    }

    private static void ShareCallback(IShareResult result)
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
    }

    public static void GetMyInfo(OnGotFBMyInfo onGotFBMyInfo = null, OnGetFBInfoFaild onGetFaild = null)
    {
        if (FB.IsLoggedIn == false)
        {
            if (onGetFaild != null)
            {
                onGetFaild();
            }

            return;
        }

        FB.API("me?fields=id,name,picture", HttpMethod.GET, (result) =>
        {
            if (onGotFBMyInfo != null)
            {
                onGotFBMyInfo(result.RawResult);
            }
        });
    }

    public static void FBGetAPPLinkUrl()
    {
        FB.GetAppLink((result) =>
        {
            Debug.Log(result.RawResult);
            Debug.Log("Ref: " + result.Ref);
            Debug.Log("TargetUrl: " + result.TargetUrl);
            Debug.Log("Url: " + result.Url);
            appLinkUrl = result.Url;
        });
    }
}