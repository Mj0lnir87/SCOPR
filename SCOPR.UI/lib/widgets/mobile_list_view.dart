import 'package:flutter/material.dart';
import '../data/models/country_summary.dart'; // Import the model

class MobileDataList extends StatelessWidget {
  final List<CountrySummary> data; // Changed type
  final List<bool> expandedItems;
  final List<bool> expandedPhoneCodes;
  final List<bool> selectedRows;
  final bool? isAllSelected;
  final Function(int) onExpansionChanged;
  final Function(int) onPhoneCodeExpansionChanged;
  final Function(int, bool) onSelectionChanged;
  final Function(bool?) onSelectAllChanged;
  final String Function(int, List<String>?)
  getDisplayPhoneCode; // Accept nullable String?

  const MobileDataList({
    super.key,
    required this.data,
    required this.expandedItems,
    required this.expandedPhoneCodes,
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
    // Safety check
    if (data.isEmpty ||
        expandedItems.length != data.length ||
        selectedRows.length != data.length ||
        expandedPhoneCodes.length != data.length) {
      return const Center(child: Text("Loading data or state mismatch..."));
    }

    return Column(
      children: [
        // Header Row
        Container(
          color: Colors.grey.shade100,
          padding: const EdgeInsets.only(right: 16.0),
          child: ListTile(
            dense: true,
            leading: Checkbox(
              value: isAllSelected,
              tristate: true,
              onChanged: onSelectAllChanged,
            ),
            title: Row(
              children: [
                Expanded(
                  flex: 2,
                  child: Text(
                    'Country',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
                Expanded(
                  flex: 2,
                  child: Text(
                    'Avg. Pop.',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
                Expanded(
                  flex: 2,
                  child: Text(
                    'Exch. Rate',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
                SizedBox(width: 60),
              ],
            ),
          ),
        ),
        // Data List
        Expanded(
          child: ListView.builder(
            itemCount: data.length,
            itemBuilder: (context, index) {
              final item = data[index]; // Now a CountrySummary object
              final phoneCodesList =
                  item.phoneCodes; // Assuming phoneCodes is a single string
              final phoneCodeDisplayString = phoneCodesList?.join(', ') ?? '';
              bool isLongPhoneCode = phoneCodeDisplayString.length > 50;

              return Card(
                margin: const EdgeInsets.symmetric(
                  horizontal: 4.0,
                  vertical: 4.0,
                ),
                child: Column(
                  children: [
                    ListTile(
                      leading: Checkbox(
                        value: selectedRows[index],
                        onChanged:
                            (bool? value) =>
                                onSelectionChanged(index, value ?? false),
                      ),
                      title: Row(
                        children: [
                          Expanded(
                            flex: 2,
                            child: Text(
                              item.countryName ?? 'N/A',
                              style: TextStyle(color: Colors.blue),
                            ),
                          ), // Use property
                          Expanded(
                            flex: 2,
                            child: Text(item.formattedPopulation),
                          ), // Use helper
                          Expanded(
                            flex: 2,
                            child: Text(item.formattedExchangeRate),
                          ), // Use helper
                        ],
                      ),
                      onTap: () => onExpansionChanged(index),
                      trailing: IconButton(
                        icon: Icon(
                          expandedItems[index]
                              ? Icons.expand_less
                              : Icons.expand_more,
                        ),
                        onPressed: () => onExpansionChanged(index),
                      ),
                    ),
                    if (expandedItems[index])
                      Padding(
                        padding: const EdgeInsets.only(
                          left: 16.0,
                          right: 16.0,
                          bottom: 16.0,
                        ),
                        // Wrap the Column in a Container to force full width
                        child: Container(
                          width:
                              double
                                  .infinity, // Force the container (and thus Column) to expand horizontally
                          child: Column(
                            crossAxisAlignment:
                                CrossAxisAlignment
                                    .start, // Align children to the start (left)
                            children: [
                              _buildDetailRow(
                                'Code',
                                item.countryCode ?? 'N/A',
                              ), // Use property
                              _buildDetailRow(
                                'Capital',
                                item.capital ?? 'N/A',
                              ), // Use property
                              _buildDetailRow(
                                'Currency',
                                item.currency?.code ?? 'N/A',
                              ), // Use property
                              const SizedBox(height: 4),
                              Text.rich(
                                TextSpan(
                                  children: [
                                    TextSpan(
                                      text: 'Phone Code: ',
                                      style: TextStyle(
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                    // Use getDisplayPhoneCode for consistent logic
                                    TextSpan(
                                      text: getDisplayPhoneCode(
                                        index,
                                        phoneCodesList,
                                      ),
                                    ),
                                  ],
                                ),
                                textAlign:
                                    TextAlign
                                        .start, // Ensure text within TextSpan aligns left
                              ),
                              if (isLongPhoneCode)
                                // Align widget is okay here as it aligns the button within the full-width column
                                Align(
                                  alignment: Alignment.centerLeft,
                                  child: TextButton(
                                    style: TextButton.styleFrom(
                                      padding: EdgeInsets.zero,
                                      minimumSize: Size(50, 20),
                                      tapTargetSize:
                                          MaterialTapTargetSize.shrinkWrap,
                                    ),
                                    // Use specific handler for phone code expansion
                                    onPressed:
                                        () =>
                                            onPhoneCodeExpansionChanged(index),
                                    child: Text(
                                      expandedPhoneCodes[index]
                                          ? 'Show less'
                                          : 'Show more',
                                      style: TextStyle(color: Colors.blue),
                                    ),
                                  ),
                                ),
                            ],
                          ),
                        ),
                      ),
                  ],
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2.0),
      child: Text.rich(
        TextSpan(
          children: [
            TextSpan(
              text: '$label: ',
              style: TextStyle(fontWeight: FontWeight.bold),
            ),
            TextSpan(text: value),
          ],
        ),
      ),
    );
  }
}
