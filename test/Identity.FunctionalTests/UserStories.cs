﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Account.Manage;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class UserStories
    {
        internal static async Task<Index> RegisterNewUserAsync(HttpClient client, string userName = null, string password = null)
        {
            userName = userName ?? $"{Guid.NewGuid()}@example.com";
            password = password ?? $"!Test.Password1$";

            var index = await Index.CreateAsync(client);
            var register = await index.ClickRegisterLinkAsync();

            return await register.SubmitRegisterFormForValidUserAsync(userName, password);
        }

        internal static async Task<Index> LoginExistingUserAsync(HttpClient client, string userName, string password)
        {
            var index = await Index.CreateAsync(client);

            var login = await index.ClickLoginLinkAsync();

            return await login.LoginValidUserAsync(userName, password);
        }

        internal static async Task<Index> RegisterNewUserWithSocialLoginAsync(HttpClient client, string userName, string email)
        {
            var index = await Index.CreateAsync(client,new DefaultUIContext().WithSocialLoginEnabled());

            var login = await index.ClickLoginLinkAsync();

            var contosoLogin = await login.ClickLoginWithContosoLinkAsync();

            var externalLogin = await contosoLogin.SendNewUserNameAsync(userName);

            return await externalLogin.SendEmailAsync(email);
        }

        internal static async Task<Index> LoginWithSocialLoginAsync(HttpClient client, string userName)
        {
            var index = await Index.CreateAsync(
                client,
                new DefaultUIContext()
                    .WithSocialLoginEnabled()
                    .WithExistingUser());

            var login = await index.ClickLoginLinkAsync();

            var contosoLogin = await login.ClickLoginWithContosoLinkAsync();

            return await contosoLogin.SendExistingUserNameAsync(userName);
        }

        internal static async Task<Index> LoginExistingUser2FaAsync(HttpClient client, string userName, string password, string twoFactorKey)
        {
            var index = await Index.CreateAsync(client);

            var loginWithPassword = await index.ClickLoginLinkAsync();

            var login2Fa = await loginWithPassword.PasswordLoginValidUserWith2FaAsync(userName, password);

            return await login2Fa.Send2FACodeAsync(twoFactorKey);
        }

        internal static async Task<ShowRecoveryCodes> EnableTwoFactorAuthentication(Index index)
        {
            var manage = await index.ClickManageLinkAsync();
            var twoFactor = await manage.ClickTwoFactorLinkAsync();
            var enableAuthenticator = await twoFactor.ClickEnableAuthenticatorLinkAsync();
            return await enableAuthenticator.SendValidCodeAsync();
        }

        internal static async Task<Index> LoginExistingUserRecoveryCodeAsync(
            HttpClient client,
            string userName,
            string password,
            string recoveryCode)
        {
            var index = await Index.CreateAsync(client);

            var loginWithPassword = await index.ClickLoginLinkAsync();

            var login2Fa = await loginWithPassword.PasswordLoginValidUserWith2FaAsync(userName, password);

            var loginRecoveryCode = await login2Fa.ClickRecoveryCodeLinkAsync();

            return await loginRecoveryCode.SendRecoveryCodeAsync(recoveryCode);
        }
    }
}