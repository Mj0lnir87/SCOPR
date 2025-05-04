import 'dart:io';
import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:intl/intl.dart';
import 'package:path_provider/path_provider.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:csv/csv.dart';
import 'package:file_saver/file_saver.dart';
import '../data/models/country_summary.dart'; // Import the model

class ExportService {
  // Formatter for display
  static final _displayDateFormatter = DateFormat('yyyy-MM-dd');

  // Add startDate and endDate parameters
  static Future<void> exportToPdf(
    List<CountrySummary> allData,
    List<bool> selectedRows,
    DateTime? startDate,
    DateTime? endDate,
  ) async {
    final pdf = pw.Document();

    List<int> selectedIndices = _getSelectedIndices(
      selectedRows,
      allData.length,
    );
    final List<CountrySummary> dataToExport =
        selectedIndices.map((index) => allData[index]).toList();

    final headers = [
      'Country',
      'Code',
      'Phone Code',
      'Capital',
      'Population',
      'Currency',
      'Exchange Rate',
    ];

    // Format date range string
    final String dateRangeString =
        'Date Range: ${startDate != null ? _displayDateFormatter.format(startDate) : "N/A"} - ${endDate != null ? _displayDateFormatter.format(endDate) : "N/A"}';
    final String selectionInfo =
        selectedIndices.length == allData.length || selectedIndices.isEmpty
            ? ' (All Rows)'
            : ' (${selectedIndices.length} Rows Selected)';

    pdf.addPage(
      pw.MultiPage(
        pageFormat: PdfPageFormat.a4.landscape,
        header: (pw.Context context) {
          // Add a header for the date range
          return pw.Container(
            alignment: pw.Alignment.centerRight,
            margin: const pw.EdgeInsets.only(bottom: 10.0),
            child: pw.Text(
              dateRangeString + selectionInfo,
              style: pw.Theme.of(
                context,
              ).defaultTextStyle.copyWith(color: PdfColors.grey),
            ),
          );
        },
        build: (pw.Context context) {
          // Define a max length for potentially long strings in the PDF
          const int maxStringLength = 100; // Adjust as needed

          return [
            pw.Table.fromTextArray(
              headers: headers,
              data:
                  dataToExport.map((item) {
                    // Prepare and potentially truncate strings for PDF display
                    String phoneCodesDisplay =
                        item.phoneCodes?.join(', ') ?? '';
                    if (phoneCodesDisplay.length > maxStringLength) {
                      phoneCodesDisplay =
                          '${phoneCodesDisplay.substring(0, maxStringLength - 3)}...';
                    }

                    String countryNameDisplay = item.countryName ?? '';
                    if (countryNameDisplay.length > maxStringLength) {
                      countryNameDisplay =
                          '${countryNameDisplay.substring(0, maxStringLength - 3)}...';
                    }

                    String capitalDisplay = item.capital ?? '';
                    if (capitalDisplay.length > maxStringLength) {
                      capitalDisplay =
                          '${capitalDisplay.substring(0, maxStringLength - 3)}...';
                    }

                    // Return the potentially truncated data for the row
                    return [
                      countryNameDisplay,
                      item.countryCode ?? '',
                      phoneCodesDisplay, // Use truncated version
                      capitalDisplay,
                      item.formattedPopulation, // Assumed to be reasonably formatted
                      item.currency?.code ?? '',
                      item.formattedExchangeRate, // Assumed to be reasonably formatted
                    ];
                  }).toList(),
              // ... (styling remains the same) ...
              headerStyle: pw.TextStyle(
                fontWeight: pw.FontWeight.bold,
                fontSize: 10,
              ), // Reduced font size slightly
              cellStyle: const pw.TextStyle(
                fontSize: 9,
              ), // Reduced font size slightly
              cellAlignment: pw.Alignment.centerLeft,
              headerDecoration: const pw.BoxDecoration(
                color: PdfColors.grey300,
              ),
              border: pw.TableBorder.all(
                color: PdfColors.grey400,
                width: 0.5,
              ), // Thinner border
              columnWidths: {
                // Adjusted widths slightly, ensure they sum reasonably
                0: const pw.FlexColumnWidth(1.5), // Country
                1: const pw.FlexColumnWidth(0.7), // Code
                2: const pw.FlexColumnWidth(2.0), // Phone (reduced slightly)
                3: const pw.FlexColumnWidth(1.3), // Capital
                4: const pw.FlexColumnWidth(1.0), // Population
                5: const pw.FlexColumnWidth(0.8), // Currency
                6: const pw.FlexColumnWidth(1.2), // Exch Rate
              },
            ),
          ];
        },
      ),
    );

    await _saveOrDownloadFile(
      await pdf.save(),
      'table_data.pdf',
      'application/pdf',
    );
  }

  // Add startDate and endDate parameters
  static Future<void> exportToCsv(
    List<CountrySummary> allData,
    List<bool> selectedRows,
    DateTime? startDate,
    DateTime? endDate,
  ) async {
    List<int> selectedIndices = _getSelectedIndices(
      selectedRows,
      allData.length,
    );
    final List<CountrySummary> dataToExport =
        selectedIndices.map((index) => allData[index]).toList();

    final List<String> headers = [
      'Country',
      'Code',
      'Phone Code',
      'Capital',
      'Population',
      'Currency',
      'Exchange Rate',
    ];

    // Format date range string
    final String dateRangeString =
        'Date Range: ${startDate != null ? _displayDateFormatter.format(startDate) : "N/A"} - ${endDate != null ? _displayDateFormatter.format(endDate) : "N/A"}';
    final String selectionInfo =
        selectedIndices.length == allData.length || selectedIndices.isEmpty
            ? ' (All Rows)'
            : ' (${selectedIndices.length} Rows Selected)';

    List<List<dynamic>> csvData = [
      // Add date range as the first row
      [dateRangeString + selectionInfo],
      [], // Add an empty row for spacing
      headers, // Actual headers
      ...dataToExport.map(
        (item) => [
          item.countryName ?? '',
          item.countryCode ?? '',
          item.phoneCodes ?? '',
          item.capital ?? '',
          item.formattedPopulation,
          item.currency?.code ?? '',
          item.formattedExchangeRate,
        ],
      ),
    ];

    String csv = const ListToCsvConverter().convert(csvData);
    await _saveOrDownloadFile(
      utf8.encode(csv),
      'table_data.csv',
      'text/csv;charset=utf-8;',
    );
  }

  // ... (_getSelectedIndices and _saveOrDownloadFile remain the same) ...
  static List<int> _getSelectedIndices(
    List<bool> selectedRows,
    int totalItems,
  ) {
    List<int> selectedIndices = [];
    // Ensure selectedRows list has the correct length before iterating
    int length =
        selectedRows.length < totalItems ? selectedRows.length : totalItems;
    for (int i = 0; i < length; i++) {
      if (selectedRows[i]) {
        selectedIndices.add(i);
      }
    }
    if (selectedIndices.isEmpty && totalItems > 0) {
      // Check totalItems > 0
      return List.generate(totalItems, (index) => index);
    }
    return selectedIndices;
  }

  static Future<void> _saveOrDownloadFile(
    Uint8List bytes,
    String fileName,
    String mimeType,
  ) async {
    try {
      if (kIsWeb) {
        // Use file_saver for web
        await FileSaver.instance.saveFile(
          name: fileName,
          bytes: bytes,
          mimeType: MimeType.custom, // Use custom mime type
          customMimeType: mimeType, // Pass the specific mime type
        );
      } else {
        // Keep existing non-web logic
        final Directory? appDocDir =
            await getDownloadsDirectory(); // Use getDownloadsDirectory for better user experience
        if (appDocDir == null) {
          print('Error: Could not get downloads directory.');
          // Optionally, fallback to getApplicationDocumentsDirectory() or show an error
          final Directory docDir = await getApplicationDocumentsDirectory();
          final String filePath = '${docDir.path}/$fileName';
          final File file = File(filePath);
          await file.writeAsBytes(bytes);
          print('File saved to $filePath (fallback location)');
          return;
        }
        final String filePath = '${appDocDir.path}/$fileName';
        final File file = File(filePath);
        await file.writeAsBytes(bytes);
        print('File saved to $filePath');
        // Consider using a package like 'open_file' to open the file after saving on mobile/desktop
      }
    } catch (e) {
      print('Error saving/downloading file: $e');
    }
  }
}
