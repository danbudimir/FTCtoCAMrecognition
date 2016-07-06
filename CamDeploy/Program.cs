using System.Security;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace CamDeploy
{
    class Program
    {
        static string user;
        static SecureString pwd;
        static string url;

        static void Main(string[] args)
        {

            user = "your user name";
            pwd = GetPassword("your password");
            url = "your SharePoint URL";

            ClientContext cc = new ClientContext(url);
            cc.AuthenticationMode = ClientAuthenticationMode.Default;
            //on-prem
            cc.Credentials = new NetworkCredential(user, pwd);
            //o365
            //cc.Credentials = new SharePointOnlineCredentials(user, pwd);
            string path = CopyFileToStyleLibrary(cc);
            InjectJs(cc, "JSInjectionLink", path);
        }

        static SecureString GetPassword(string pwd)
        {
            SecureString sStrPwd = new SecureString();
            char[] x = pwd.ToCharArray();
            foreach (char c in x)
            {
                sStrPwd.AppendChar(c);
            }
            return sStrPwd;
        }

        static private byte[] getFile(string name)
        {
            //depending on where youre running this the input file could be a few levels down
            string x = Directory.GetCurrentDirectory();

            while (true)
            {
                if (System.IO.File.Exists(x + "\\"+ name))
                {
                    return System.IO.File.ReadAllBytes(x + "\\" + name);
                }
                int p = x.LastIndexOf("\\");
                if (p > -1)
                {
                    x = x.Substring(0, p);
                }
                else
                {
                    return null;
                }

            }
        }
        static string CopyFileToStyleLibrary(ClientContext cc)
        {
            var clientContext = cc;

            if (clientContext != null)
            {
                Web web = clientContext.Web;
                List styleLib = web.Lists.GetByTitle("Style Library");
                clientContext.Load(styleLib, l => l.RootFolder);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = getFile("Inject.js");
                newFile.Url = "inject.js";
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File uploadFile = styleLib.RootFolder.Files.Add(newFile);
                clientContext.Load(uploadFile);
                clientContext.ExecuteQuery();
                return styleLib.RootFolder.ServerRelativeUrl + @"/inject.js";
            }
            return null;
        }
        static public void InjectJs(ClientContext cc, string name, string path)
        {
            bool install = true;
            bool uninstalled = false;
            try
            {
                var web = cc.Web;
                string revision = Guid.NewGuid().ToString().Replace("-", "");
                string jsLink = string.Format("{0}?rev={1}", path, revision);

                StringBuilder scripts = new StringBuilder(@"
                        var headID = document.getElementsByTagName('head')[0]; 
                        var");

                scripts.AppendFormat(@"
                        newScript = document.createElement('script');
                        newScript.type = 'text/javascript';
                        newScript.src = '{0}';
                        headID.appendChild(newScript);", jsLink);
                string scriptBlock = scripts.ToString();

                var existingActions = web.UserCustomActions;
                cc.Load(existingActions);
                cc.ExecuteQuery();
                var actions = existingActions.ToArray();
                foreach (var action in actions)
                {
                    if (action.Name == name)
                    {
                        action.DeleteObject();
                        cc.ExecuteQuery();
                        uninstalled = true;
                    }
                }
                if (uninstalled)
                {
                    Console.WriteLine("The JavaScript customization has been removed, do you want to reinstall it? y/n");
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            ConsoleKey k = Console.ReadKey(true).Key;

                            if (k == ConsoleKey.Y)
                            {
                                break;
                            }
                            else if (k == ConsoleKey.N)
                            {
                                install = false;
                                break;
                            }
                        }
                    }

                }
                if (install)
                {
                    var newAction = existingActions.Add();
                    newAction.Name = name;
                    newAction.Location = "ScriptLink";

                    newAction.ScriptBlock = scriptBlock;
                    newAction.Update();
                    cc.Load(web, s => s.UserCustomActions);
                    cc.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
