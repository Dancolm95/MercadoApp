using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MercadoApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IPuestoRepository _puestoRepository;
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPagoRepository _pagoRepository;

        public ReportesController(IPuestoRepository puestoRepository, IDeudaRepository deudaRepository, IPagoRepository pagoRepository)
        {
            _puestoRepository = puestoRepository;
            _deudaRepository = deudaRepository;
            _pagoRepository = pagoRepository;
        }

        public IActionResult Index()
        {
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = DateTime.Now;

            ViewBag.FechaInicio = fechaInicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy-MM-dd");

            return View();
        }

        [HttpPost]
        public IActionResult Deudas(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);

            var deudas = _deudaRepository.GetByFecha(inicio, fin);

            var model = new ReporteDeudaViewModel
            {
                Deudas = deudas,
                Cantidad = ((System.Collections.Generic.List<Deuda>)deudas).Count,
                TotalMonto = ((System.Collections.Generic.List<Deuda>)deudas).Sum(d => d.Monto)
            };

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.Titulo = "Reporte de Deudas";

            return View("ReporteDeudas", model);
        }

        [HttpPost]
        public IActionResult Pagos(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);

            var pagos = _pagoRepository.GetByFecha(inicio, fin);

            var model = new ReportePagoViewModel
            {
                Pagos = pagos,
                Cantidad = ((System.Collections.Generic.List<Pago>)pagos).Count,
                TotalIngresos = ((System.Collections.Generic.List<Pago>)pagos).Sum(p => p.MontoPagado)
            };

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.Titulo = "Reporte de Ingresos/Pagos";

            return View("ReportePagos", model);
        }

        [HttpPost]
        public IActionResult Puestos(string estado)
        {
            var puestos = _puestoRepository.GetAll();

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                puestos = ((System.Collections.Generic.List<Puesto>)puestos).FindAll(p => p.Estado == estado);
            }

            var model = new ReportePuestoViewModel
            {
                Puestos = puestos,
                Cantidad = ((System.Collections.Generic.List<Puesto>)puestos).Count
            };

            ViewBag.Estado = estado;
            ViewBag.Titulo = "Reporte de Puestos";

            return View("ReportePuestos", model);
        }
        [HttpGet]
        public IActionResult ExportarDeudasExcel(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
            var deudas = _deudaRepository.GetByFecha(inicio, fin);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Deudas");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Puesto";
                worksheet.Cell(currentRow, 3).Value = "Tipo Servicio";
                worksheet.Cell(currentRow, 4).Value = "Monto";
                worksheet.Cell(currentRow, 5).Value = "Fecha";
                worksheet.Cell(currentRow, 6).Value = "Estado";

                foreach (var deuda in deudas)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = deuda.IdDeuda;
                    worksheet.Cell(currentRow, 2).Value = deuda.NumeroPuesto;
                    worksheet.Cell(currentRow, 3).Value = deuda.TipoServicio;
                    worksheet.Cell(currentRow, 4).Value = deuda.Monto;
                    worksheet.Cell(currentRow, 5).Value = deuda.FechaGenerada.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 6).Value = deuda.Pagada ? "Pagada" : "Pendiente";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteDeudas.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult ExportarDeudasPdf(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
            var deudas = _deudaRepository.GetByFecha(inicio, fin);

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph("Reporte de Deudas", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                document.Add(new Paragraph($"Período: {fechaInicio} al {fechaFin}", fontNormal));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;

                table.AddCell(new PdfPCell(new Phrase("ID", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Puesto", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Tipo Servicio", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Monto", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Fecha", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Estado", fontTitle)));

                foreach (var deuda in deudas)
                {
                    table.AddCell(new PdfPCell(new Phrase(deuda.IdDeuda.ToString(), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(deuda.NumeroPuesto.ToString(), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(deuda.TipoServicio, fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(deuda.Monto.ToString("C"), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(deuda.FechaGenerada.ToString("dd/MM/yyyy"), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(deuda.Pagada ? "Pagada" : "Pendiente", fontNormal)));
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "ReporteDeudas.pdf");
            }
        }

        [HttpGet]
        public IActionResult ExportarPagosExcel(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
            var pagos = _pagoRepository.GetByFecha(inicio, fin);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Pagos");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Deuda";
                worksheet.Cell(currentRow, 3).Value = "Persona";
                worksheet.Cell(currentRow, 4).Value = "Monto";
                worksheet.Cell(currentRow, 5).Value = "Fecha";
                worksheet.Cell(currentRow, 6).Value = "Referencia";

                foreach (var pago in pagos)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = pago.IdPago;
                    worksheet.Cell(currentRow, 2).Value = pago.DetallesDeuda;
                    worksheet.Cell(currentRow, 3).Value = pago.NombrePersona;
                    worksheet.Cell(currentRow, 4).Value = pago.MontoPagado;
                    worksheet.Cell(currentRow, 5).Value = pago.FechaPago.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 6).Value = pago.Referencia;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportePagos.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult ExportarPagosPdf(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);
            var pagos = _pagoRepository.GetByFecha(inicio, fin);

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph("Reporte de Pagos", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                document.Add(new Paragraph($"Período: {fechaInicio} al {fechaFin}", fontNormal));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;

                table.AddCell(new PdfPCell(new Phrase("ID", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Deuda", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Persona", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Monto", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Fecha", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Referencia", fontTitle)));

                foreach (var pago in pagos)
                {
                    table.AddCell(new PdfPCell(new Phrase(pago.IdPago.ToString(), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(pago.DetallesDeuda ?? "", fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(pago.NombrePersona ?? "", fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(pago.MontoPagado.ToString("C"), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(pago.FechaPago.ToString("dd/MM/yyyy"), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(pago.Referencia ?? "", fontNormal)));
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "ReportePagos.pdf");
            }
        }

        [HttpGet]
        public IActionResult ExportarPuestosExcel(string estado)
        {
            var puestos = _puestoRepository.GetAll();

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                puestos = ((System.Collections.Generic.List<Puesto>)puestos).FindAll(p => p.Estado == estado);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Puestos");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Número";
                worksheet.Cell(currentRow, 2).Value = "Sección";
                worksheet.Cell(currentRow, 3).Value = "Área (M2)";
                worksheet.Cell(currentRow, 4).Value = "Estado";
                worksheet.Cell(currentRow, 5).Value = "Monto Mensual";

                foreach (var puesto in puestos)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = puesto.NumeroPuesto;
                    worksheet.Cell(currentRow, 2).Value = puesto.Seccion;
                    worksheet.Cell(currentRow, 3).Value = puesto.AreaM2;
                    worksheet.Cell(currentRow, 4).Value = puesto.Estado;
                    worksheet.Cell(currentRow, 5).Value = puesto.MontoMensual;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportePuestos.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult ExportarPuestosPdf(string estado)
        {
            var puestos = _puestoRepository.GetAll();

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                puestos = ((System.Collections.Generic.List<Puesto>)puestos).FindAll(p => p.Estado == estado);
            }

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph("Reporte de Puestos", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                document.Add(new Paragraph($"Filtro: {(string.IsNullOrEmpty(estado) ? "Todos" : estado)}", fontNormal));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                table.AddCell(new PdfPCell(new Phrase("Número", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Sección", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Área (M2)", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Estado", fontTitle)));
                table.AddCell(new PdfPCell(new Phrase("Monto Mensual", fontTitle)));

                foreach (var puesto in puestos)
                {
                    table.AddCell(new PdfPCell(new Phrase(puesto.NumeroPuesto, fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(puesto.Seccion, fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(puesto.AreaM2.ToString(), fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(puesto.Estado, fontNormal)));
                    table.AddCell(new PdfPCell(new Phrase(puesto.MontoMensual.ToString("C"), fontNormal)));
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "ReportePuestos.pdf");
            }
        }
    }
}