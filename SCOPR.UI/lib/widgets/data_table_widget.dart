import 'package:flutter/material.dart';
import '../data/models/country_summary.dart'; // Import the model

class ExpandableDataTable extends StatelessWidget {
  final List<CountrySummary> data; // Changed type
  final List<bool> expandedItems;
  final List<bool> selectedRows;
  final bool? isAllSelected; // Changed to nullable bool?
  final Function(int) onExpansionChanged;
  final Function(int) onPhoneCodeExpansionChanged;
  final Function(int, bool) onSelectionChanged;
  final Function(bool?) onSelectAllChanged;
  final String Function(int, List<String>?)
  getDisplayPhoneCode; // Accept nullable String?

  const ExpandableDataTable({
    super.key,
    required this.data,
    required this.expandedItems,
    required this.selectedRows,
    required this.isAllSelected,
    required this.onExpansionChanged,
    required this.onPhoneCodeExpansionChanged,
    required this.onSelectionChanged,
    required this.onSelectAllChanged,
    required this.getDisplayPhoneCode,
  });

  @override
  Widget build(BuildContext context) {
    // Ensure state lists match data length (safety check)
    if (data.isEmpty ||
        expandedItems.length != data.length ||
        selectedRows.length != data.length) {
      // Handle case where data is empty or state lists are mismatched
      // This might happen briefly during loading or if an error occurs
      return const Center(child: Text("Loading data or state mismatch..."));
    }

    return SingleChildScrollView(
      child: Table(
        // ... (border, columnWidths, defaultVerticalAlignment remain the same) ...
        border: TableBorder(
          horizontalInside: BorderSide(color: Colors.grey.shade300),
          bottom: BorderSide(color: Colors.grey.shade300),
        ),
        columnWidths: const {
          0: FixedColumnWidth(60),
          1: FlexColumnWidth(1.5),
          2: FlexColumnWidth(0.8),
          3: FlexColumnWidth(2.5),
          4: FlexColumnWidth(1.5),
          5: FlexColumnWidth(1),
          6: FlexColumnWidth(1),
          7: FlexColumnWidth(1.2),
        },
        defaultVerticalAlignment: TableCellVerticalAlignment.middle,
        children: [
          _buildHeaderRow(),
          for (int i = 0; i < data.length; i++)
            _buildTableRow(context, i, data[i]), // Pass the whole object
        ],
      ),
    );
  }

  TableRow _buildHeaderRow() {
    return TableRow(
      decoration: BoxDecoration(color: Colors.grey.shade100),
      children: [
        TableCell(
          verticalAlignment: TableCellVerticalAlignment.middle,
          child: Checkbox(
            value: isAllSelected,
            tristate: true,
            onChanged: onSelectAllChanged,
          ),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text('Country', style: TextStyle(fontWeight: FontWeight.bold)),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text('Code', style: TextStyle(fontWeight: FontWeight.bold)),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text(
            'Phone Code',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text('Capital', style: TextStyle(fontWeight: FontWeight.bold)),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text(
            'Avg. Population',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text(
            'Currency',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
        ),
        Padding(
          padding: EdgeInsets.all(12),
          child: Text(
            'Exch. Rate',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
        ),
      ],
    );
  }

  TableRow _buildTableRow(
    BuildContext context,
    int index,
    CountrySummary item,
  ) {
    // Accept CountrySummary
    final phoneCodesList =
        item.phoneCodes; // Assuming phoneCodes is a single string
    final phoneCodeDisplayString = phoneCodesList?.join(', ') ?? '';
    bool isLongPhoneCode = phoneCodeDisplayString.length > 50;

    // Determine if the phone code section for *this row* is expanded
    // Assuming expandedItems now controls phone code expansion
    bool isPhoneCodeExpanded = expandedItems[index];

    return TableRow(
      decoration: BoxDecoration(
        color: expandedItems[index] ? Colors.blue.withOpacity(0.05) : null,
      ),
      children: [
        TableCell(
          verticalAlignment: TableCellVerticalAlignment.top,
          child: Checkbox(
            value: selectedRows[index],
            onChanged:
                (bool? value) => onSelectionChanged(index, value ?? false),
          ),
        ),
        TableCell(
          child: Material(
            color: Colors.transparent,
            child: InkWell(
              // Only allow expansion if phone code is long
              onTap: isLongPhoneCode ? () => onExpansionChanged(index) : null,
              child: Padding(
                padding: EdgeInsets.all(12),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      item.countryName ?? 'N/A',
                      style: TextStyle(color: Colors.blue),
                    ), // Use item property
                    const SizedBox(width: 4),
                    if (isLongPhoneCode)
                      Icon(
                        expandedItems[index]
                            ? Icons.expand_less
                            : Icons.expand_more,
                        size: 16,
                        color: Colors.blue,
                      ),
                  ],
                ),
              ),
            ),
          ),
        ),
        TableCell(
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Text(item.countryCode ?? 'N/A'),
          ),
        ), // Use item property
        TableCell(
          verticalAlignment:
              TableCellVerticalAlignment.top, // Align content top
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min, // Take minimum vertical space
              children: [
                Text(
                  getDisplayPhoneCode(index, phoneCodesList),
                  softWrap: true,
                  // Use the dedicated state for maxLines
                  maxLines: isPhoneCodeExpanded && isLongPhoneCode ? 10 : 1,
                  overflow: TextOverflow.ellipsis,
                ),
                if (isLongPhoneCode)
                  Padding(
                    // Add padding to separate button slightly
                    padding: const EdgeInsets.only(top: 4.0),
                    child: TextButton(
                      // Use the new specific callback
                      onPressed:
                          () => onPhoneCodeExpansionChanged(
                            index,
                          ), // <-- CHANGE THIS
                      style: TextButton.styleFrom(
                        padding: EdgeInsets.zero,
                        minimumSize: Size(50, 20),
                        tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        alignment:
                            Alignment.centerLeft, // Align text within button
                      ),
                      child: Text(
                        // Use the dedicated state for button text
                        isPhoneCodeExpanded ? 'Show less' : 'Show more',
                        style: TextStyle(color: Colors.blue, fontSize: 12),
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
        TableCell(
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Text(item.capital ?? 'N/A'),
          ),
        ), // Use item property
        TableCell(
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Text(item.formattedPopulation),
          ),
        ), // Use formatted helper
        TableCell(
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Text(item.currency?.code ?? 'N/A'),
          ),
        ), // Use item property
        TableCell(
          child: Padding(
            padding: EdgeInsets.all(12),
            child: Text(item.formattedExchangeRate),
          ),
        ), // Use formatted helper
      ],
    );
  }
}
