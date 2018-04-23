using System;
using System.IO;
using System.Windows;

using Microsoft.Win32;

using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.Office.Services;

namespace ExportOnlyBodyContent {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        CssPropertiesExportType cssExportType;
        ExportRootTag htmlExportType;

        public MainWindow() {
            InitializeComponent();
            InitComboHtmlExportType();
            InitComboCssExportType();
        }

        #region Initializing
        private void InitComboHtmlExportType() {
            ListItemCollection collExportHtml = edtExportHtmlType.Items;
            collExportHtml.BeginUpdate();
            collExportHtml.Clear();
            collExportHtml.Add(ExportRootTag.Body);
            collExportHtml.Add(ExportRootTag.Html);
            collExportHtml.EndUpdate();
            edtExportHtmlType.SelectedIndex = 0;
            htmlExportType = ExportRootTag.Body;
        }
        private void InitComboCssExportType() {
            ListItemCollection collCssStyle = edtCssStyleType.Items;
            collCssStyle.BeginUpdate();
            collCssStyle.Clear();
            collCssStyle.Add(CssPropertiesExportType.Style);
            collCssStyle.Add(CssPropertiesExportType.Link);
            collCssStyle.Add(CssPropertiesExportType.Inline);
            collCssStyle.EndUpdate();
            edtCssStyleType.SelectedIndex = 0;
            cssExportType = CssPropertiesExportType.Style;
        }
        #endregion
        private void btnLoadDocument_Click(object sender, RoutedEventArgs e) {
            this.richEditControl1.LoadDocument();
        }

        private void richEditControl1_DocumentLoaded(object sender, EventArgs e) {
            try {
                string fileName = richEditControl1.Options.DocumentSaveOptions.CurrentFileName;
                if (!String.IsNullOrEmpty(fileName)) {
                    using (StreamReader reader = new StreamReader(fileName)) {
                        this.memoEdit1.Text = reader.ReadToEnd();
                    }
                }
            }
            catch {
            }
        }

        private void richEditControl1_EmptyDocumentCreated(object sender, EventArgs e) {
            this.memoEdit1.Text = String.Empty;
        }

        #region Adjusting
        private string GetFileName(string filter) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.DereferenceLinks = true;
            saveFileDialog.ValidateNames = true;
            if (saveFileDialog.ShowDialog(this) == true)
                return saveFileDialog.FileName;
            return String.Empty;
        }
        private void SaveFile(string fileName, string value) {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(value);
                }
            }
        }

        private void edtCssStyleType_SelectedIndexChanged(object sender, RoutedEventArgs e) {
            this.cssExportType = (CssPropertiesExportType)edtCssStyleType.EditValue;
        }

        private void edtExportHtmlType_SelectedIndexChanged(object sender, RoutedEventArgs e) {
            this.htmlExportType = (ExportRootTag)edtExportHtmlType.EditValue;
        }
        #endregion

        private void btnExportHtml_Click(object sender, RoutedEventArgs e) {
            string fileName = GetFileName("HyperText Markup Language Format|*.html");
            if (String.IsNullOrEmpty(fileName))
                return;
            IUriProviderService svc = (IUriProviderService)richEditControl1.GetService(typeof(IUriProviderService));
            svc.RegisterProvider(new MyUriProvider(Path.GetDirectoryName(fileName)));
            string stringHtml = String.Empty;
            ExportHtml(out stringHtml, null, fileName);
            this.memoEdit2.Text = stringHtml;
            SaveFile(fileName, stringHtml);
        }
        #region #exporting
        private void ExportHtml(out string stringHtml, HtmlExporter exporter, string fileName) {
            stringHtml = String.Empty;
            HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
            options.ExportRootTag = htmlExportType;
            options.CssPropertiesExportType = cssExportType;
            options.TargetUri = Path.GetFileNameWithoutExtension(fileName);
            exporter = new HtmlExporter(this.richEditControl1.Model, options);
            stringHtml = exporter.Export();
        }
        #endregion #exporting
    }
}
