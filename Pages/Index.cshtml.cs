using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.AcroForms;
using PdfSharpCore.Pdf.IO;

namespace PdfTest.Pages {
    public class IndexModel : PageModel {
        public IndexModel(IWebHostEnvironment environment) { _environment=environment; }

        public async void OnGet() {
            var bolTemplate = PdfReader.Open($@"{_environment.ContentRootPath}\Templates\bol-to-fillin.pdf", PdfDocumentOpenMode.Modify);
            (bolTemplate.AcroForm.Fields["bol_freight_charges"] as PdfRadioButtonField).Value=new PdfName("/Prepaid");

            if (bolTemplate.AcroForm.Elements.ContainsKey("/NeedAppearences"))
                bolTemplate.AcroForm.Elements["/NeedApperances"]=new PdfBoolean(true);
            else
                bolTemplate.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));

            using var ms = new MemoryStream();
            bolTemplate.Save(ms);
            bolTemplate.Close();

            await ms.CopyToAsync(Response.Body);
            ms.Close();
        }

        private readonly IWebHostEnvironment _environment;
    }
}
