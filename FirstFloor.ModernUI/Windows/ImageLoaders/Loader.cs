﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from FirstFloor.ModernUI INC. team.
//  
// Copyrights (c) 2014 FirstFloor.ModernUI INC. All rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.Windows.ImageLoaders
{
    public static class Loader
    {
        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.RegisterAttached("SourceType", typeof(SourceType), typeof(Loader),
            new UIPropertyMetadata(SourceType.LocalDisk));
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(string), typeof(Loader),
            new UIPropertyMetadata(string.Empty, OnSourceChanged));
        public static readonly DependencyProperty DisplayWaitingAnimationDuringLoadingProperty =
            DependencyProperty.RegisterAttached("DisplayWaitingAnimationDuringLoading", typeof(bool), typeof(Loader), new UIPropertyMetadata(true));
        public static readonly DependencyProperty DisplayErrorThumbnailOnErrorProperty = DependencyProperty.RegisterAttached("DisplayErrorThumbnailOnError",
            typeof(bool), typeof(Loader), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(Loader),
            new UIPropertyMetadata(true));
        public static readonly DependencyProperty ErrorDetectedProperty = DependencyProperty.RegisterAttached("ErrorDetected", typeof(bool), typeof(Loader),
            new UIPropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static SourceType GetSourceType(Image obj)
        {
            return (SourceType) obj.GetValue(SourceTypeProperty);
        }

        public static void SetSourceType(Image obj, SourceType value)
        {
            obj.SetValue(SourceTypeProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static string GetSource(Image obj)
        {
            return (string) obj.GetValue(SourceProperty);
        }

        public static void SetSource(Image obj, string value)
        {
            obj.SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Manager.Instance.LoadImage(e.NewValue as string, d as Image);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static bool GetDisplayWaitingAnimationDuringLoading(Image obj)
        {
            return (bool) obj.GetValue(DisplayWaitingAnimationDuringLoadingProperty);
        }

        public static void SetDisplayWaitingAnimationDuringLoading(Image obj, bool value)
        {
            obj.SetValue(DisplayWaitingAnimationDuringLoadingProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static bool GetDisplayErrorThumbnailOnError(Image obj)
        {
            return (bool) obj.GetValue(DisplayErrorThumbnailOnErrorProperty);
        }

        public static void SetDisplayErrorThumbnailOnError(Image obj, bool value)
        {
            obj.SetValue(DisplayErrorThumbnailOnErrorProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static bool GetIsLoading(Image obj)
        {
            return (bool) obj.GetValue(IsLoadingProperty);
        }

        internal static void SetIsLoading(Image obj, bool value)
        {
            obj.SetValue(IsLoadingProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static bool GetErrorDetected(Image obj)
        {
            return (bool) obj.GetValue(ErrorDetectedProperty);
        }

        internal static void SetErrorDetected(Image obj, bool value)
        {
            obj.SetValue(ErrorDetectedProperty, value);
        }
    }
}