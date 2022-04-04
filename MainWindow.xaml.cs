using QRCoder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            bdrPenSet.Visibility = Visibility.Collapsed;
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
            ppt = new();
            lblIP.Content = ppt.ip;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(ppt.ip, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Drawing.Bitmap ImageOriginalBase = qrCode.GetGraphic(20);
            BitmapImage bitmapImage = new BitmapImage();
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
            string[] pargs = Environment.GetCommandLineArgs();

            Topmost = true;
            timer.Tick += new EventHandler(timer_Tick);
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
                if (ppt.presentation != null)
                {
                    timer.Start();
                }
            }
            Excp("WELCOME", "TO SLIDECANVAS");
            InkCanvasMain.Strokes.StrokesChanged += Strokes_StrokesChanged;
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
                        string[] val = ppt.info.Split("/");
                        for (int i = 0; i < Convert.ToInt32(val[0]); i++)
                            slidestroke.Add(new StrokeCollection());
                        if (slidestroke[Convert.ToInt32(val[0])] == new StrokeCollection())
                        {
                            slidestroke[Convert.ToInt32(val[0])] = InkCanvasMain.Strokes;
                            InkCanvasMain.Strokes.Clear();
                        }
                        else
                        {
                            InkCanvasMain.Strokes = slidestroke[Convert.ToInt32(val[0])];
                        }
                        tempList.Clear();
                        btnPage.Content = ppt.info;
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


        private void ToggleBtn(object sender, RoutedEventArgs e)
        {
            ToggleVisibility(bdrPenSet, Visibility.Collapsed);
            ToggleVisibility(bdrSet, Visibility.Collapsed);
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
                    this.InkCanvasMain.EditingMode = InkCanvasEditingMode.Select;
                    break;
                case "btnPen":
                    if (this.InkCanvasMain.ActiveEditingMode == InkCanvasEditingMode.Ink)
                    {
                        bdrPenSet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFFFFFF"));
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
                default:
                    break;
            }
            ((sender as Button).Parent as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DFF4D"));
        }
        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
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
        private void ToggleVisibility(Border character)
        {
            Storyboard storyboard = new(), storyboard1 = new();
            character.Visibility = Visibility.Visible;
            storyboard = (FindResource("FadeIn") as System.Windows.Media.Animation.Storyboard);
            storyboard.SpeedRatio = 1;
            storyboard1 = (FindResource("FadeOutSlow") as System.Windows.Media.Animation.Storyboard);
            storyboard.Completed += (o, a) =>
            {
                storyboard1.Begin(character);
            };
            storyboard1.Completed += (o, a) => { character.Visibility = Visibility.Collapsed; };
            storyboard.Begin(character);
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
                if (!ppt.Zoom(20))
                    Excp("Error", "打开缩略图失败");
                ToggleBtn(btnArr, e);
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
                btnPage.Content = ppt.info;
            }
            catch (Exception ex)
            {
                Excp("Error", ex.ToString());
            }
        }
        private void Excp(string title, string content)
        {
            Border bdr = bdrInfo;
            ((bdr.Child as StackPanel).Children[0] as Label).Content = title;
            (((bdr.Child as StackPanel).Children[1] as Border).Child as Label).Content = content;
            ToggleVisibility(bdr);

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
                lblSpot.Content = "Launching...";
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                var access = tetheringManager.GetCurrentAccessPointConfiguration();
                access.Ssid = System.Net.Dns.GetHostName() + " - SlideCanvas";
                access.Passphrase = "SlideCanvas";
                var result = await tetheringManager.StartTetheringAsync();
                if (result.Status == TetheringOperationStatus.Success)
                {
                    lblSpot.Content = string.Format("SSID: {0}\nPwd: {1}", access.Ssid, access.Passphrase);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(string.Format("WIFI:T:WPA;S:{0};P:{1};;", access.Ssid, access.Passphrase), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    System.Drawing.Bitmap ImageOriginalBase = qrCode.GetGraphic(20);
                    BitmapImage bitmapImage = new BitmapImage();
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
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
                    lblSpot.Content = "Failed to Launch.";
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
                    lblSpot.Content = "Failed to Shut.";
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

        private void ArrowTog(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Name)
            { 
                case "RadSlc":
                break;
            case "RadArr":
                this.Background = Brushes.Transparent;
                break;
            default:
                break;
            }
        }

        private void ClearDrag(object sender, RoutedEventArgs e)
        {
            if(sld2Clr.Value >= 50)
                ClearCanvas(sender, e);
            ToggleVisibility(bdrClr, Visibility.Collapsed);
            sld2Clr.Value = 0;
        }
    }
}
