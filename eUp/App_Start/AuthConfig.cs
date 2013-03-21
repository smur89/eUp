using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using eUp.Models;

namespace eUp
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
              //  consumerKey: "ADauTQYC6mnRBImdvMBFQ",
                //consumerSecret: "RvHPT0cMTc35xXFLB8MmIFR0de3Pj0MBV3UFYAZWzfb4");

            OAuthWebSecurity.RegisterFacebookClient(
               appId: "535858986458141",
               appSecret: "17fccec8c90134d8580c470bd76d60c6");

            OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
