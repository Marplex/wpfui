﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WPFUI.Common;

namespace WPFUI.Appearance;

/// <summary>
/// A set of dangerous methods to modify the appearance.
/// </summary>
[Obsolete("This class is not depracted, but is dangerous to use.")]
public static class UnsafeNativeMethods
{
    #region Window Corners

    /// <summary>
    /// Tries to set the <see cref="System.Windows.Window"/> corner preference.
    /// </summary>
    /// <param name="window">Selected window.</param>
    /// <param name="cornerPreference">Window corner preference.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowCornerPreference(System.Windows.Window window, WindowCornerPreference cornerPreference)
        => GetHandle(window, out IntPtr windowHandle) && ApplyWindowCornerPreference(windowHandle, cornerPreference);

    /// <summary>
    /// Tries to set the corner preference of the selected window.
    /// </summary>
    /// <param name="handle">Selected window handle.</param>
    /// <param name="cornerPreference">Window corner preference.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowCornerPreference(IntPtr handle, WindowCornerPreference cornerPreference)
    {
        var dmwaCornerPreference = cornerPreference switch
        {
            WindowCornerPreference.DoNotRound => Interop.Dwmapi.DWM_WINDOW_CORNER_PREFERENCE.DONOTROUND,
            WindowCornerPreference.Round => Interop.Dwmapi.DWM_WINDOW_CORNER_PREFERENCE.ROUND,
            WindowCornerPreference.RoundSmall => Interop.Dwmapi.DWM_WINDOW_CORNER_PREFERENCE.ROUNDSMALL,
            _ => Interop.Dwmapi.DWM_WINDOW_CORNER_PREFERENCE.DEFAULT
        };

        int pvAttribute = (int)dmwaCornerPreference;

        // TODO: Validate HRESULT
        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
            ref pvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    #endregion

    #region Window Immersive Dark Mode

    /// <summary>
    /// Tries to remove ImmersiveDarkMode effect from the <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window to which the effect is to be applied.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowDarkMode(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && RemoveWindowDarkMode(windowHandle);

    /// <summary>
    /// Tries to remove ImmersiveDarkMode effect from the window handle.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowDarkMode(IntPtr handle)
    {
        var pvAttribute = 0x0; // Disable
        var dwAttribute = Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (Common.Windows.IsBelow(WindowsRelease.Windows10Insider1))
            dwAttribute = Interop.Dwmapi.DWMWINDOWATTRIBUTE.DMWA_USE_IMMERSIVE_DARK_MODE_OLD;

        // TODO: Validate HRESULT
        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            dwAttribute,
            ref pvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    /// <summary>
    /// Tries to apply ImmersiveDarkMode effect for the <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window to which the effect is to be applied.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowDarkMode(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && ApplyWindowDarkMode(windowHandle);

    /// <summary>
    /// Tries to apply ImmersiveDarkMode effect for the window handle.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowDarkMode(IntPtr handle)
    {
        var pvAttribute = 0x1; // Enable
        var dwAttribute = Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (Common.Windows.IsBelow(WindowsRelease.Windows10Insider1))
            dwAttribute = Interop.Dwmapi.DWMWINDOWATTRIBUTE.DMWA_USE_IMMERSIVE_DARK_MODE_OLD;

        // TODO: Validate HRESULT
        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            dwAttribute,
            ref pvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    #endregion

    #region Window Titlebar

    /// <summary>
    /// Tries to remove titlebar from selected <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window to which the effect is to be applied.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowTitlebar(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && RemoveWindowTitlebar(windowHandle);

    /// <summary>
    /// Tries to remove titlebar from selected window handle.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowTitlebar(IntPtr handle)
    {
        int windowStyleLong = Interop.User32.GetWindowLong(handle, Interop.User32.GWL.GWL_STYLE);
        windowStyleLong &= ~0x80000; // NativeMethods.Interop.User32.WS.POPUP

        var result = Interop.User32.SetWindowLong(handle, Interop.User32.GWL.GWL_STYLE, windowStyleLong);

        return result > 0x0;
    }

    #endregion

    #region Remove Window DMWA backdrop effects

    /// <summary>
    /// Tries to remove backdrop effect from the <see cref="Window"/>.
    /// </summary>
    /// <param name="window">Selected Window.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowBackdrop(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && RemoveWindowBackdrop(windowHandle);

    /// <summary>
    /// Tries to remove backdrop effect from the window handle.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool RemoveWindowBackdrop(IntPtr handle)
    {
        var pvAttribute = 0x0; // Disable
        var backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_DISABLE;

        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            ref pvAttribute,
            Marshal.SizeOf(typeof(int)));

        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    #endregion

    #region Initial Windows 11 Mica

    /// <summary>
    /// Tries to apply legacy Mica effect for the selected <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window to which the effect is to be applied.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowLegacyMicaEffect(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && ApplyWindowLegacyMicaEffect(windowHandle);

    /// <summary>
    /// Tries to apply legacy Mica effect for the selected <see cref="Window"/>.
    /// </summary>
    /// <param name="handle">Window handle</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowLegacyMicaEffect(IntPtr handle)
    {
        var backdropPvAttribute = 0x1; //Enable

        // TODO: Validate HRESULT
        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    #endregion

    #region Window Legacy Acrylic

    /// <summary>
    /// Tries to apply legacy Acrylic effect for the selected <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window to which the effect is to be applied.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowLegacyAcrylicEffect(System.Windows.Window window)
        => GetHandle(window, out IntPtr windowHandle) && ApplyWindowLegacyAcrylicEffect(windowHandle);

    /// <summary>
    /// Tries to apply legacy Acrylic effect for the selected <see cref="Window"/>.
    /// </summary>
    /// <param name="handle">Window handle</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowLegacyAcrylicEffect(IntPtr handle)
    {
        // TODO
        return false;
        //if (Common.Windows.Is(WindowsRelease.Windows11Insider1))
        //{
        //    if (!UnsafeNativeMethods.RemoveWindowTitlebar(handle))
        //        return false;

        //    int backdropPvAttribute = (int)NativeMethods.Interop.Dwmapi.DWMSBT.DWMSBT_TRANSIENTWINDOW;

        //    NativeMethods.Interop.Dwmapi.DwmSetWindowAttribute(
        //        handle,
        //        NativeMethods.Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
        //        ref backdropPvAttribute,
        //        Marshal.SizeOf(typeof(int)));

        //    if (!AppearanceData.Handlers.Contains(handle))
        //        AppearanceData.Handlers.Add(handle);

        //    return true;
        //}

        //if (Common.Windows.Is(WindowsRelease.Windows10V20H1))
        //{
        //    //TODO: We need to set window transparency to True

        //    var accentPolicy = new NativeMethods.Interop.User32.ACCENT_POLICY
        //    {
        //        AccentState = NativeMethods.Interop.User32.ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
        //        GradientColor = (0 << 24) | (0x990000 & 0xFFFFFF)
        //    };

        //    int accentStructSize = Marshal.SizeOf(accentPolicy);

        //    IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
        //    Marshal.StructureToPtr(accentPolicy, accentPtr, false);

        //    var data = new NativeMethods.Interop.User32.WINCOMPATTRDATA
        //    {
        //        Attribute = NativeMethods.Interop.User32.WINCOMPATTR.WCA_ACCENT_POLICY,
        //        SizeOfData = accentStructSize,
        //        Data = accentPtr
        //    };

        //    NativeMethods.Interop.User32.SetWindowCompositionAttribute(handle, ref data);

        //    Marshal.FreeHGlobal(accentPtr);

        //    return true;
        //}

        //return false;
    }

    #endregion

    #region Window Backdrop Effect

    /// <summary>
    /// Tries to apply selected backdrop type for <see cref="Window"/>
    /// </summary>
    /// <param name="window">Selected window.</param>
    /// <param name="backgroundType">Backdrop type.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowBackdrop(System.Windows.Window window, BackgroundType backgroundType)
        => GetHandle(window, out IntPtr windowHandle) && ApplyWindowBackdrop(windowHandle, backgroundType);

    /// <summary>
    /// Tries to apply selected backdrop type for window handle.
    /// </summary>
    /// <param name="handle">Selected window handle.</param>
    /// <param name="backgroundType">Backdrop type.</param>
    /// <returns><see langword="true"/> if invocation of native Windows function succeeds.</returns>
    public static bool ApplyWindowBackdrop(IntPtr handle, BackgroundType backgroundType)
    {
        var backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_DISABLE;

        switch (backgroundType)
        {
            case BackgroundType.Auto:
                backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_AUTO;
                break;

            case BackgroundType.Mica:
                backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_MAINWINDOW;
                break;

            case BackgroundType.Acrylic:
                backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_TRANSIENTWINDOW;
                break;

            case BackgroundType.Tabbed:
                backdropPvAttribute = (int)Interop.Dwmapi.DWMSBT.DWMSBT_TABBEDWINDOW;
                break;
        }

        if (backdropPvAttribute == (int)Interop.Dwmapi.DWMSBT.DWMSBT_DISABLE)
            return false;

        // TODO: Validate HRESULT
        Interop.Dwmapi.DwmSetWindowAttribute(
            handle,
            Interop.Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    #endregion

    #region DMWA Colorization

    /// <summary>
    /// Tries to get currently selected Window accent color.
    /// </summary>
    public static Color GetDwmColor()
    {
        Interop.Dwmapi.DwmGetColorizationParameters(out var dwmParams);

        var values = BitConverter.GetBytes(dwmParams.clrColor);

        return Color.FromArgb(
            255,
            values[2],
            values[1],
            values[0]
        );
    }

    #endregion

    /// <summary>
    /// Tries to get the pointer to the window handle.
    /// </summary>
    /// <param name="window"></param>
    /// <param name="windowHandle"></param>
    /// <returns><see langword="true"/> if the handle is not <see cref="IntPtr.Zero"/>.</returns>
    private static bool GetHandle(System.Windows.Window window, out IntPtr windowHandle)
    {
        windowHandle = new WindowInteropHelper(window).Handle;

        return windowHandle != IntPtr.Zero;
    }
}
