![Project Icon](StyletIcon.png) Stylet.Avalonia
======================================

[英文文档](./README-EN.md)

>  请注意本项目需要 AvaloniaUI 版本 >= 0.11.0-preview

## 项目介绍

`Stylet.Avalonia`是原来[Stylet](https://github.com/canton7/Stylet)项目对[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) 框架的适配。具体介绍请查看[Stylet项目介绍](https://github.com/canton7/Stylet)

## 快速开始

第一步：创建一个AvaloniaUI框架

第二步：nuget 管理器安装 `Stylet.Avalonia`包

第三步：创建`AppBootstrapper`类，其内容如下

```c#
public class AppBootstrapper:Bootstrapper<RootViewModel>
{
    
}
```

第四步：创建`ShellViewModel`类，以及`Avalonia Window`类型名为`ShellView`的窗口组件，其内容如下【其实啥也没动】

- ShellViewModel.cs

```c#
public class ShellViewModel
{
    
}
```

- ShellView.axaml

```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Avalonia.NETCoreApp1.ShellView"
        Title="ShellView">
    Welcome to Avalonia!
</Window>
```

- ShellView.axaml.cs

```csharp
public partial class ShellView : Window
{
    public ShellView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
```



第五步：修改`App.axaml`文件，其内容如下：

```xaml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:s="using:Stylet.Xaml"
             xmlns:local="using:Avalonia.NETCoreApp1"
             x:Class="Avalonia.NETCoreApp1.App">
    <Application.Resources>
        <s:ApplicationLoader>
            <s:ApplicationLoader.Bootstrapper>
                <local:AppBootstrapper></local:AppBootstrapper>
            </s:ApplicationLoader.Bootstrapper>
        </s:ApplicationLoader>
    </Application.Resources>
    <Application.Styles>
        <FluentTheme Mode="Light"/>
    </Application.Styles>
</Application>
```



第六步：修改`App.axaml.cs`文件，其内容如下：

```c#
using Avalonia.Markup.Xaml;

namespace Avalonia.NETCoreApp1
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
    }
}
```



第七步：运行。快乐的写代码吧！





## 其他

更多资料点击[这里](https://github.com/canton7/Stylet/wiki)跳转查看。同时，可以查看本仓库中存放的示例项目。











