﻿#pragma checksum "..\..\..\..\Backup\MainWindow - Copy.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A2F8ED41C31906A2FEF31FD9BB703DC1728F7C28"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPFConfigUpdater;


namespace WPFConfigUpdater {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\Backup\MainWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_klick_me;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\Backup\MainWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listview_lv;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Backup\MainWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textblock_HelloWPF;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\Backup\MainWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label textblock_MiniserverLink;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPFConfigUpdater;component/backup/mainwindow%20-%20copy.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btn_klick_me = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            this.btn_klick_me.Click += new System.Windows.RoutedEventHandler(this.Button_click);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            this.btn_klick_me.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.btn_klick_me_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.listview_lv = ((System.Windows.Controls.ListView)(target));
            
            #line 20 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            this.listview_lv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListView_SelectionChanged_1);
            
            #line default
            #line hidden
            return;
            case 3:
            this.textblock_HelloWPF = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.textblock_MiniserverLink = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            
            #line 44 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 48 "..\..\..\..\Backup\MainWindow - Copy.xaml"
            ((System.Windows.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

