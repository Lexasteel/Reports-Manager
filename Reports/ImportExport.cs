using System.IO;
using System.Diagnostics;


namespace Reports
{
    internal static class ImportExport
    {
        public static void ReportImport(string filename)
        {
            //DataContext context = new DataContext();
            //string js = File.ReadAllText(filename);
            //ReportDefinition rep = JsonConvert.DeserializeObject<ReportDefinition>(js);
            //rep.ReportDefinitionID = 0;
            //rep.TimePeriodInfo = "1";
            //rep.Header2 = "2,3";
            //foreach (HistPoint item in rep.HistPoints)
            //{
            //    item.HistPointID = 0;
            //}
            //context.ReportDefinitions.Add(rep);
            //context.SaveChanges();
        }

        public static void ReportExplorer(string path)
        {
            Process.Start("explorer.exe", @Path.GetDirectoryName(path));
        }
    }
}
