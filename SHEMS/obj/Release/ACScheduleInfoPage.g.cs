﻿

#pragma checksum "E:\智能家居相关文章\SmartHomeEnergyManagementSystem\SHEMS\ACScheduleInfoPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D0092683D4B60DC49BCAA8A444A02B88"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SHEMS
{
    partial class ACScheduleInfoPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 66 "..\..\ACScheduleInfoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemClickChangeStatus;
                 #line default
                 #line hidden
                #line 66 "..\..\ACScheduleInfoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.gridView_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 63 "..\..\ACScheduleInfoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Button_Click_ChangeComfort;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


