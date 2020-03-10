using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReadExcel_OpenXML_CSharp.Models;

namespace ReadExcel_OpenXML_CSharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            DataTable dt = new DataTable();

            string pathName = @"E:\NTPHONG\4. GITHUB\ReadExcel_OpenXML_Csharp\danh_sach_sinh_vien.xlsx";
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(pathName, false))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                foreach (Cell cell in rows.ElementAt(6))
                {
                    dt.Columns.Add(GetValueInCell(doc, cell));
                }

                foreach (Row row in rows)
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        dataRow[i] = GetValueInCell(doc, row.Descendants<Cell>().ElementAt(i));
                    }

                    dt.Rows.Add(dataRow);
                }
            }
            ViewData["dataExcel"] = dt;
            return View();
        }

        private string GetValueInCell(SpreadsheetDocument doc, Cell cell)
        {
            string value = string.Empty;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }
            }
            return value;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
