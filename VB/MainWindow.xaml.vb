Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports DevExpress.Xpf.Editors
Imports DevExpress.XtraRichEdit.Export
Imports DevExpress.XtraRichEdit.Export.Html
Imports DevExpress.Office.Services
Imports System.Runtime.InteropServices

Namespace ExportOnlyBodyContent

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window

        Private cssExportType As CssPropertiesExportType

        Private htmlExportType As ExportRootTag

        Public Sub New()
            Me.InitializeComponent()
            InitComboHtmlExportType()
            InitComboCssExportType()
        End Sub

'#Region "Initializing"
        Private Sub InitComboHtmlExportType()
            Dim collExportHtml As ListItemCollection = Me.edtExportHtmlType.Items
            collExportHtml.BeginUpdate()
            collExportHtml.Clear()
            collExportHtml.Add(ExportRootTag.Body)
            collExportHtml.Add(ExportRootTag.Html)
            collExportHtml.EndUpdate()
            Me.edtExportHtmlType.SelectedIndex = 0
            htmlExportType = ExportRootTag.Body
        End Sub

        Private Sub InitComboCssExportType()
            Dim collCssStyle As ListItemCollection = Me.edtCssStyleType.Items
            collCssStyle.BeginUpdate()
            collCssStyle.Clear()
            collCssStyle.Add(CssPropertiesExportType.Style)
            collCssStyle.Add(CssPropertiesExportType.Link)
            collCssStyle.Add(CssPropertiesExportType.Inline)
            collCssStyle.EndUpdate()
            Me.edtCssStyleType.SelectedIndex = 0
            cssExportType = CssPropertiesExportType.Style
        End Sub

'#End Region
        Private Sub btnLoadDocument_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.richEditControl1.LoadDocument()
        End Sub

        Private Sub richEditControl1_DocumentLoaded(ByVal sender As Object, ByVal e As EventArgs)
            Try
                Dim fileName As String = Me.richEditControl1.Options.DocumentSaveOptions.CurrentFileName
                If Not String.IsNullOrEmpty(fileName) Then
                    Using reader As StreamReader = New StreamReader(fileName)
                        Me.memoEdit1.Text = reader.ReadToEnd()
                    End Using
                End If
            Catch
            End Try
        End Sub

        Private Sub richEditControl1_EmptyDocumentCreated(ByVal sender As Object, ByVal e As EventArgs)
            Me.memoEdit1.Text = String.Empty
        End Sub

'#Region "Adjusting"
        Private Function GetFileName(ByVal filter As String) As String
            Dim saveFileDialog As SaveFileDialog = New SaveFileDialog()
            saveFileDialog.Filter = filter
            saveFileDialog.RestoreDirectory = True
            saveFileDialog.CheckFileExists = False
            saveFileDialog.CheckPathExists = True
            saveFileDialog.OverwritePrompt = True
            saveFileDialog.DereferenceLinks = True
            saveFileDialog.ValidateNames = True
            If saveFileDialog.ShowDialog(Me) = True Then Return saveFileDialog.FileName
            Return String.Empty
        End Function

        Private Sub SaveFile(ByVal fileName As String, ByVal value As String)
            Using stream As FileStream = New FileStream(fileName, FileMode.Create, FileAccess.Write)
                Using writer As StreamWriter = New StreamWriter(stream)
                    writer.Write(value)
                End Using
            End Using
        End Sub

        Private Sub edtCssStyleType_SelectedIndexChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            cssExportType = CType(Me.edtCssStyleType.EditValue, CssPropertiesExportType)
        End Sub

        Private Sub edtExportHtmlType_SelectedIndexChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            htmlExportType = CType(Me.edtExportHtmlType.EditValue, ExportRootTag)
        End Sub

'#End Region
        Private Sub btnExportHtml_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim fileName As String = GetFileName("HyperText Markup Language Format|*.html")
            If String.IsNullOrEmpty(fileName) Then Return
            Dim svc As IUriProviderService = CType(Me.richEditControl1.GetService(GetType(IUriProviderService)), IUriProviderService)
            svc.RegisterProvider(New MyUriProvider(Path.GetDirectoryName(fileName)))
            Dim stringHtml As String = String.Empty
            ExportHtml(stringHtml, Nothing, fileName)
            Me.memoEdit2.Text = stringHtml
            SaveFile(fileName, stringHtml)
        End Sub

'#Region "#exporting"
        Private Sub ExportHtml(<Out> ByRef stringHtml As String, ByVal exporter As HtmlExporter, ByVal fileName As String)
            stringHtml = String.Empty
            Dim options As HtmlDocumentExporterOptions = New HtmlDocumentExporterOptions()
            options.ExportRootTag = htmlExportType
            options.CssPropertiesExportType = cssExportType
            options.TargetUri = Path.GetFileNameWithoutExtension(fileName)
            exporter = New HtmlExporter(Me.richEditControl1.Model, options)
            stringHtml = exporter.Export()
        End Sub
'#End Region  ' #exporting
    End Class
End Namespace
