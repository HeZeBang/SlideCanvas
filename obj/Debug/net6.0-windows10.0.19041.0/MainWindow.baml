   M S B A M L   `   `   `  ����F  BSlideCanvas, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SlideCanvasSlideCanvas  5   	       SlideCanvas.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:SlideCanvas     5   	   D @ModernWpf, Version=0.9.4.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.4.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	     � Title$  
MainWindow��5	   	   $	��768��6   $
��1366��6)   0  *ModernWpf.Controls.Primitives.WindowHelper   UseModernWindowStyle$
 True��5
   	   ���	#01FFFFFF � WindowStyle$
 None=�5        � AllowsTransparency$
 True��63    J� Opacity$	 1.0Q�6M    � WindowState$ 	Maximized=�6[   .��5      � 
������ .+�5   
   �� 
������ ._�5      �� -   5         ��Name$ InkCanvasMain��5       �� 
Background
 ���� 5      5      � 5   
    � ColumnDefinitions 5      �� 5      $��1* �6#   60   �� 5      $	��600 �6#   61   �� 5      $	��350 �6#   61   .+�5      �� $��3q�6   	 �� IsDirectionReversed$	 False��6    
 �� Orientation$
 
Horizontal=�6<   � 5      -   5         �Name$ 	ToolPanel��5      $��65��6+   $��1��67   $��Bottom=�6G    � 
ItemHeight$ 65��6b    � 	ItemWidth$	 100��6r   .+�5      �� -   5        $��Select��5      5      �� 5      -   5        $	��Pen��5      5      �� 5      -   5        $��Eraser��5      5       �� 5!      -   5        $��Clear��5!      5"      5#      5$      5%      5&      