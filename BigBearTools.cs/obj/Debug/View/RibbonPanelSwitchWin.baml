   M S B A M L   `   `   `  ����I  EBigBearTools, Version=20.11.5.1, Culture=neutral, PublicKeyToken=null+clr-namespace:BigBearToolsBigBearTools  5      '    !BigBearTools.RibbonPanelSwitchWin   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:BigBearTools     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	     � Title$  选项卡开关��5	      $	��450��6   $	��300��6(    � 	Resources 5
       �� 
TargetType �� Property �� Value��%\  '��       '�� �     '	� T    '�� �    �� 5   
   " ��6   .�5      ��  � ��$ Right=�64   �� 5       � ��$ Center=�62   �� 5       � ��$ 0 0 5 0q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 5q�6'   �� 5       � ��$ Center=�69   �� 5       � ��$ 25��6'   �� 5       � ��$	 100��6&   5      �� 5   
   " 	�6   .�5      ��  � ��$ Center=�62   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ 70��6&   �� 5       � ��$ 21��6'   5      5      .��5      �  � RowDefinitions 5!   
   �� 5"      $
��400* �6   6,   �� 5#      $	��50* �6   6+   �� 5$      $	��50* �6   6+   .+�5&   
   &   System.Windows.Controls.DataGrid  -   5        $��RibbonDataGrid��5&      $��10q�6)      AutoGenerateColumns .     GridLinesVisibility$
 None=�6R      CanUserAddRows .  	   CanUserDeleteRows	 .  
   SelectionMode$
 Extended=�6�      SelectionUnit$ FullRow=�6�   $��16(�6�      RowHeaderWidth$ 0Q�6�      Columns 5'      4  .System.Windows.Controls.DataGridTemplateColumn  5(      ,  &System.Windows.Controls.DataGridColumn   Header$  ��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ 6* 63      CellTemplate 5)      �� 5*      .R�5+      �� -   5        $��Center=�5+      $��Center=�6�    d� 	IsChecked 6'   �� *IsOpen+ �� UpdateSourceTrigger$ PropertyChanged=�6�   5,      5.      0  *System.Windows.Controls.DataGridTextColumn  5/         
IsReadOnly . $ 选项卡名称��67   $	 50* 6j   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6F   �� *RibbonPanelName+6j   51      �� 52   
   $��1��6   $��10q�6"    �� Orientation$ 
Horizontal=�6.   $
��Left=�6G   .+�53      �� -   5        $��CheckAll��53      $��全选��6%   $��5,0,10,0q�62   6D   �� 54      -   5        $��InvertSelection��54      $��反选��6,   $��5,0,10,0q�69   6K   55      �� 57   
   $��2��6   $��10q�6"   $ 
Horizontal=�6.   $��Right=�6G   .+�58      �� -   5        $
��IsOk��58      $��确认��6!   $��5,0,10,0q�6.    �� 	IsDefault . 6Q   �� 59      -   5        $��IsCancel��59      $��取消��6%   $��5,0,0,0q�62    �� IsCancel . 6S   5:      5<      5=      