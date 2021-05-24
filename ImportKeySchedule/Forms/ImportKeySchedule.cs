using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImportKeySchedule;
using System.Reflection;
using System.Globalization;
using ImportKeySchedule.Resources;

namespace BBI.JD.Forms
{
    public partial class ImportKeySchedule : System.Windows.Forms.Form
    {
        private CultureInfo culture;
        private UIApplication application;
        private UIDocument uiDoc;
        private Document document;
        private List<ViewSchedule> viewSchedules;
        private ViewSchedule schedule;
        private List<string> columnsSchedule;
        private List<string> columnsExcel;

        public ImportKeySchedule(UIApplication application)
        {
            culture = Command.CultureByDefault();

            InitializeComponent();

            this.application = application;
            uiDoc = application.ActiveUIDocument;
            document = uiDoc.Document;
        }

        private void ImportKeySchedule_Load(object sender, EventArgs e)
        {
            viewSchedules = new FilteredElementCollector(document)
                        .OfClass(typeof(ViewSchedule))
                            .Cast<ViewSchedule>()
                                .Where(x => x.Definition.IsKeySchedule) // Get only Key Schedule
                                    .OrderBy(x => x.Name)
                                        .ToList();

            bool found = false;

            for (int i = 0; i < viewSchedules.Count; i++)
            {
                ViewSchedule schedule = viewSchedules[i];

                // Remove <Revision Schedule> kind
                if (schedule.Name.Contains("<Revision Schedule>"))
                {
                    viewSchedules.RemoveAt(i--);

                    continue;
                }

                lst_KeySchedules.Items.Add(schedule.Name);

                // Try to auto-select default Key Schedule
                if (!found)
                {
                    if (found = schedule.Name.Contains("Clave.Programa"))
                    {
                        lst_KeySchedules.SelectedItem = schedule.Name;
                    }
                }
            }

            lst_KeySchedules.Enabled = true;
        }

        private void lst_KeySchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_Ok.Enabled = ActiveOk();
            LoadScheduleStructure();
        }

        private void txt_ExcelPath_TextChanged(object sender, EventArgs e)
        {
            btn_Ok.Enabled = ActiveOk();

            if (!string.IsNullOrEmpty(txt_ExcelPath.Text))
            {
                LoadExcelStructure();
            }
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                txt_ExcelPath.Text = openFileDialog1.FileName;
            }
        }

        private void chk_Purge_CheckedChanged(object sender, EventArgs e)
        {
            chk_Overwrite.Enabled = !(sender as CheckBox).Checked;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            if (lst_KeySchedules.SelectedIndex < 0)
            {
                MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("SelectKeyScheduleDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("SelectKeySchedule", culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            if (!MatchScheduleStructure())
            {
                MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("UnmatchedScheduleDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("UnmatchedSchedule", culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            bool purge = false;

            if (chk_Purge.Checked)
            {
                DialogResult dialog = MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("PurgeDataDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("PurgeData", culture),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                purge = dialog == DialogResult.Yes;
            }

            if (!ValidPID() && !purge)
            {
                DialogResult dialog = MessageBox.Show(
                    string.Format("{0}\n{1}",
                        ImportKeyScheduleResource.ResourceManager.GetString("InvalidPIDDesc", culture),
                        ImportKeyScheduleResource.ResourceManager.GetString("InvalidPIDDesc1", culture)),
                    ImportKeyScheduleResource.ResourceManager.GetString("InvalidPID", culture),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (dialog == DialogResult.No)
                {
                    return;
                }

                chk_Overwrite.Enabled = false;
            }

            bool persisted = true;

            Transaction transaction = null;

            try
            {
                transaction = new Transaction(document, "Key Schedule imported");

                transaction.Start();

                if (purge)
                {
                    PurgeSchedule();
                }
                
                persisted = PersistData(chk_Overwrite.Enabled & chk_Overwrite.Checked);

                if (persisted)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.RollBack();
                }
            }
            catch(Exception ex)
            {
                if (null != transaction)
                {
                    transaction.RollBack();
                }

                throw ex;
            }

            if (persisted)
            {
                MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("SuccessfulImportDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("SuccessfulImport", culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }

        private string GetTiTleForm()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format(
                "{0} ({1}.{2}.{3}.{4})", 
                ImportKeyScheduleResource.ResourceManager.GetString("WindowsTitle", culture), 
                version.Major, version.Minor, version.Build, version.Revision);
        }

        private bool ActiveOk()
        {
            return (lst_KeySchedules.SelectedIndex >= 0 && File.Exists(txt_ExcelPath.Text));
        }

        private void LoadScheduleStructure()
        {
            schedule = viewSchedules.ElementAt(lst_KeySchedules.SelectedIndex);

            columnsSchedule = new List<string>(schedule.Definition.GetFieldCount());

            txt_ColumnsKeySchedule.Clear();

            // Get fields form Key Schedule
            for (int i = 0; i < columnsSchedule.Capacity; i++)
            {
                columnsSchedule.Add(schedule.Definition.GetField(i).GetName());

                txt_ColumnsKeySchedule.Text += string.IsNullOrEmpty(txt_ColumnsKeySchedule.Text) ? "" : ",";
                txt_ColumnsKeySchedule.Text += columnsSchedule.Last();
            }
        }

        private void LoadExcelStructure()
        {
            SLDocument sl = null;

            try
            {
                sl = new SLDocument(txt_ExcelPath.Text);
            }
            catch (Exception ex)
            {
                txt_ExcelPath.Clear();
                txt_ColumnsExcel.Clear();

                MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("ExceptionExcelFileDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("ExceptionExcelFile", culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (sl != null)
            {
                SLWorksheetStatistics stats = sl.GetWorksheetStatistics();

                columnsExcel = new List<string>();

                txt_ColumnsExcel.Clear();

                // Get fields form Excel
                for (int i = 1; i <= stats.EndColumnIndex; i++)
                {
                    string name = sl.GetCellValueAsString(1, i);

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "EMPTY";
                    }

                    columnsExcel.Add(name);

                    txt_ColumnsExcel.Text += string.IsNullOrEmpty(txt_ColumnsExcel.Text) ? "" : ",";
                    txt_ColumnsExcel.Text += columnsExcel.Last();
                }
            }
        }

        private bool MatchScheduleStructure()
        {
            // Compare number's entries
            if (columnsSchedule.Count != columnsExcel.Count)
            {
                return false;
            }

            // Compare column name by column name
            for (int i = 0; i < columnsSchedule.Count; i++)
            {
                if (columnsSchedule[i] != columnsExcel[i])
                {
                    return false;
                }
            }

            return true;
        }

        private List<List<string>> LoadExcelData()
        {
            List<List<string>> data = null;

            SLDocument sl = null;

            try
            {
                sl = new SLDocument(txt_ExcelPath.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    ImportKeyScheduleResource.ResourceManager.GetString("ExceptionExcelFileDesc", culture),
                    ImportKeyScheduleResource.ResourceManager.GetString("ExceptionExcelFile", culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }

            if (sl != null)
            {
                data = new List<List<string>>();

                SLWorksheetStatistics stats = sl.GetWorksheetStatistics();

                // Get data form Excel
                for (int i = 2; i <= stats.EndRowIndex; i++)
                {
                    List<string> row = new List<string>();

                    for (int j = 1; j <= stats.EndColumnIndex; j++)
                    {
                        string value = sl.GetCellValueAsString(i, j);

                        row.Add(value);
                    }

                    data.Add(row);
                }
            }

            return data;
        }

        private bool ValidPID()
        {
            return columnsSchedule.FindIndex(x => x == "pID") >= 0 && columnsExcel.FindIndex(x => x == "pID") >= 0;
        }

        private void PurgeSchedule()
        {
            int rowCount = schedule.GetTableData().GetSectionData(SectionType.Body).NumberOfRows;

            TableSectionData tsd = schedule.GetTableData().GetSectionData(SectionType.Body);

            using (SubTransaction transaction = new SubTransaction(document))
            {
                transaction.Start();

                for (int i = 1; i < rowCount; i++)
                {
                    tsd.RemoveRow(i);
                }

                transaction.Commit();
            }
        }

        private bool PersistData(bool overwrite = false)
        {
            // Parameters list for each entry
            Dictionary<string, List<Parameter>> parameters = new Dictionary<string, List<Parameter>>();

            IList<Element> elements = new FilteredElementCollector(document, schedule.Id).ToElements();

            bool invalid = false;

            if (overwrite)
            {
                foreach (Element element in elements)
                {
                    string pID = null;
                    List<Parameter> entry = new List<Parameter>();

                    foreach (var key in columnsSchedule)
                    {
                        entry.Add(element.LookupParameter(key));

                        if (key == "pID")
                        {
                            pID = entry.Last().AsString();
                        }
                    }

                    if (!string.IsNullOrEmpty(pID))
                    {
                        if (!parameters.ContainsKey(pID))
                        {
                            parameters.Add(pID, entry);
                        }
                        else
                        {
                            invalid = true;
                        }
                    }
                    else
                    {
                        invalid = true;
                    }
                }
            }

            if (invalid)
            {
                DialogResult dialog = MessageBox.Show(
                    string.Format("{0}\n{1}", 
                        ImportKeyScheduleResource.ResourceManager.GetString("WrongKeyScheduleDataDesc", culture),
                        ImportKeyScheduleResource.ResourceManager.GetString("WrongKeyScheduleDataDesc1", culture)),
                    ImportKeyScheduleResource.ResourceManager.GetString("WrongKeyScheduleData", culture), 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (dialog == DialogResult.No)
                {
                    return false;
                }
            }

            List<List<string>> data = LoadExcelData();

            if (data == null)
            {
                return false;
            }

            TableSectionData tsd = schedule.GetTableData().GetSectionData(SectionType.Body);

            using (SubTransaction transaction = new SubTransaction(document))
            {
                transaction.Start();

                int index = -1;

                if (overwrite)
                {
                    index = columnsExcel.FindIndex(x => x == "pID");
                }

                foreach (var entry in data)
                {
                    List<Parameter> pEntry = null;
                    bool nEntry = false;

                    if (overwrite) {
                        if (parameters.ContainsKey(entry[index]))
                        {
                            // Get entry for overwrite values
                            pEntry = parameters[entry[index]];
                        }
                    }

                    if (pEntry == null)
                    {
                        // Add new entry
                        pEntry = new List<Parameter>();

                        tsd.InsertRow(0);

                        Element newElement = new FilteredElementCollector(document, schedule.Id).Last();

                        if (newElement != null)
                        {
                            foreach (string key in columnsSchedule)
                            {
                                pEntry.Add(newElement.LookupParameter(key));
                            }
                        }

                        nEntry = true;
                    }

                    if (nEntry || overwrite)
                    {
                        for (int i = 0; i < entry.Count; i++)
                        {
                            pEntry[i].Set(entry[i]);
                        }
                    }
                }

                transaction.Commit();
            }

            return true;
        }
    }
}