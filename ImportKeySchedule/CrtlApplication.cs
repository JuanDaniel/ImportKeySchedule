using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using ImportKeySchedule;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Threading;
using ImportKeySchedule.Resources;

namespace BBI.JD
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        private UIApplication application;
        private static CultureInfo culture = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            application = commandData.Application;

            try
            {
                ShowForm();
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public static CultureInfo CultureByDefault()
        {
            if (culture == null)
            {
                String[] culturesPlugin = { "ENU", "ESN", "FRA" };
                CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;

                if (culturesPlugin.Contains(uiCulture.ThreeLetterWindowsLanguageName)){
                    culture = uiCulture;
                }
                else
                {
                    culture = new CultureInfo("en-US");
                }
            }

            return culture;
        }

        private void ShowForm()
        {
            using (var form = new Forms.ImportKeySchedule(application))
            {
                form.ShowDialog();
            }
        }
    }

    public class CrtlApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string folder = new FileInfo(assemblyPath).Directory.FullName;
            CultureInfo culture = Command.CultureByDefault();

            // Create a customm ribbon tab
            string tabName = "BBI";
            Autodesk.Windows.RibbonTab tab = CreateRibbonTab(application, tabName);

            // Add new ribbon panel
            string panelName = ImportKeyScheduleResource.ResourceManager.GetString("KeySchedule", culture);
            RibbonPanel ribbonPanel = CreateRibbonPanel(application, tab, panelName);

            // Create a push button in the ribbon panel
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData(
                "ImportKeySchedule",
                ImportKeyScheduleResource.ResourceManager.GetString("KeyScheduleButton", culture), 
                assemblyPath, "BBI.JD.Command")) as PushButton;

            // Set the large image shown on button
            Uri uriImage = new Uri(string.Concat(folder, "/icon_32x32.png"));
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private Autodesk.Windows.RibbonTab CreateRibbonTab(UIControlledApplication application, string tabName)
        {
            Autodesk.Windows.RibbonTab tab = Autodesk.Windows.ComponentManager.Ribbon.Tabs.FirstOrDefault(x => x.Id == tabName);

            if (tab == null)
            {
                application.CreateRibbonTab(tabName);

                tab = Autodesk.Windows.ComponentManager.Ribbon.Tabs.FirstOrDefault(x => x.Id == tabName);
            }

            return tab;
        }

        private RibbonPanel CreateRibbonPanel(UIControlledApplication application, Autodesk.Windows.RibbonTab tab, string panelName)
        {
            RibbonPanel panel = application.GetRibbonPanels(tab.Name).FirstOrDefault(x => x.Name == panelName);

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tab.Name, panelName);
            }

            return panel;
        }
    }
}
