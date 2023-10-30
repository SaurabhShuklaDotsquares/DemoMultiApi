using System.Drawing.Printing;

namespace Rotativa
{
    internal class UrlAsPdf
    {
        private string pagename;

        public UrlAsPdf(string pagename)
        {
            this.pagename = pagename;
        }

        public object PageOrientation { get; set; }
        public object PageSize { get; set; }
        public Margins PageMargins { get; set; }
    }
}