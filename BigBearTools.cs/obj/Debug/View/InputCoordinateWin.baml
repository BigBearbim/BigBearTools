   M S B A M L   `   `   `  ����I  EBigBearTools, Version=20.11.5.1, Culture=neutral, PublicKeyToken=null+clr-namespace:BigBearToolsBigBearTools  5      %    BigBearTools.InputCoordinateWin   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:BigBearTools     5      $	��250��5      $	��250��6     � Title$  坐标点输入��6'   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��250* �6   6,   �� 5      $	��70* �6   6+   .+�5   
   �� $��0��6   $��坐标点输入��6    $��
10,0,10,10q�6/   .��5      �  � ColumnDefinitions 5      �� 5      $
��0.2* �6'   66   �� 5      $
��0.7* �6'   66    5      �� 5      $	��35* �6$   63   �� 5      $	��35* �6$   63   .+�5      �� $��X值:��6   $��Center=�6'   $��Right=�6B   $��10q�6^   6l   �� 5      $��Y值:��6   $��1��6'   $��Center=�64   $��Right=�6O   $��10q�6k   6y   �� 5      -   5         ��Name$ xValue��5      $��0��6*   $��0��63   $��1��6@   $
��Left=�6P   $��Center=�6k   $	��120��6�   $��23��6�   �� 5      -   5        $ yValue��5      $��0��6*   $��1��63   $��1��6@   $
��Left=�6P   $��Center=�6k   $	��120��6�   $��23��6�   5      5      �� 5   
   $��1��6   $��10q�6"    �� Orientation$ 
Horizontal=�6.   $��Right=�6G   .+�5      �� -   5        $
��IsOk��5   9   $��生成��6   $��75��6"   $��23��6-   $��	10,0,10,0q�6E    �� 	IsDefault . 6k   �� 5       -   5        $��IsCancel��5    9   $��取消��6   $��75��6"   $��23��6-   $��10,0,0,0q�6I    �� IsCancel . 6r   5!      5"      5#      