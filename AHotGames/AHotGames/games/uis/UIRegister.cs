﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class UIRegister : AHotBase
{
    protected override void InitComponents()
    {
        var btnReturn = FindWidget<Button>("btnReturn");
        btnReturn.onClick.AddListener(() =>
        {
            UnloadThisUI();

            LoadAnotherUI<UILogin>();
        });

        var dErrors = new Dictionary<string, string>();
        dErrors.Add("-1", "系统错误。");
        dErrors.Add("-2", "重复的用户名。");
        dErrors.Add("-3", "无效的用户名长度。");
        dErrors.Add("-4", "无效的邮箱地址。");

        var inputUsername = FindWidget<InputField>("inputUsername");
        var inputEmail = FindWidget<InputField>("inputEmail");
        var inputPassword = FindWidget<InputField>("inputPassword");
        var inputPasswordConfirm = FindWidget<InputField>("inputPasswordConfirm");
        var btnRegister = FindWidget<Button>("btnRegister");
        btnRegister.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(inputUsername.text)
                || string.IsNullOrEmpty(inputPassword.text)
                || string.IsNullOrEmpty(inputEmail.text))
            {
                return;
            }
            if (!inputEmail.text.Contains("@")
                || !inputEmail.text.Contains(".")
                || inputEmail.text.IndexOf("@") > inputEmail.text.IndexOf("."))
            {
                UIAlert.Show("请输入合法的邮箱，这将是找回密码的唯一途径。", null, null, true);
                return;
            }
            if (inputUsername.text.Length < 4 || inputUsername.text.Length > 16)
            {
                UIAlert.Show("用户名长度应为4-16个字节。", null, null, true);
                return;
            }
            if (inputPasswordConfirm.text != inputPassword.text)
            {
                UIAlert.Show("两次输入的密码不一致，请重新输入。", null, null, true);
                return;
            }
            if (inputPassword.text.Length < 6 || inputPassword.text.Length > 16)
            {
                UIAlert.Show("密码长度应为6-16个字节。", null, null, true);
                return;
            }
            UWebSender.Instance.OnRequest(Utils.BaseURL + "accountregister"
                , "username=" + inputUsername.text
                    + "&password=" + Utils.MD5Hash(inputPassword.text)
                    + "&mail=" + inputEmail.text
                , (successRes) =>
                {
                    var jres = (JObject)JsonConvert.DeserializeObject(successRes);
                    var err = jres["err"].ToString();
                    if (err == "0")
                    {
                        UIAlert.Show("注册成功，请返回登录界面登录。", () =>
                        {
                            UnloadThisUI();

                            UILogin.CachedUsername = jres["username"].ToString();
                            LoadAnotherUI<UILogin>();
                        }, null, true);
                    }
                    else if (dErrors.ContainsKey(err))
                    {
                        UIAlert.Show("注册失败，" + dErrors[err]);
                    }
                    else
                    {
                        UIAlert.Show("注册失败，错误码：" + err + ",请重新再试。");
                    }
                }
                , (failedRes) =>
                {
                    UIAlert.Show("网络错误：" + failedRes + " 请稍后再试。");
                });
        });

    }
}

