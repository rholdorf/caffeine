using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Caffeine
{
    public class CaffeineApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly IdleObserver _cursorObserver;

        public CaffeineApplicationContext()
        {
            Debug.WriteLine("Iniciando...");
            var icon = GetIcon("16h.png");
            _trayIcon = new NotifyIcon()
            {
                Icon = icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Sair", Exit)
            }),
                Visible = true,
                Text = "Caffeine"
            };

            _cursorObserver = new IdleObserver(_trayIcon);
            _cursorObserver.Start();
        }

        void Exit(object sender, EventArgs e)
        {
            Debug.WriteLine("Finalizando...");
            _cursorObserver.Stop();
            _trayIcon.Visible = false;
            Application.Exit();
        }

        private Icon GetIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith(resourceName));
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                using (Bitmap bitmap = (Bitmap)Image.FromStream(stream))
                {
                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
        }
    }
}
