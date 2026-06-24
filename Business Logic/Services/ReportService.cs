using Data_Logic.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Business_Logic.Services
{
    public class ReportService
    {
        public void ExportDetailsToPdf(string filePath, IEnumerable<Detail> details)
        {
          
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);

            try
            {
               
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts", "arial.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

               
                BaseColor blackColor = new BaseColor(0, 0, 0);       
                BaseColor whiteColor = new BaseColor(255, 255, 255); 

                Font titleFont = new Font(baseFont, 16, Font.BOLD, blackColor);
                Font headerFont = new Font(baseFont, 10, Font.BOLD, whiteColor);
                Font textFont = new Font(baseFont, 10, Font.NORMAL, blackColor);

                Paragraph title = new Paragraph("ОТЧЕТ ПОИЗВОДСТВА ДЕТАЛЕЙ", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

             
                Paragraph dateInfo = new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n", textFont);
                dateInfo.Alignment = Element.ALIGN_RIGHT;
                document.Add(dateInfo);

              
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100; 

               
                float[] widths = new float[] { 8f, 22f, 12f, 12f, 14f, 14f, 22f, 8f };
                table.SetWidths(widths);

              
                string[] headers = { "ID", "Название детали", "Объём", "Норма (ч)", "Дата изг.", "Участок", "Рабочий", "Смена" };

                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = new BaseColor(74, 101, 246); 
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 6;
                    table.AddCell(cell);
                }

                // 7. Заполняем таблицу данными из базы
                foreach (var detail in details)
                {
                    table.AddCell(CreateCell(detail.IdRecord.ToString(), textFont, Element.ALIGN_CENTER));
                    table.AddCell(CreateCell(detail.DetailName, textFont, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell(detail.BatchVolume.ToString(), textFont, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell(detail.TimeNorm.ToString(), textFont, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell(detail.ManufactureDate.ToString("dd.MM.yyyy"), textFont, Element.ALIGN_CENTER));
                    table.AddCell(CreateCell(detail.SectorName, textFont, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell(detail.WorkerFullName, textFont, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell(detail.ShiftNumber.ToString(), textFont, Element.ALIGN_CENTER));
                }

                
                document.Add(table);
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось сгенерировать PDF-отчет: {ex.Message}", ex);
            }
            finally
            {
               
                document.Close();
            }
        }

      
        private PdfPCell CreateCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            return cell;
        }
    }
}