   M S B A M L   `   `   `  ����I  EBigBearTools, Version=20.11.5.1, Culture=neutral, PublicKeyToken=null+clr-namespace:BigBearToolsBigBearTools  5      %    BigBearTools.UndergroundPipeWin   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:BigBearTools     5      $	��200��5      $	��600��6     � 
ResizeMode$  NoResize=�6'    � Title$ 导入管道Excel表格��6=   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   6*   �� 5      $
��0.5* �6   6,    � ColumnDefinitions 5   
   �� 5      $��1* �6   6,   �� 5      $
��0.2* �6   6.   .+�5   
   �� -   5        $��FilePath��5      $��0��6"   $��0��6/   $��10q�6?   $��Center=�6K   $��Right=�6f    �� 
IsReadOnly . $��22��6�   $	��448��6�   6�   �� 5   
   -   5        $��Browse��5      $��浏览��6   $��23��6,   $��75��68   $��10q�6C   $
��Left=�6O   $��Center=�6j   $��1��6�   6�   �� 5   
   $��1��6   $��2��6"   $��10q�66    �� Orientation$ 
Horizontal=�6B   $��Right=�6[   .+�5      �� -   5        $��确定��5      $��74��6"   $��23��6-   $��	10,0,10,0q�69    �� 	IsDefault . 6a   �� 5      -   5        $��取消��5      $��74��6"   $��23��6-   $��10,0,0,0q�69    �� IsCancel . 6b   5      5      5      