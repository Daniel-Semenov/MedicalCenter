using Appointments.Application.DTO_s;
using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Constants;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Appointments.Application.Generators;

public class PdfResultGenerator : IPdfResultGenerator
{
    public byte[] Generate(AppointmentResultDto result)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(PdfGeneratorConstants.PageMargin, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(PdfGeneratorConstants.DefaultFontSize));

                page.Header()
                    .Text("Result of appointment")
                    .SemiBold()
                    .FontSize(PdfGeneratorConstants.TitleFontSize)
                    .FontColor(Colors.Blue.Medium)
                    .AlignCenter();

                page.Content()
                    .PaddingVertical(PdfGeneratorConstants.PaddingVertical, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(PdfGeneratorConstants.ColumnSpacing);
                        
                        x.Item().Text($"Patient: {result.PatientFullName}");
                        x.Item().Text($"Doctor: {result.DoctorFullName}");
                        x.Item().Text($"Date of an appointment: {result.ResultDate:dd.MM.yyyy HH:mm}");
                        
                        x.Item().LineHorizontal(PdfGeneratorConstants.LineWidth);
                        
                        x.Item().Text("Complaints:").SemiBold();
                        x.Item().Text(result.Complaints);
                        
                        x.Item().Text("Conclusions:").SemiBold();
                        x.Item().Text(result.Conclusion);
                        
                        x.Item().Text("Recommendations:").SemiBold();
                        x.Item().Text(result.Recommendations);
                        
                        x.Item().LineHorizontal(PdfGeneratorConstants.LineWidth);
                        
                        x.Item()
                            .Text($"Date of creating: {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .FontSize(PdfGeneratorConstants.SmallFontSize)
                            .FontColor(Colors.Grey.Medium);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ")
                            .FontSize(PdfGeneratorConstants.SmallFontSize)
                            .FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber()
                            .FontSize(PdfGeneratorConstants.SmallFontSize)
                            .FontColor(Colors.Grey.Medium);
                        x.Span(" of ")
                            .FontSize(PdfGeneratorConstants.SmallFontSize)
                            .FontColor(Colors.Grey.Medium);
                        x.TotalPages()
                            .FontSize(PdfGeneratorConstants.SmallFontSize)
                            .FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }
}