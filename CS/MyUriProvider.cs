// Developer Express Code Central Example:
// Export to HTML - How to export only <BODY> tag content and specify the location of CSS styles
// 
// This example illustrates how you can use the HtmlExporter class to export the
// document in HTML format. The Export method enables you to emit only content
// encompassed by the specified root tag. The HtmlDocumentExporterOptions object
// passed as the method's parameter specifies the location of CSS styles - they can
// be exported to external file, located within the style tag or embedded as inline
// styles in the corresponding HTML tags.
// The code uses a custom UriProvider to
// accomplish this task. It is instantiated and registered via the RegisterProvider
// method of RichEditControl's IUriProviderService service.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1726

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;


namespace ExportOnlyBodyContent {
    public class MyUriProvider : IUriProvider {
        string rootDirecory;
        public MyUriProvider(string rootDirectory) {
            if(String.IsNullOrEmpty(rootDirectory))
                Exceptions.ThrowArgumentException("rootDirectory", rootDirectory);
            this.rootDirecory = rootDirectory;
        }

        public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
            string cssDir = String.Format("{0}\\{1}", this.rootDirecory, rootUri.Trim('/'));
            if(!Directory.Exists(cssDir))
                Directory.CreateDirectory(cssDir);
            string cssFileName = String.Format("{0}\\style.css", cssDir);
            File.AppendAllText(cssFileName, styleText);
            return GetRelativePath(cssFileName);
        }
        public string CreateImageUri(string rootUri, RichEditImage image, string relativeUri) {
            string imagesDir = String.Format("{0}\\{1}", this.rootDirecory, rootUri.Trim('/'));
            if(!Directory.Exists(imagesDir))
                Directory.CreateDirectory(imagesDir);
            string imageName = String.Format("{0}\\{1}.png", imagesDir, Guid.NewGuid());
            image.NativeImage.Save(imageName, ImageFormat.Png);
            return GetRelativePath(imageName);
        }
        string GetRelativePath(string path) {
            string substring = path.Substring(this.rootDirecory.Length);
            return substring.Replace("\\", "/").Trim('/');
        }
    }
}
