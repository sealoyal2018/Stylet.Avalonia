![Project Icon](./StyletIcon.png) Stylet.Avalonia
======================================

[英文文档](./README-EN.md)

>  请注意本项目需要 AvaloniaUI 版本 >= 0.11.0-preview

## 项目介绍

`Stylet.Avalonia`是原来[Stylet](https://github.com/canton7/Stylet)项目对[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) 框架的适配。具体介绍请查看[Stylet项目介绍](https://github.com/canton7/Stylet)

## 快速开始

第一步：创建一个AvaloniaUI框架

第二步：nuget 管理器安装 `Stylet.Avalonia`包

第三步：创建`ShellViewModel`类，以及`Avalonia Window`类型名为`ShellView`的窗口组件，其内容如下【其实啥也没动】

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
    }
}
```



第四步：修改`App.axaml`文件，其内容如下：

```xaml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    		 xmlns:s="https://github.com/sealoyal2018/stylet.avalonia"
             xmlns:local="using:Avalonia.NETCoreApp1"
             x:Class="Avalonia.NETCoreApp1.App"
    RequestedThemeVariant="Light">
    <Application.Styles>
        <FluentTheme/>
    </Application.Styles>
</Application>
```

第五步：找到并打开`App.axaml.cs`文件，使其继承于`StyletApplication<T>`其中`T`为任一`ViewModel`，当前内容如下

```c#
public partial class App : StyletApplication<ShellViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize(); // 初始化stylet，不能去掉
    }
}
```

第七步：运行。快乐的写代码吧！

## 其他

更多资料点击[这里](https://github.com/canton7/Stylet/wiki)跳转查看。同时，可以查看本仓库中存放的示例项目。



## 从 0.0.1升级？

> 请将avalonia 升级到11.x，[升级指南](https://docs.avaloniaui.net/docs/next/stay-up-to-date/upgrade-from-0.10)

0.将`nuget`包`XamlNameReferenceGenerator`移除(新版本已内置)

1.找到并打开`App.axaml`文件，移除`AppBootstrapper`资源，即：

```xaml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <s:ApplicationLoader>
                <s:ApplicationLoader.Bootstrapper>
                    <local:AppBootstrapper  />
                </s:ApplicationLoader.Bootstrapper>
            </s:ApplicationLoader>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

2.找到并打开`App.axaml.cs`文件，使其继承于`StyletApplication<T>`, 此时`App.axaml.cs`成为了原来`AppBootstrapper<ShellViewModel>`, 将原来的`AppBootstrapper.cs`的内容移到`App.axaml.cs`文件中即可。

***其注意***：`App.axaml.cs`文件内`Initialize()`方法必须调用`base.Initialize();`

