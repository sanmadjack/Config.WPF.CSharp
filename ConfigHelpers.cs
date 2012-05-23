using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Config;
using System.IO;

namespace Config.WPF {
    public class ConfigHelpers {
        public static bool changeConfigFolder(Window parent, ASettings settings, string setting_name,
            string description, string error_message, Permissions required_permissions) {
            string old_path = settings.get(setting_name);
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = description;
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(GetIWin32Window(parent)) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (required_permissions < Permissions.Write 
                            ||PermissionsHelper.isWritable(new_path)) {

                                settings.set(setting_name, new_path);

                            return new_path != old_path;
                        } else {
                            folderBrowser.Description = error_message;
                            try_again = true;
                        }
                    } else {
                        folderBrowser.Description = error_message;
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }

        #region stuff for interacting with windows.forms controls
        // Ruthlessly stolen from http://stackoverflow.com/questions/315164/how-to-use-a-folderbrowserdialog-from-a-wpf-application
        private static System.Windows.Forms.IWin32Window GetIWin32Window(Window window) {
            var source = System.Windows.PresentationSource.FromVisual(window) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle) {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle {
                get { return _handle; }
            }
            #endregion
        }
        #endregion
    }
}
