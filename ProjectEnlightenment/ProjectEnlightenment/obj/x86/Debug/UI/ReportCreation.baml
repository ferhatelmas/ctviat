   M S B A M L   `   `   `  ����  ProjectEnlightenment,    &ProjectEnlightenment.UI.ReportCreation   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  ReportCreation��5   	   $	��600��6    $	��800��6-    � WindowStyle$
 None=�69    � 
ResizeMode$ NoResize=�6L   .��5      � ��5   
   �� 5       �� ImageSource$! ../Resources/background.jpg��6    � RowDefinitions 5
   
   �� 5      $��50 �6   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      � $��0��6    5      �� 5      $
��Auto �6'   .+�5      �� $��0��6   $��Search��6(   .��5      � $��Center=�6    .+�5      �� $��Search Usernames:��6    $��14(�6<   $��Center=�6J   �� 5      -   5        $��searchTextBox��5   "   $	��150��67   $��30��6C   $��16(�6O   $��Center=�6]   6x   �� 5      -   5        $��searchButton��5   !   $��Search��65   $��60��6F   $��5,0,0,0q�6Q   $��14(�6b   6p   5      5      5       5"      � 5$   
   $��1��6   .+�5%      �� -   5        $��usersListView��5%      $��18(�6,   $��5q�6:    �� SelectionMode$ Single=�6E   6\   5&      � 5'   
   $��2��6    5(      �� 5)      $��* �6#   �� 5*      $	��130 �6#   �� 5+      $	��130 �6#   .+�5-      �� -   5        $��createButton��5-   %   $��1��6   $��5q�69   $��Create Report��6D   $��14(�6\   6j   �� 5.      -   5        $��
backButton��5.   %   $��2��6   $��5q�67   $��	Main Menu��6B   $��14(�6V   6d   5/      50      51      