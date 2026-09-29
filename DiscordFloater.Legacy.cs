using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DiscordFloater
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WindowPinningForm());
        }
    }

    internal sealed class WindowPinningForm : Form
    {
        private const int SwpNoSize = 0x0001;
        private const int SwpNoMove = 0x0002;
        private const int SwpShowWindow = 0x0040;
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private static readonly IntPtr HwndNotTopmost = new IntPtr(-2);
        private readonly ComboBox windows = new ComboBox();
        private readonly Label status = new Label();
        private WindowItem pinnedWindow;

        public WindowPinningForm()
        {
            Text = "Fijar ventana";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            ShowInTaskbar = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(440, 205);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 31, 34);
            ForeColor = Color.FromArgb(242, 243, 245);

            var heading = new Label { AutoSize = true, Font = new Font(Font.FontFamily, 12, FontStyle.Bold), Location = new Point(20, 18), Text = "Mantener una ventana en primer plano" };
            var help = new Label { AutoSize = false, Location = new Point(20, 50), Size = new Size(395, 34), Text = "Elige una ventana abierta y pulsa Fijar. Podras quitar la prioridad cuando quieras." };
            windows.DropDownStyle = ComboBoxStyle.DropDownList;
            windows.Location = new Point(20, 96);
            windows.Size = new Size(395, 24);

            var refresh = new Button { Location = new Point(20, 134), Size = new Size(100, 30), Text = "Actualizar" };
            refresh.Click += delegate { RefreshWindows(); };
            var pin = new Button { Location = new Point(130, 134), Size = new Size(130, 30), Text = "Fijar seleccionada" };
            pin.Click += delegate { PinSelectedWindow(); };
            var unpin = new Button { Location = new Point(270, 134), Size = new Size(145, 30), Text = "Quitar prioridad" };
            unpin.Click += delegate { UnpinWindow(); };
            status.AutoSize = false;
            status.Location = new Point(20, 174);
            status.Size = new Size(395, 20);
            status.Text = "Pulsa Actualizar para ver las ventanas.";

            Controls.AddRange(new Control[] { heading, help, windows, refresh, pin, unpin, status });
            RefreshWindows();
        }

        private void RefreshWindows()
        {
            windows.Items.Clear();
            var results = new List<WindowItem>();
            EnumWindows(delegate(IntPtr handle, IntPtr parameter)
            {
                if (handle == Handle || !IsWindowVisible(handle)) return true;
                var text = new StringBuilder(512);
                GetWindowText(handle, text, text.Capacity);
                if (String.IsNullOrWhiteSpace(text.ToString())) return true;
                uint processId;
                GetWindowThreadProcessId(handle, out processId);
                string processName;
                try { processName = Process.GetProcessById((int)processId).ProcessName; }
                catch { processName = "Proceso desconocido"; }
                results.Add(new WindowItem(handle, text.ToString(), processName));
                return true;
            }, IntPtr.Zero);
            results.Sort(delegate(WindowItem first, WindowItem second) { return String.Compare(first.DisplayName, second.DisplayName, StringComparison.CurrentCultureIgnoreCase); });
            foreach (var item in results) windows.Items.Add(item);
            if (windows.Items.Count > 0) windows.SelectedIndex = 0;
            status.Text = windows.Items.Count + " ventana(s) disponibles.";
        }

        private void PinSelectedWindow()
        {
            var selected = windows.SelectedItem as WindowItem;
            if (selected == null) { status.Text = "Selecciona una ventana primero."; return; }
            if (!IsWindow(selected.Handle)) { status.Text = "Esa ventana ya no esta disponible. Actualiza la lista."; return; }
            SetWindowPos(selected.Handle, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
            pinnedWindow = selected;
            status.Text = "Fijada: " + selected.Title;
        }

        private void UnpinWindow()
        {
            if (pinnedWindow == null) { status.Text = "No hay ninguna ventana fijada desde la aplicacion."; return; }
            if (IsWindow(pinnedWindow.Handle)) SetWindowPos(pinnedWindow.Handle, HwndNotTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
            status.Text = "Prioridad quitada: " + pinnedWindow.Title;
            pinnedWindow = null;
        }

        private sealed class WindowItem
        {
            public readonly IntPtr Handle;
            public readonly string Title;
            private readonly string processName;
            public string DisplayName { get { return Title + "  (" + processName + ")"; } }
            public WindowItem(IntPtr handle, string title, string process) { Handle = handle; Title = title; processName = process; }
            public override string ToString() { return DisplayName; }
        }

        private delegate bool EnumWindowsDelegate(IntPtr handle, IntPtr parameter);
        [DllImport("user32.dll")] private static extern bool EnumWindows(EnumWindowsDelegate callback, IntPtr parameter);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr handle);
        [DllImport("user32.dll")] private static extern bool IsWindow(IntPtr handle);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int GetWindowText(IntPtr handle, StringBuilder text, int maxCount);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr handle, IntPtr insertAfter, int x, int y, int width, int height, int flags);
    }
}
