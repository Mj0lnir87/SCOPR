using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using System.Xml.Linq;

namespace SCOPR.Infrastructure.Reporting;

public class QuestPdfReportGenerator: IReportGenerator
{
    public Task<byte[]> GenerateCountrySummaryReportAsync(List<CountrySummary> summaries)
    {
        // Use QuestPDF to create a document
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.Header().Text("Landenrapport").SemiBold().FontSize(24);
                page.Content().Element(x => RenderContent(x, summaries));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Pagina ");
                    x.CurrentPageNumber();
                    x.Span(" van ");
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
            column.Item().Text("Landensamenvatting").FontSize(18);
            column.Item().Text($"Periode: {summaries.First().StartDate:d} - {summaries.First().EndDate:d}").FontSize(12);
            column.Item().PaddingTop(10);

            column.Item().Table(table =>
            {
                // Definieer kolommen
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(120);
                    cols.ConstantColumn(80);
                    cols.ConstantColumn(120);
                    cols.ConstantColumn(100);
                    cols.RelativeColumn();
                });

                // Header rij
                table.Header(header =>
                {
                    header.Cell().Text("Land").SemiBold();
                    header.Cell().Text("Tel. Code").SemiBold();
                    header.Cell().Text("Hoofdstad").SemiBold();
                    header.Cell().Text("Munteenheid").SemiBold();
                    header.Cell().Text("Gem. Koers").SemiBold();
                });

                // Data rijen
                foreach (var summary in summaries)
                {
                    table.Cell().Text(summary.CountryName);
                    table.Cell().Text(summary.PhoneCode);
                    table.Cell().Text(summary.Capital);
                    table.Cell().Text(summary.Currency.Code);
                    table.Cell().Text($"€1 = {summary.AverageExchangeRate:F4}");
                }
            });
        });
    }
}