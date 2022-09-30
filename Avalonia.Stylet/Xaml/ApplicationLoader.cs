using System;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace Stylet.Xaml
{
    /// <summary>
    /// Added to your App.xaml, this is responsible for loading the Boostrapper you specify, and Stylet's other resources
    /// </summary>
    public class ApplicationLoader : ResourceDictionary
    {
        private readonly Style styletResourceDictionary;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApplicationLoader"/> class
        /// </summary>
        public ApplicationLoader()
        {
            this.styletResourceDictionary = new Style(); // {  Source = new Uri("pack://application:,,,/Stylet;component/Xaml/StyletResourceDictionary.xaml", UriKind.Absolute) };
            
            this.styletResourceDictionary.Add(new StyleInclude(new Uri("avares://Avalonia.Stylet/Xaml/StyletResourceDictionary.axaml", UriKind.Absolute)));
            
            // this.styletResourceDictionary.MergedDictionaries.Add(new ResourceInclude()
            // {
            //     Source = new Uri("avares://Avalonia.Stylet/Xaml/StyletResourceDictionary.axaml", UriKind.Absolute)
            // });
            
            this.LoadStyletResources = true;
        }

        private IBootstrapper _bootstrapper;

        /// <summary>
        /// Gets or sets the bootstrapper instance to use to start your application. This must be set.
        /// </summary>
        public IBootstrapper Bootstrapper
        {
            get { return this._bootstrapper; }
            set
            {
                this._bootstrapper = value;
                this._bootstrapper.Setup(Application.Current);
            }
        }

        private bool _loadStyletResources;

        /// <summary>
        /// Gets or sets a value indicating whether to load Stylet's own resources (e.g. StyletConductorTabControl). Defaults to true.
        /// </summary>
        public bool LoadStyletResources
        {
            get { return this._loadStyletResources; }
            set
            {
                this._loadStyletResources = value;
                if (this._loadStyletResources)
                    this.MergedDictionaries.Add(this.styletResourceDictionary);
                else
                    this.MergedDictionaries.Remove(this.styletResourceDictionary);
            }
        }
    }
}
