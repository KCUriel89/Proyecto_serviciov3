// File: Proyecto_servicio/App.xaml.cs
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using Proyecto_servicio.DataBase;

namespace Proyecto_servicio
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            try
            {
                // Ensure the shell is created here so startup XAML errors are caught
                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Startup error: {ex}");
                // Show simple error page so the app doesn't immediately crash
                MainPage = new ContentPage
                {
                    Content = new Label { Text = "Startup error: " + ex.Message }
                };
            }
        }
        
    }
}