' Developer Express Code Central Example:
' Export to HTML - How to export only <BODY> tag content and specify the location of CSS styles
' 
' This example illustrates how you can use the HtmlExporter class to export the
' document in HTML format. The Export method enables you to emit only content
' encompassed by the specified root tag. The HtmlDocumentExporterOptions object
' passed as the method's parameter specifies the location of CSS styles - they can
' be exported to external file, located within the style tag or embedded as inline
' styles in the corresponding HTML tags.
' The code uses a custom UriProvider to
' accomplish this task. It is instantiated and registered via the RegisterProvider
' method of RichEditControl's IUriProviderService service.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E1726


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports DevExpress.XtraRichEdit.Services
Imports DevExpress.XtraRichEdit.Utils
Imports DevExpress.Office.Utils


Namespace ExportOnlyBodyContent
	Public Class MyUriProvider
		Implements IUriProvider
		Private rootDirecory As String
		Public Sub New(ByVal rootDirectory As String)
			If String.IsNullOrEmpty(rootDirectory) Then
				Exceptions.ThrowArgumentException("rootDirectory", rootDirectory)
			End If
			Me.rootDirecory = rootDirectory
		End Sub

		Public Function CreateCssUri(ByVal rootUri As String, ByVal styleText As String, ByVal relativeUri As String) As String Implements IUriProvider.CreateCssUri
			Dim cssDir As String = String.Format("{0}\{1}", Me.rootDirecory, rootUri.Trim("/"c))
			If (Not Directory.Exists(cssDir)) Then
				Directory.CreateDirectory(cssDir)
			End If
			Dim cssFileName As String = String.Format("{0}\style.css", cssDir)
			File.AppendAllText(cssFileName, styleText)
			Return GetRelativePath(cssFileName)
		End Function
		Public Function CreateImageUri(ByVal rootUri As String, ByVal image As RichEditImage, ByVal relativeUri As String) As String Implements IUriProvider.CreateImageUri
			Dim imagesDir As String = String.Format("{0}\{1}", Me.rootDirecory, rootUri.Trim("/"c))
			If (Not Directory.Exists(imagesDir)) Then
				Directory.CreateDirectory(imagesDir)
			End If
			Dim imageName As String = String.Format("{0}\{1}.png", imagesDir, Guid.NewGuid())
			image.NativeImage.Save(imageName, ImageFormat.Png)
			Return GetRelativePath(imageName)
		End Function
		Private Function GetRelativePath(ByVal path As String) As String
			Dim substring As String = path.Substring(Me.rootDirecory.Length)
			Return substring.Replace("\", "/").Trim("/"c)
		End Function
	End Class
End Namespace
