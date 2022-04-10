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
using System.Threading.Tasks;
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

        internal dynamic presentation, pptApp;

        internal string ip = "", page = "", info = "";

        internal int curpg = 0, totpg = 0;

        internal bool ExpUpd = false;

        internal BitmapImage bim;

        private COMReferenceTracker comReference = new COMReferenceTracker();

        private static readonly string AUTH_KEY = "auth";

        private static Hashtable user = new Hashtable();
        internal PPTInterface()
        {
            ExpUpd = false;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("网络未连接，程序可能会运行异常，请连接网络后重试！");
            }
            // 显示IP
            _ = ShowUrl();
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
            try
            {
                // 关闭所有 COM　对象，以及当前打开的PPT
                ClearComRefs();
                // 停止运行服务器
                this.webHost.StopAsync();
                this.webHost.WaitForShutdown();
            }
            catch { }
            Process.GetCurrentProcess().Kill();
        }
        internal void Clear()
        {
            // 关闭所有 COM　对象，以及当前打开的PPT
            ClearComRefs();
            // 停止运行服务器
            this.webHost.StopAsync();
            this.webHost.WaitForShutdown();
        }
        private async void ConfigureWebApp(IApplicationBuilder app)
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
                    await Task.Run(() => { page = value; });
                    response.StatusCode = 200;
                    await response.WriteAsync("ok");
                }
                else if (path == "/api/getNote")
                {
                    string notesText = "";
                    _ = ((Func<Task>)(async () =>
                      {
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
                      })).Invoke();
                    await response.WriteAsync(notesText);
                }
                else if (path == "/api/next")
                {
                    response.StatusCode = 200;
                    await Task.Run(() =>
                    {
                        if (this.presentation == null)
                        {
                            return;
                        }
                        try
                        {
                            T(T(this.presentation.SlideShowWindow).View).Next();
                            presentation.SlideShowWindow.Activate();
                            GetNum();
                            hasRun = true;
                        }
                        catch (COMException e)
                        {
                            hasRun = false;
                        }
                    });

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
                    await Task.Run(() =>
                    {
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
                    });

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
        internal string ShowUrl()
        {
            this.ip = "http://" + GetIPUtil.IPV4() + ":" + port;
            return "http://" + GetIPUtil.IPV4() + ":" + port;
        }

        internal void Goto(int pagenum)
        {
            if(pagenum <= totpg)
            {
                try
                {
                    T(T(this.presentation.SlideShowWindow).View).GotoSlide(pagenum);
                }
                catch { }
            }
        }
        internal bool ExportAsPng()
        {
            try
            {
                if (this.presentation != null)
                {
                    T(this.presentation).Export(Directory.GetCurrentDirectory() + "\\wwwroot\\Slides", "png", 192, 108);
                    ExpUpd = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
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
        internal void ClearComRefs()
        {
            try
            {
                if (this.presentation != null)
                {
                    // T(this.presentation.Application).Quit();
                    T(T(presentation.SlideShowWindow).View).Exit();
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

        internal dynamic T(dynamic comObj)
        {
            return this.comReference.T(comObj);
        }

        internal void OpenFile(string filename)
        {
            this.ClearComRefs();
            pptApp = T(PowerPointHelper.CreatePowerPointApplication());
            pptApp.Visible = true;
            dynamic presentations = T(pptApp.Presentations);
            this.presentation = T(presentations.Open(filename));
            T(this.presentation.SlideShowSettings).Run();
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\wwwroot\\Slides");
            if (di.Exists)
            {
                try
                { di.Delete(true); }
                catch { }
                Task.Run(() =>
                {
                    ExportAsPng();
                });
            }
            else
            {
                // di.Create();
                Task.Run(() =>
                {
                    ExportAsPng();
                });
            }
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
                curpg = cur;
                totpg = tot;
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
            presentation.SlideShowWindow.Activate();
            return true;
        }
        internal bool Insert(int page)
        {
            if (this.presentation == null)
            {
                return false;
            }
            T(T(pptApp.ActivePresentation).Slides).Add(page + 1, 12);
            //http://www.360doc.com/content/19/1227/16/59724406_882555619.shtml
            return true;
        }
        internal bool Insert()
        {
            if (this.presentation == null)
            {
                return false;
            }
            T(T(pptApp.ActivePresentation).Slides).Add(curpg + 1, 12);
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
                        Task task = Task.Run(() =>
                        {
                            presentation.SlideShowWindow.Activate();
                            SendKeys.SendWait("G");
                        });
                        SendKeys.SendWait("-");
                    }
                    catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine(ex2.Message);
                        return false;
                    }
                }
            return true;
        }
    }
}
