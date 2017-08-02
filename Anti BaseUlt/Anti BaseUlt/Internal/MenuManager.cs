using System.Reflection;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace Anti_BaseUlt.Internal
{
    internal class MenuManager
    {
        public static Menu Root;

        public static void Initialize()
        {
            Root = new Menu("AntiBaseUlt", "Anti BaseUlt", true);
            {
                Root.Add(new MenuBool("Enable", "Enable"));
                Root.Add(new MenuSeperator("Version", "Version: " + Assembly.GetExecutingAssembly().GetName().Version));
            }

            Root.Attach();
        }
    }
}