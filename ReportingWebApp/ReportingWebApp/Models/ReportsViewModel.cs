using System.Collections.Generic;
using Microsoft.PowerBI.Api.V1.Models;

namespace ReportingWebApp.Models
{
    public class ReportsViewModel
    {
        public List<Report> Reports { get; set; }

        public Dictionary<string, string> ReportWorkspace { get; set; }
    }
}