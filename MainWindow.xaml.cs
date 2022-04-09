using ModernWpf;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Networking.NetworkOperators;

namespace SlideCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PPTInterface ppt;
        DispatcherTimer timer = new DispatcherTimer();
        Stack<StrokeCollection> tempList = new Stack<StrokeCollection>();
        List<StrokeCollection> slidestroke = new List<StrokeCollection>();
        public MainWindow()
        {
            InitializeComponent();

            this.btnArr.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#iconfont");

            togDarkMode.IsOn = false;
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;

            this.InkCanvasMain.EraserShape = new RectangleStylusShape(30, 30 * 16 / 9);

            CanvasFoc(this, new RoutedEventArgs());


            this.sldSize.Minimum = InkCanvasMain.DefaultDrawingAttributes.Width;
            var drawingAttributes = new DrawingAttributes()
            {
                StylusTip = StylusTip.Ellipse,
                FitToCurve = true,
                Width = this.sldSize.Value,
                Height = this.sldSize.Value
            };

            drawingAttributes.StylusTip = StylusTip.Ellipse;
            drawingAttributes.FitToCurve = true;
            InkCanvasMain.DefaultDrawingAttributes = drawingAttributes;

            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            InitAsync();

            ToggleButton(btnPen, new RoutedEventArgs());
            Excp("WELCOME", "TO SLIDECANVAS");
            InkCanvasMain.Strokes.StrokesChanged += Strokes_StrokesChanged;
        }

        private async void InitAsync()
        {
            ppt = new();
            BitmapImage bitmapImage = new BitmapImage();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(ppt.ip, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Drawing.Bitmap ImageOriginalBase = qrCode.GetGraphic(20);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            bmpIP.Source = bitmapImage;
            lblIP.Content = ppt.ip;
            Topmost = true;
            await Task.Run(() =>
            {
                string[] pargs = Environment.GetCommandLineArgs();

                if (pargs.Length > 1)
                {
                    Task task = new(() =>
                    {
                        ppt.OpenFile(pargs[1]);
                        ppt.GetNum();
                    });
                    task.Start();
                    slidestroke.Clear();
                    tempList.Clear();
                }
            });
            if (ppt.presentation != null)
            {
                timer.Start();
            }
        }
        public static T DeepCopy<T>(T obj)
        {
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
        private void Strokes_StrokesChanged(object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
        {
            if (e.Added.Count > 0)
            {
                tempList.Push(e.Added);
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ppt.presentation != null)
                {
                    ppt.GetNum();
                    if (btnPage.Content != ppt.info)
                    {
                        for (int i = 0; i < ppt.curpg; i++)
                            slidestroke.Add(new StrokeCollection());
                        if (slidestroke[ppt.curpg] == new StrokeCollection())
                        {
                            slidestroke[ppt.curpg] = InkCanvasMain.Strokes;
                            InkCanvasMain.Strokes.Clear();
                        }
                        else
                        {
                            InkCanvasMain.Strokes = slidestroke[ppt.curpg];
                        }
                        tempList.Clear();
                        btnPage.Content = ppt.curpg + "/" + ppt.totpg;
                    }
                    if (ppt.ExpUpd)
                    {
                        try
                        {
                            IEnumerable<string> files = Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory() + "\\wwwroot\\Slides", "*", SearchOption.AllDirectories);
                            int idx = 0;
                            foreach (var item in files)
                            {
                                /*idx++;
                                ListViewItem lvi = new();
                                Grid sp = new();
                                sp.Background = Brushes.Transparent;
                                sp.Children.Add(new Image() { Source = new BitmapImage(new Uri(item)), Height = 108, Width = 192 });
                                sp.Children.Add(new Label() { Content = string.Format("Slide - {0}", idx) });
                                lvi.Content = sp;
                                lvSlides.Items.Add(lvi);*/
                                lvSlides.Items.Add(new Image() { Source = new BitmapImage(new Uri(item)), Height = 108, Width = 192, Margin = new Thickness(5) });
                            }
                        }
                        catch { }
                        ppt.ExpUpd = false;
                    }
                }
            }
            catch { }
        }

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            if (this.InkCanvasMain.GetSelectedStrokes().Count != 0)
                this.InkCanvasMain.Strokes.Remove(this.InkCanvasMain.GetSelectedStrokes());
            else
                this.InkCanvasMain.Strokes.Clear();
        }
        private void CustomColor(object sender, RoutedEventArgs e)
        {
            var pck = new System.Windows.Forms.ColorDialog();
            var dialogResult = pck.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                var drawingAttributes = this.InkCanvasMain.DefaultDrawingAttributes;
                drawingAttributes.Color = Color.FromArgb(pck.Color.A, pck.Color.R, pck.Color.G, pck.Color.B);
                radCus.IsChecked = true;
                InkCanvasMain.DefaultDrawingAttributes = drawingAttributes;
            }
        }


        private void SizeSet(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var drawingAttributes = this.InkCanvasMain.DefaultDrawingAttributes;
            drawingAttributes.StylusTip = StylusTip.Ellipse;
            this.InkCanvasMain.DefaultDrawingAttributes.Width = this.sldSize.Value;
            this.InkCanvasMain.DefaultDrawingAttributes.Height = this.sldSize.Value;
            InkCanvasMain.DefaultDrawingAttributes = drawingAttributes;
        }


        private void ToggleButton(object sender, RoutedEventArgs e)
        {
            CanvasFoc(sender, new RoutedEventArgs());

            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01FFFFFF"));

            foreach (Border item in ToolPanel.Children)
                item.Background = null;
            switch ((sender as Button).Name)
            {
                case "btnArr":
                    if (this.InkCanvasMain.ActiveEditingMode == InkCanvasEditingMode.Select)
                    {
                        ToggleVisibility(bdrArrSet, Visibility.Visible);
                    }
                    else
                    {
                        this.RadArr.IsChecked = true;
                        ToggleArrow(RadArr, e);
                    }
                    break;
                case "btnPen":
                    if (this.InkCanvasMain.ActiveEditingMode == InkCanvasEditingMode.Ink)
                    {
                        bdrPenSet.Background = (Brush)this.bdrArrSet.Background;
                        ToggleVisibility(bdrPenSet, Visibility.Visible);
                    }
                    this.InkCanvasMain.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "btnEra":
                    if (this.InkCanvasMain.ActiveEditingMode == InkCanvasEditingMode.EraseByPoint)
                    {
                        ToggleVisibility(bdrClr, Visibility.Visible);
                    }
                    this.InkCanvasMain.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case "btnPage":
                    this.RadArr.IsChecked = true;
                    break;
                default:
                    break;
            }
            if ((sender as Button).Name != "btnPage")
                ((sender as Button).Parent as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DFF4D"));
            else
                (btnArr.Parent as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DFF4D"));
        }


        private void ColorPick(object sender, RoutedEventArgs e)
        {
            var drawingAttributes = this.InkCanvasMain.DefaultDrawingAttributes;
            drawingAttributes.StylusTip = StylusTip.Ellipse;
            bdrPenSet.Background = ((sender as RadioButton).Parent as Border).Background;
            drawingAttributes.Color = (Color)ColorConverter.ConvertFromString(((sender as RadioButton).Parent as Border).Background.ToString());
            InkCanvasMain.DefaultDrawingAttributes = drawingAttributes;
        }
        private void ClsPanel(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ToggleVisibility((sender as Border), Visibility.Collapsed);
        }

        private void CanvasFoc(object sender, RoutedEventArgs e)
        {
            ToggleVisibility(bdrPenSet, Visibility.Collapsed);
            ToggleVisibility(bdrSet, Visibility.Collapsed);
            ToggleVisibility(bdrArrSet, Visibility.Collapsed);
            ToggleVisibility(bdrClr, Visibility.Collapsed);
            // ToggleVisibility(bdrSlide, Visibility.Collapsed);
        }
        private void ToggleVisibility(Border character, Visibility visibility)
        {
            Storyboard storyboard = new();
            if (visibility == Visibility.Visible)
            {
                character.Visibility = visibility;
                storyboard = (FindResource("FadeIn") as System.Windows.Media.Animation.Storyboard);
            }
            else
                storyboard = (FindResource("FadeOut") as System.Windows.Media.Animation.Storyboard);
            storyboard.SpeedRatio = 1;
            storyboard.Completed += (o, a) => { character.Visibility = visibility; };
            storyboard.Begin(character);
        }
        private async void ToggleVisibility(Border character, int msec)
        {
            ToggleVisibility(character, Visibility.Visible);
            await Task.Run(() =>
            {
                Thread.Sleep(msec);
            });
            ToggleVisibility(character, Visibility.Collapsed);
        }
        private void ToggleVisibility(Border character)
        {
            if (character.Visibility == Visibility.Visible)
                ToggleVisibility(character, Visibility.Hidden);
            else
                ToggleVisibility(character, Visibility.Visible);
        }

        private void PageClick(object sender, RoutedEventArgs e)
        {
            if (ppt.presentation == null)
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "PowerPoint|*.ppt;*.pptx;*.pptm|所有文件|*.*";
                var result = openFileDialog.ShowDialog();
                if (openFileDialog.FileName != null)
                {
                    try
                    {
                        string filename = openFileDialog.FileName;
                        ppt.OpenFile(filename);
                        ppt.FetchInnerText();
                    }
                    catch (Exception ex)
                    {
                        Excp("哦吼", ex.Message);
                    }
                }
                slidestroke.Clear();
                tempList.Clear();
                timer.Start();
            }
            else
            {
                /*if (!ppt.Zoom(20))
                    Excp("Error", "打开缩略图失败");*/
                ToggleVisibility(bdrSlide);
                // ToggleButton(sender, e);
            }
        }

        private void PageChange(object sender, RoutedEventArgs e)
        {
            try
            {
                switch ((sender as Button).Name)
                {
                    case "btnPrev":
                        if (!ppt.Prev())
                            Excp("啊？", "请先打开一个PPT");
                        break;
                    case "btnNext":
                        if (!ppt.Next())
                            Excp("啊？", "请先打开一个PPT");
                        break;
                    default:
                        break;
                }
                if (ppt.presentation != null)
                    ppt.GetNum();
                btnPage.Content = ppt.curpg + "/" + ppt.totpg;
            }
            catch (Exception ex)
            {
                Excp("Error", ex.ToString());
            }
        }
        public async void Excp(string title, string content)
        {
            Border bdr = bdrInfo;
            ((bdr.Child as StackPanel).Children[0] as Label).Content = title;
            (((bdr.Child as StackPanel).Children[1] as Border).Child as Label).Content = content;
            ToggleVisibility(bdr, 2000);
        }

        private void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ppt.Shut();
        }

        private void SetShow(object sender, RoutedEventArgs e)
        {
            if (bdrSet.Visibility == Visibility.Visible)
                ToggleVisibility(bdrSet, Visibility.Collapsed);
            else
                ToggleVisibility(bdrSet, Visibility.Visible);
        }


        private async void ToggleWLAN(object sender, RoutedEventArgs e)
        {
            if (swcSpot.IsOn)
            {
                grdSpot.Visibility = Visibility.Visible;
                lblSpot.Content = "正在启动热点";
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                
                var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                var access = tetheringManager.GetCurrentAccessPointConfiguration();
                /*access.Ssid = System.Net.Dns.GetHostName() + " - SlideCanvas";
                access.Passphrase = "SlideCanvas";*/
                var result = await tetheringManager.StartTetheringAsync();
                if (result.Status == TetheringOperationStatus.Success)
                {
                    lblSpot.Content = string.Format("网络名称: {0}\n密码: {1}\n验证方式: {2}\n加密方式: {3}\n", access.Ssid, access.Passphrase, connectionProfile.NetworkSecuritySettings.NetworkAuthenticationType.ToString(), connectionProfile.NetworkSecuritySettings.NetworkEncryptionType.ToString());
                    this.lblIP.Content = ppt.ShowUrl();
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(string.Format("WIFI:T:{0};S:{1};P:{2};;", connectionProfile.NetworkSecuritySettings.NetworkEncryptionType.ToString(), access.Ssid, access.Passphrase), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    System.Drawing.Bitmap ImageOriginalBase = qrCode.GetGraphic(20);
                    BitmapImage bitmapImage = new BitmapImage();
                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }
                    bmpSpot.Source = bitmapImage;
                }
                else
                {
                    lblSpot.Content = "启动失败";
                }
            }
            else
            {
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                var result = await tetheringManager.StopTetheringAsync();
                if (result.Status == TetheringOperationStatus.Success)
                    grdSpot.Visibility = Visibility.Collapsed;
                else
                    lblSpot.Content = "关闭失败";
                grdSpot.Visibility = Visibility.Collapsed;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (tempList.Count > 0)
                InkCanvasMain.Strokes.Remove(tempList.Pop());
            else
                Excp("咩？", "已经到底了（＞人＜；）");
        }

        private void CanvasSeleChanged(object sender, EventArgs e)
        {
        }

        private void About(object sender, RoutedEventArgs e)
        {
            Excp("芜湖~", "Made by ZAMBAR~");
        }

        private void ToggleArrow(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Name)
            {
                case "RadSlc":
                    this.InkCanvasMain.EditingMode = InkCanvasEditingMode.Select;
                    this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01FFFFFF"));
                    break;
                case "RadArr":
                    this.InkCanvasMain.EditingMode = InkCanvasEditingMode.Select;
                    this.Background = Brushes.Transparent;
                    break;
                default:
                    break;
            }
        }

        private void ClearDrag(object sender, RoutedEventArgs e)
        {
            if (sld2Clr.Value >= 50)
                ClearCanvas(sender, e);
            ToggleVisibility(bdrClr, Visibility.Collapsed);
            sld2Clr.Value = 0;
        }

        private void CanvasKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Down || e.Key == System.Windows.Input.Key.PageDown || e.Key == System.Windows.Input.Key.Enter)
                PageChange(btnNext, e);
            else if (e.Key == System.Windows.Input.Key.PageUp || e.Key == System.Windows.Input.Key.Up)
                PageChange(btnPrev, e);
        }

        private void DarkMode(object sender, RoutedEventArgs e)
        {
            if (togDarkMode.IsOn)
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            else
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
        }
        private InkCanvasEditingMode prevState;
        private void CanvasTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (this.InkCanvasMain.ActiveEditingMode != InkCanvasEditingMode.EraseByPoint && this.InkCanvasMain.ActiveEditingMode != InkCanvasEditingMode.EraseByStroke)
                prevState = this.InkCanvasMain.ActiveEditingMode;
            double width = e.GetTouchPoint(null).Bounds.Width;
            if (width > this.sldSize.Value)
            {
                this.InkCanvasMain.EraserShape = new RectangleStylusShape(width, width * 16 / 9);
                InkCanvasMain.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else
            {
            }
        }

        private void CanvasPTouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            InkCanvasMain.EditingMode = prevState;
        }

        private void ChangedAlign(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex == 0)
                (ToolPanel.Parent as Border).VerticalAlignment = VerticalAlignment.Top;
            else
                (ToolPanel.Parent as Border).VerticalAlignment = VerticalAlignment.Bottom;

        }

        private void Window_Deactivated(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void ItemClick(object sender, ModernWpf.Controls.ItemClickEventArgs e)
        {
            int index = lvSlides.Items.IndexOf(e.ClickedItem);
            lvSlides.SelectedValue = index;
            ppt.Goto(index + 1);
        }

        private void RefreshIP(object sender, RoutedEventArgs e)
        {
            this.lblIP.Content = ppt.ShowUrl();
        }
    }
}
