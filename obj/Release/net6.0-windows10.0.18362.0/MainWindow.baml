   M S B A M L   `   `   `  ���� F  BSlideCanvas, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SlideCanvasSlideCanvas      SlideCanvas.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:SlideCanvas     D @ModernWpf, Version=0.9.4.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.4.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   -     ModernWpf.ThemeManager    IsThemeAware$
  True�� � Title$ SlideCanvas��$	��768��$
��1366�����	#01FFFFFF � WindowStyle$
 None=� � AllowsTransparency$
 True�� J� Opacity$	 1.0Q� � WindowState$ 	Maximized=� � Icon$ /SlideCanvas.png�� � 	Resources  
  FadeIn �� TargetProperty	 g� Duration	
 W� To  FadeOut  FadeOutSlow��%�   &	        &	 2     &	 d     �� .��W� $ Opacity�$	 0:0:0.15D�
 1.0�� .��W� $ Opacity�$	 0:0:0.15D�
 0.0�� .��W� $ Opacity�$	 0:0:0.8D�
 0.0.��� -    �Name$ grdMain��
������ .+��� 
������ ._��� -    ��Name$ InkCanvasMain�� �� 
Background
 ���� �  � ColumnDefinitions �� $��1* ��� $	��600 ��� $	��350 �.+���  �� CornerRadius$ 3�� - )SystemControlBackgroundAltMediumHighBrush#���  $��65��$��0��$��Bottom=�$��10,0,0,5q�$
��Left=� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 2Q�   
BlurRadius$ 6Q�   	Direction$	 -40Q�   Opacity$	 0.8Q�   RenderingBias$ Performance=�  SystemChromeHighColor   Color# �  ._�� -    �Name$ 	MenuPanel�� � 
ItemHeight$ 65�� � 	ItemWidth$ 65��.+��� $ 3��$��55��$��55��._��� -    ��Name$ btnExit��$��55��$��55��
�������$	����$��30(�$��./Resources/#iconfont)��� $ 3��$��55��$��55��._��� -   $ btnInfo��$��55��$��55��
�������$	����$��30(�$��./Resources/#iconfont)��� $ 3��#���  $��65��$��1��$��Bottom=�$��0,0,0,5q�$
��Left=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� -   $ 	ToolPanel��$ 65��$ 75��.+��� $ 3��$��55��$��65��._��� -   $ btnArr��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65���� $ 3��$��55��$��65�����	#FF4DFF4D._��� -	   $ btnPen��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65���� $ 3��$��55��$��65��._��� -
   $ btnEra��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65���� $ 3��$��55��$��65��._��� -   $ btnClr��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65���� $ 3��$��55��$��65��._��� -   $ btnUnd��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65��$��Center=��� $ 3��$��55��$��65��._��� -   $ btnSet��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��65��$��Center=��� -    ��Name$ 	bdrPenSet��$ 5��#���  $��1��$	��187��$��40,80,300,80q�$��Bottom=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._��  �� $��1* ��� $��65 �.+��� $��笔刷颜色与大小��$��
10,10,10,0q�$
��Left=�$��20(�$	��Top=�$��Microsoft YaHei UI Light)��� $��10,42,10,10q�$ 5�� 7 3SystemControlBackgroundAltHighRevealBackgroundBrush#���     $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� $ 45��$ 43��.+��� $ 5��$��40��$��40��
��� E�����	#CCCCCCCC$��1,1,1,1q�._�	� -    	� 	GroupName$ Color���� $ 5��$��40��$��40��
��� ������	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
��� ������	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
���2�2����	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
����� ����	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
����d����	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
����p�����	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� $ 5��$��40��$��40��
���   ����	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color�� d� 	IsChecked$
 TrueY��� $ 5��$��40��$��40��
����������	#CCCCCCCC$��1,1,1,1q�._�	� -   $ Color���� -   $ brdCus��$ 5��$��40��$��40�����	#CCCCCCCC$��1,1,1,1q�����  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���   � Color$  	OrangeRed��� $  Orange��! � Offset$! 0.142Q�� $  Yellow��$! 0.285Q�� $  	LimeGreen��$! 0.428Q�� $  DarkCyan��$! 0.574Q�� $  CornflowerBlue��$! 0.714Q�� $  MediumPurple��$! 0.857Q�� $  	OrangeRed��$! 1Q�._�	� -   " 	�Name$" radCus��$ Color���� $��
5,10,10,10q�$��1��$��Stretch=�$ 5��#���     $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� # � RowDefinitions# �� $��30 ��� $��1* �.+�A� $��Center=�$��0,5,5,0q� 1 -AcrylicBackgroundFillColorDefaultInverseBrush#���  $��Center=����� *Value+$ �� ElementName$$ sldSize������ *Value+$$ sldSize���� -   % ��Name$% sldSize��$��1��& �� Orientation$& Vertical=�$��5,5,5,5q�' � Maximum$' 25Q�( � SmallChange$( 5Q�) �� TickFrequency$) 5Q�* �� TickPlacement$* BottomRight=�+ � Value$+ 5Q�, � LargeChange$, 5Q�$��Stretch=�$��Stretch=��� -   $ 	bdrArrSet��$ 5��#���  $��1��$��75��$	��125��$��0,0,0,80q�$
��Left=�$��Bottom=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� .+��� $��指针��$��
10,10,10,0q�$
��Left=�$��20(�$	��Top=�$��Microsoft YaHei UI Light)��� $ 5��#���  $��Stretch=�$��Stretch=�$��5,40,5,5q����	#FFCCCCCC$��1,1,1,1q�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._��� .+�	� -   $" RadSlc��$ Arrow��$��3,5,0,0q�$
 TrueY�.��选择	� -   $" RadArr��$ Arrow��$��3,5,0,0q�.��鼠标�� -   $ bdrSet��$ 5��#���  $��1��$	��260��$	��340��$��
0,80,40,80q�$��Bottom=�$��Right=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� .+��� $��设置��$��20(�$
��Left=�$	��Top=�$��	10,10,0,0q�$��Microsoft YaHei UI Light)��� $ 5��#���  $��Stretch=�$��Stretch=�$��5,40,5,5q� ! GridViewItemRevealBorderBrush#���  $��1,1,1,1q�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�'  !ModernWpf.Controls.ScrollViewerEx  .���� $��5,5,5,5q�.+��� $��20(�$%��./Resources/#Sarasa Cascadia TC)�$y�	Collapsed=�.��This is an embedded font�� $��高级设置��$��0,5,0,5q�$��17(�$��Microsoft YaHei)��� $��5,5,5,0q�$��对齐��$��Microsoft YaHei Light)��� -   $��5,0,5,5q�- �� SelectedIndex$- 1��.@��� .��顶部�� .��底部�� $��5,5,5,0q�$��暗黑模式��$��Microsoft YaHei Light)�%  ModernWpf.Controls.ToggleSwitch  -    .  Name$. togDarkMode��$��5,0,5,5q�/   IsOn$/ False���� $��5,5,5,0q�$��橡皮阈值��$��Microsoft YaHei Light)��� -!   $% sliEra��$	' 100Q�0 � Minimum$0 10Q�$+ 20Q�$��5,0,5,5q�$( 1Q�$) 10Q�$* BottomRight=��� $��5,5,5,0q�$��平滑笔刷��$��Microsoft YaHei Light)�  $��5,0,5,5q�/ �� *%#DefaultDrawingAttributes.FitToCurve+$$ InkCanvasMain���� $��5,5,5,0q�$��禁用压感��$��Microsoft YaHei Light)�  $��5,0,5,5q�/ �� *)'DefaultDrawingAttributes.IgnorePressure+$$ InkCanvasMain���� $��远程控制��$��17(�$��Microsoft YaHei)��� $��5,5,5,5q�$"��1. 使你的设备连接WLAN��$��Microsoft YaHei Light)��� $��	10,0,10,5q�$��启动热点��$��Microsoft YaHei Light)�  -"   $��swcSpot��$��	10,0,0,10q�� -#   $��grdSpot��$��Center=�$y�	Collapsed=�.+��� -$   $��bmpSpot��$	��125��$	��125��$	��Top=��� -%   $��lblSpot��$��	0,125,0,0q�$��SSID/PWD��$��Microsoft YaHei Light)��� $��5,5,5,5q�$��2. 扫描二维码��$��Microsoft YaHei Light)�� $��Center=�.+��� -&   $��bmpIP��$	��125��$	��125��$	��Top=��� -'   $��lblIP��$��	0,125,0,0q�$��IP/PORT��$��Microsoft YaHei Light)��� -(   $ bdrClr��$ 5��#���  $��1��$	��150��$��85��$��
110,0,0,80q�$
��Left=�$��Bottom=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� .+��� $��滑动以清空��$��20(�$
��Left=�$	��Top=�$��	10,10,0,0q�$��Microsoft YaHei UI Light)��� $ 5��#���  $��Stretch=�$��Stretch=�$��5,40,5,5q�#���  $��1,1,1,1q�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._��� -)   $% sld2Clr��$��1��$& 
Horizontal=�$��5,5,5,5q�$	' 100Q�$( 10Q�$) 10Q�$
* None=�$, 50Q�$��Stretch=�$��Stretch=�1 � UseLayoutRounding$
1 True��$0 0Q��� -*   $ bdrInfo��$ 5��#���  $��2��$��	0,0,10,72q�$��Right=�$	��259��$
��Auto��$��Bottom=�$y�Visible=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._��� .+��� -+   2 ��Name$2 infTitle��$��Title��$��5,5,5,0q�$��20(�$��Stretch=�$	��Top=��� $ 5��#���  $��5,0,5,5q�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._��� -,   $2 
infContent��$��Content��$��5,5,5,5q�� $��65��$��2��$��Bottom=�$��0,0,10,5q�$��Right=�.+��� $ 3��#���  $	��200��$��Right=�   $ 2Q�$ 6Q�$	 -40Q�$	 0.8Q�$ Performance=�# �  ._�� --   $ 
SlidePanel��$ 65��$ 60��3 � Orientation$3 
Horizontal=�$��Center=�.+��� $ 3��$��55��$��55��._��� -.   $ btnPrev��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��55��$��Normal"��� $ 3��$��55��$��55��
��� E��._��� -/   $ btnPage��$��

导入��$��./Resources/#iconfont)�
�������$��55��$��55��$��Normal$�$��Normal"�$��Medium&��� $ 3��$��55��$��55��._��� -0   $ btnNext��$	����$��30(�$��./Resources/#iconfont)�
�������$��55��$��55��