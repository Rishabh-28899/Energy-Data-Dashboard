using System.Collections;
using System.Diagnostics;
using System.Globalization;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Emergy_report.models;
using Emergy_report.Services;
using Microsoft.AspNetCore.Mvc;


namespace Emergy_report.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly Immemoryqueue _queue;

        public ExcelController(Immemoryqueue queue)
        {
            _queue = queue;
        }
        [HttpGet("test")]
        public IActionResult test()
        {
            return Ok("Running");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().Skip(1);

            int successCount = 0;
            int skippedCount = 0;

            Console.WriteLine($"Total Rows Found: {rows.Count()}");

            foreach (var row in rows)
            {
                try
                {
                    Console.WriteLine($"Processing Row: {row.RowNumber()}");

                    // ✅ DATE HANDLING (supports both formats)
                    DateTime parsedDate;
                    var cell = row.Cell(2);

                    if (cell.DataType == XLDataType.DateTime)
                    {
                        parsedDate = cell.GetDateTime();
                    }
                    else
                    {
                        var dateString = cell.GetString().Trim();

                        bool isValid =
                            DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                            || DateTime.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                            || DateTime.TryParseExact(dateString, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                             || DateTime.TryParse(dateString, out parsedDate);

                        if (!isValid)
                        {
                            Console.WriteLine($"❌ Skipped Row {row.RowNumber()} - Invalid Date: {dateString}");
                            skippedCount++;
                            continue;
                        }
                    }

                    // ✅ CREATE OBJECT SAFELY
                    var data = new Emerguapp
                    {
                        Date = parsedDate.Date,

                        Block = row.Cell(3).TryGetValue<double>(out var block) ? Convert.ToInt32(block) : 0,

                        StartTime = row.Cell(4).GetString(),
                        EndTime = row.Cell(5).GetString(),

                        FrequencyHz = row.Cell(6).TryGetValue<double>(out var freq) ? freq : 0,
                        ActualGeneration = row.Cell(7).TryGetValue<double>(out var ag) ? ag : 0,
                        DeclaredCapacity = row.Cell(8).TryGetValue<double>(out var dc) ? dc : 0,
                        ScheduledGeneration = row.Cell(9).TryGetValue<double>(out var sg) ? sg : 0,
                        AgcAdjustment = row.Cell(10).TryGetValue<double>(out var adj) ? adj : 0,
                        OverInjection = row.Cell(11).TryGetValue<double>(out var oi) ? oi : 0,
                        UnderInjection = row.Cell(12).TryGetValue<double>(out var ui) ? ui : 0,
                        TotalCharge = row.Cell(13).TryGetValue<decimal>(out var tc) ? tc : 0
                    };

                    // ✅ ADD TO QUEUE
                    _queue.Enqueue(data);
                    successCount++;

                    Console.WriteLine($"✅ Added Row {row.RowNumber()} | Queue Count: {_queue.Count}");
                }
                catch (Exception ex)
                {
                    skippedCount++;
                    Console.WriteLine($"❌ Error Row {row.RowNumber()}: {ex.Message}");
                }
            }

            // ✅ FINAL RESPONSE
            return Ok(new
            {
                totalRows = rows.Count(),
                addedToQueue = successCount,
                skipped = skippedCount,
                currentQueueCount = _queue.Count
            });
        }
    }
}


