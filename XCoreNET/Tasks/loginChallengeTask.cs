﻿using Global;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static XCoreNET.ClassModel.globalModel;

namespace XCoreNET.Tasks
{
    class loginChallengeTask
    {
        public loginChallengeTask()
        {
            gb.resetTokens();
        }
        public loginChallengeTask(bool isSwitching)
        {
            if (!isSwitching)
                gb.resetTokens();
        }

        public async Task<DialogResult> start()
        {
            HttpListener http = new HttpListener();
            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", "localhost", 5026);
            output("Redirect URI: " + redirectURI);
            try
            {
                // Creates an HttpListener to listen for requests on that redirect URI.
                http.Prefixes.Add(redirectURI);
                output("Listening...");
                http.Start();
                gb.httpUsing = true;



                // Waits for the OAuth authorization response.
                var context = await http.GetContextAsync();

                // Sends an HTTP response to the browser.
                var response = context.Response;
                //string responseString = string.Format("<html><head></head><body>Please return to the app.</body></html>");
                //string responseString = "<html><head><meta charset=\"utf-8\" /></head><body><div align=\"center\"><h1>工作階段已結束，請關閉此視窗並回到應用程式。</h1></div></body></html>";
                string responseString = Properties.Resources.http_response;
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
                {
                    responseOutput.Close();
                    http.Stop();
                    gb.httpUsing = false;
                    Console.WriteLine("HTTP server stopped.");
                });

                if (context.Request.QueryString.Get("code") != null)
                {
                    output(context.Request.QueryString.Get("code"));
                    gb.azureToken = context.Request.QueryString.Get("code");

                    return DialogResult.OK;
                }
                else
                {
                    output($"登入失敗: {context.Request.QueryString.Get("error")}");
                    MessageBox.Show($"登入失敗: {context.Request.QueryString.Get("error")}\n{context.Request.QueryString.Get("error_description")}", "說明", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return DialogResult.None;
                }
            }
            catch (Exception exx)
            {
                output($"登入失敗: {exx.Message}");
                MessageBox.Show($"登入失敗: {exx.Message}", "說明", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return DialogResult.None;
            }
        }
        public static BrowserInfoModel DeterminePath()
        {
            string path = String.Empty;
            string prodID = String.Empty;
            RegistryKey regKey = null;
            string ErrorMessage = null;

            try
            {
                //set the registry key we want to open
                regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice", false);

                //get rid of the enclosing quotes
                prodID = regKey.GetValue("Progid").ToString().Replace("" + (char)34, "");
                Console.WriteLine($"Prod Name: {prodID}");

                regKey = Registry.ClassesRoot.OpenSubKey(prodID + @"\shell\open\command", false);
                
                path = regKey.GetValue(null).ToString().Replace("" + (char)34, "");

                if (!path.EndsWith("exe"))
                    path = path.Substring(0, path.LastIndexOf(".exe") + 4);
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module", ex.GetType(), ex.TargetSite);
                Console.WriteLine(ErrorMessage);
            }
            finally
            {
                //check and see if the key is still open, if so
                //then close it
                if (regKey != null)
                    regKey.Close();
            }

            BrowserInfoModel bim = new BrowserInfoModel();
            bim.name = prodID;
            bim.path = path;
            return bim;
        }
        private void output(string output)
        {
            Console.WriteLine(output);
        }
    }
}
