using System;
using System.IO;
using System.Runtime.InteropServices;

namespace libzeta {

    /// <summary>
    /// StbImage wrapper.
    /// </summary>
    public static class StbImage {

        #region Constants
        const string LibOSX     = "stb_image.dylib";
        const string LibWin32   = "stb_image-win32.dll";
        const string LibWin64   = "stb_image-win64.dll";
        const string LibLinux32 = "stb_image-i686.so";
        const string LibLinux64 = "stb_image-x86_64.so";
        const string FuncLoad   = "stbi_load";
        const string FuncFree   = "stbi_image_free";
        #endregion

        delegate IntPtr LoadDelegate (string filename, ref int x, ref int y, ref int n, int req_comp);
        delegate void FreeDelegate (IntPtr data);

        public static IntPtr LoadImage (string filename, ref int x, ref int y, ref int n, int req_comp) {
            LoadDelegate loadFunction;
            switch (Environment.OSVersion.Platform) {
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
                loadFunction = Environment.Is64BitProcess ? (LoadDelegate) SafeNativeMethods.Win64.Load : SafeNativeMethods.Win32.Load;
                break;
            case PlatformID.Unix:
                loadFunction = Environment.Is64BitProcess ? (LoadDelegate) SafeNativeMethods.Linux64.Load : SafeNativeMethods.Linux32.Load;
                break;
            case PlatformID.MacOSX:
                loadFunction = SafeNativeMethods.OSX.Load;
                break;
            default:
                if (Directory.Exists ("/Library") && Directory.Exists ("/System")) {
                    loadFunction = SafeNativeMethods.OSX.Load;
                } else {
                    loadFunction = Environment.Is64BitProcess ? (LoadDelegate) SafeNativeMethods.Linux64.Load : SafeNativeMethods.Linux32.Load;
                }
                break;
            }
            return loadFunction (filename, ref x, ref y, ref n, req_comp);
        }

        public static void FreeImage (IntPtr data) {
            FreeDelegate freeFunction;
            switch (Environment.OSVersion.Platform) {
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
                freeFunction = Environment.Is64BitProcess ? (FreeDelegate) SafeNativeMethods.Win64.Free : SafeNativeMethods.Win32.Free;
                break;
            case PlatformID.Unix:
                freeFunction = Environment.Is64BitProcess ? (FreeDelegate) SafeNativeMethods.Linux64.Free : SafeNativeMethods.Linux32.Free;
                break;
            case PlatformID.MacOSX:
                freeFunction = SafeNativeMethods.OSX.Free;
                break;
            default:
                if (Directory.Exists ("/Library") && Directory.Exists ("/System")) {
                    freeFunction = SafeNativeMethods.OSX.Free;
                } else {
                    freeFunction = Environment.Is64BitProcess ? (FreeDelegate) SafeNativeMethods.Linux64.Free : SafeNativeMethods.Linux32.Free;
                }
                break;
            }
            freeFunction (data);
        }

        class SafeNativeMethods {

            internal class Win32 {
                [DllImport (LibWin32, EntryPoint = FuncLoad, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern IntPtr Load ([MarshalAs (UnmanagedType.AnsiBStr)] string filename, ref int x, ref int y, ref int n, int req_comp);
                [DllImport (LibWin32, EntryPoint = FuncFree, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern void Free (IntPtr data);
            }

            internal class Win64 {
                [DllImport (LibWin64, EntryPoint = FuncLoad, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern IntPtr Load ([MarshalAs (UnmanagedType.AnsiBStr)] string filename, ref int x, ref int y, ref int n, int req_comp);
                [DllImport (LibWin64, EntryPoint = FuncFree, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern void Free (IntPtr data);
            }

            internal class Linux32 {
                [DllImport (LibLinux32, EntryPoint = FuncLoad, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern IntPtr Load ([MarshalAs (UnmanagedType.AnsiBStr)] string filename, ref int x, ref int y, ref int n, int req_comp);
                [DllImport (LibLinux32, EntryPoint = FuncFree, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern void Free (IntPtr data);
            }

            internal class Linux64 {
                [DllImport (LibLinux64, EntryPoint = FuncLoad, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern IntPtr Load ([MarshalAs (UnmanagedType.AnsiBStr)] string filename, ref int x, ref int y, ref int n, int req_comp);
                [DllImport (LibLinux64, EntryPoint = FuncFree, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern void Free (IntPtr data);
            }

            internal class OSX {
                [DllImport (LibOSX, EntryPoint = FuncLoad, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern IntPtr Load ([MarshalAs (UnmanagedType.AnsiBStr)] string filename, ref int x, ref int y, ref int n, int req_comp);
                [DllImport (LibOSX, EntryPoint = FuncFree, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
                public static extern void Free (IntPtr data);
            }
        }
    }
}

