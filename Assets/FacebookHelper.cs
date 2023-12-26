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

    internal static void GetMyInfo(Action<string> p, object v)
    {
        throw new NotImplementedException();
    }

    public delegate void OnFBInvitedSucceed(string resultJsonStr);

    private static string appLinkUrl;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init(Action action)
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    // Signal an app activation App Event
                    FB.ActivateApp();
                    // Continue with Facebook SDK
                    // ...
                    FBGetAPPLinkUrl();

                    if (action != null)
                    {
                        action();
                    }
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

            if (action != null)
            {
                action();
            }
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
        List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, (result) =>
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

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
//            FB.ShareLink(new Uri("https://play.google.com/store/apps/details?id=" + UpdateManager.Ins.verData.data.googleId), callback: ShareCallback);
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
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
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

    /// <summary>   
    /// 邀请
    /// </summary>
    public static void Invite()
    {
        //string message, IEnumerable<string> to = null, IEnumerable<object> filters = null, IEnumerable<string> excludeIds = null, int? maxRecipients = default(int?), string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null
        // FB.AppRequest(APP.facebookID,null/*to*/,null/**/,null);

        //FB.AppRequest("Come play this great game!", null, null, null, null, null, null, 
        //        delegate (IAppRequestResult result) {
        //            Debug.Log(result.RawResult); Debug.Log(result.RawResult);
        //});
    }

    /// <summary>
    /// 获取自己的信息
    /// </summary>
    /// <param name="onGotFBMyInfo"></param>
    public static void GetMyInfo(OnGotFBMyInfo onGotFBMyInfo = null, OnGetFBInfoFaild onGetFaild = null)
    {
        //Logger.LogUI("GetMyInfo");
        //没有授权
        if (FB.IsLoggedIn == false)
        {
            if (onGetFaild != null)
            {
                onGetFaild();
            }

            return;
        }

        //Logger.LogUI("API");
        FB.API("me?fields=id,name,picture", HttpMethod.GET, (result) =>
        {
            //Logger.LogUI(result.RawResult);
            if (onGotFBMyInfo != null)
            {
                onGotFBMyInfo(result.RawResult);
            }
        });
    }

    /// <summary>
    ///  获取APPLink, 获取失败，TODO
    /// </summary>
    public static void FBGetAPPLinkUrl()
    {
        FB.GetAppLink((result) =>
        {
            Debug.Log(result.RawResult);
            Debug.Log("Ref: " + result.Ref);
            Debug.Log("TargetUrl: " + result.TargetUrl);
            Debug.Log("Url: " + result.Url);
            appLinkUrl = result.Url;
            //Logger.LogUI(appLinkUrl);
        });
    }
}