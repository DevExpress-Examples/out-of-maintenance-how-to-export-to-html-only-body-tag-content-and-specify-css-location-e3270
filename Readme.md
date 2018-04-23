# How to export to HTML only <BODY> tag content and specify CSS location


<p>This example illustrates how you can use the <strong>HtmlExporter</strong> class to export the document in HTML format. The <strong>Export</strong> method enables you to emit only content encompassed by the specified root tag. The <strong>HtmlDocumentExporterOptions</strong> object passed as the method's parameter specifies the location of CSS styles - they can be exported to external file, located within the <i>style</i> tag or embedded as inline styles in the corresponding HTML tags.<br />
The code uses a custom <strong>UriProvider</strong> to accomplish this task. It is instantiated and registered via the <strong>RegisterProvider</strong> method of RichEditControl's <strong>IUriProviderService </strong>service.<br />
</p><br />


<br/>


