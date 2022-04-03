using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Zack.ComObjectHelpers;

namespace SlideCanvas
{
    /* Github Reference
     * https://github.com/yangzhongke/PhoneAsPrompter
     * https://github.com/PuZhiweizuishuai/PPT-Remote-control
     */
    internal class PPTInterface
    {
        internal const int port = 7999;

        private IWebHost webHost;

        internal dynamic presentation;

        internal string ip = "", page = "", info = "";

        internal BitmapImage bim;

        private COMReferenceTracker comReference = new COMReferenceTracker();

        private static readonly string AUTH_KEY = "auth";

        private static Hashtable user = new Hashtable();
        internal PPTInterface()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("网络未连接，程序可能会运行异常，请连接网络后重试！");
            }
            // 显示IP
            ShowUrl();
            // 配置服务器
            this.webHost = new WebHostBuilder()
                .UseKestrel()
                .Configure(ConfigureWebApp)
                .UseUrls("http://*:" + port)
                .Build();

            // 异步运行服务器
            this.webHost.RunAsync();


            // 关闭窗口处理
            // this.FormClosed += Form1_FormClosed;
        }
        internal void Shut()
        {
            // 关闭所有 COM　对象，以及当前打开的PPT
            ClearComRefs();
            // 停止运行服务器
            this.webHost.StopAsync();
            this.webHost.WaitForShutdown();
            Process.GetCurrentProcess().Kill();
        }
        private void ConfigureWebApp(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                // 处理非静态请求 
                var request = context.Request;
                var response = context.Response;
                string path = request.Path.Value;
                response.ContentType = "application/json; charset=UTF-8";
                bool hasRun = true;

                IsAuth(request.Cookies, response);

                if (path == "/api/report")
                {
                    string value = request.Query["value"];
                    page = value;
                    response.StatusCode = 200;
                    await response.WriteAsync("ok");
                }
                else if (path == "/api/getNote")
                {
                    string notesText = "";
                    if (this.presentation == null)
                    {
                        return;
                    }
                    try
                    {
                        dynamic notesPage = T(T(T(T(presentation.SlideShowWindow).View).Slide).NotesPage);
                        notesText = GetInnerText(notesPage);
                    }
                    catch (COMException ex)
                    {
                        notesText = "";
                    }
                    await response.WriteAsync(notesText);
                }
                else if (path == "/api/next")
                {
                    response.StatusCode = 200;
                    if (this.presentation == null)
                    {
                        return;
                    }
                    try
                    {
                        T(T(this.presentation.SlideShowWindow).View).Next();
                        GetNum();
                        hasRun = true;
                    }
                    catch (COMException e)
                    {
                        hasRun = false;
                    }



                    if (hasRun)
                    {
                        await response.WriteAsync("OK");
                    }
                    else
                    {
                        await response.WriteAsync("NO");
                    }
                }
                else if (path == "/api/previous")
                {
                    response.StatusCode = 200;
                    if (this.presentation == null)
                    {
                        return;
                    }
                    try
                    {
                        T(T(this.presentation.SlideShowWindow).View).Previous();
                        GetNum();
                        hasRun = true;
                    }
                    catch (COMException e)
                    {
                        hasRun = false;
                    }

                    if (hasRun)
                    {
                        await response.WriteAsync("OK");
                    }
                    else
                    {
                        await response.WriteAsync("NO");
                    }

                }
                else
                {
                    response.StatusCode = 404;
                }
            });

        }
        internal void ShowUrl()
        {
            this.ip = "http://" + GetIPUtil.IPV4() + ":" + port;
            return;
        }
        private bool IsAuth(IRequestCookieCollection cookies, HttpResponse response)
        {
            foreach (var cookie in cookies)
            {
                if (cookie.Key == AUTH_KEY)
                {
                    if (user.ContainsKey(cookie.Value))
                    {
                        return true;
                    }
                    return false;
                }
            }
            CookieOptions cookieOptions = new();
            cookieOptions.HttpOnly = true;
            cookieOptions.Path = "/";
            response.Cookies.Append(AUTH_KEY, Guid.NewGuid().ToString("N"), cookieOptions);
            return false;

        }
        private void ClearComRefs()
        {
            try
            {
                if (this.presentation != null)
                {
                    T(this.presentation.Application).Quit();
                    this.presentation = null;
                }
            }
            catch (COMException ex)
            {
                Debug.WriteLine(ex);
                this.presentation = null;
            }
            this.comReference.Dispose();
            this.comReference = new COMReferenceTracker();
        }

        private dynamic T(dynamic comObj)
        {
            return this.comReference.T(comObj);
        }

        internal void OpenFile(string filename)
        {
            this.ClearComRefs();
            dynamic pptApp = T(PowerPointHelper.CreatePowerPointApplication());
            pptApp.Visible = true;
            dynamic presentations = T(pptApp.Presentations);
            this.presentation = T(presentations.Open(filename));
            T(this.presentation.SlideShowSettings).Run();
        }
        private string GetInnerText(dynamic part)
        {
            StringBuilder sb = new StringBuilder();
            dynamic shapes = T(T(part).Shapes);
            int shapesCount = shapes.Count;
            for (int i = 0; i < shapesCount; i++)
            {
                dynamic shape = T(shapes[i + 1]);
                var textFrame = T(shape.TextFrame);
                // MsoTriState.msoTrue==-1
                if (textFrame.HasText == -1)
                {
                    string text = T(textFrame.TextRange).Text;
                    sb.AppendLine(text);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        internal void FetchInnerText()
        {
            if (this.presentation == null)
            {
                MessageBox.Show("请先选择打开一个PPT文件");
                return;
            }

            dynamic notesPage = T(T(T(T(presentation.SlideShowWindow).View).Slide).NotesPage);
            string notesText = GetInnerText(notesPage);
        }
        
        internal void GetNum()
        {
            if (this.presentation == null)
            {
                info = "&#xefc8;\n导入";
            }
            try
            {
                dynamic cur = T(T(T(presentation.SlideShowWindow).View).Slide).SlideIndex;
                dynamic tot = T(presentation.Slides).Count;
                info = string.Format("{0}/{1}", cur, tot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        internal bool Prev()
        {
            if (this.presentation == null)
            {
                return false;
            }
            T(T(presentation.SlideShowWindow).View).Previous();
            return true;
        }
        internal bool Next()
        {
            if (this.presentation == null)
            {
                return false;
            }
            T(T(presentation.SlideShowWindow).View).Next();
            return true;
        }
        internal bool Zoom(int index)
        {
            if (this.presentation == null)
            {
                return false;
            }
            if (index >= 10 && index <= 400)
                try
                {
                    T(T(presentation.SlideShowWindow).View).Zoom = 30;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    try
                    {
                        presentation.SlideShowWindow.Activate();
                        SendKeys.SendWait("G");
                    }catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine(ex2.Message);
                    }
                }
            return true;
        }
    }
}
