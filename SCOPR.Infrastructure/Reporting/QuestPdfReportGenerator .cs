using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace SCOPR.Infrastructure.Reporting;

public class QuestPdfReportGenerator: IReportGenerator
{
    public QuestPdfReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateCountrySummaryReportAsync(List<CountrySummary> summaries)
    {
        // Use QuestPDF to create a document
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(15);
                page.Header().Text("Country Report").SemiBold().FontSize(24);
                page.Content().Element(x => RenderContent(x, summaries));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private void RenderContent(IContainer container, List<CountrySummary> summaries)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Country summary").FontSize(14);
            column.Item().Text($"Period: {summaries.First().StartDate:d} - {summaries.First().EndDate:d}").FontSize(10);
            column.Item().PaddingTop(5);
            column.Item().PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Medium);

            column.Item().Table(table =>
            {
                // Column definition
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(120);
                    cols.ConstantColumn(300);
                    cols.ConstantColumn(120);
                    cols.ConstantColumn(100);
                    cols.ConstantColumn(80);
                    cols.RelativeColumn();
                });

                // Header row
                table.Header(header =>
                {
                    header.Cell().Text("Country").Bold();
                    header.Cell().Text("Phone code").Bold();
                    header.Cell().Text("Capital").Bold();
                    header.Cell().Text("Avg. Population").Bold();
                    header.Cell().Text("Currency").Bold();
                    header.Cell().Text("Avg. Rate").Bold();
                    header.Cell().ColumnSpan(6).PaddingTop(3).PaddingBottom(5).Border(2);
                });

                // Data rows
                for (int i = 0; i < summaries.Count; i++)
                {
                    var summary = summaries[i];
                    var backgroundColor = i % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                    var textColor = i % 2 == 0 ? Colors.Black : Colors.DeepPurple.Accent1;

                    table.Cell().Background(backgroundColor).Text(summary.CountryName).FontColor(textColor);
                    table.Cell().Background(backgroundColor).Text(string.Join(", ", summary.PhoneCodes)).FontColor(textColor);
                    table.Cell().Background(backgroundColor).Text(summary.Capital).FontColor(textColor);
                    table.Cell().Background(backgroundColor).Text(summary.AveragePopulation.ToString("0.0")).FontColor(textColor);
                    table.Cell().Background(backgroundColor).Text(summary.Currency.Code).FontColor(textColor);
                    table.Cell().Background(backgroundColor).Text($"€1 = {summary.Currency.Symbol}{summary.AverageExchangeRate:F4}").FontColor(textColor);
                    table.Cell().ColumnSpan(6).PaddingTop(5).Text(string.Empty);
                }
            });
        });
    }
}