function exportToCSV(dataTable, filename = 'table.csv') {
    // Get all the table data
    const data = instance; // instance.jexcel.getData();

    // Convert the data to CSV format
    let csvContent = data.map(e => e.join(",")).join("\n");

    // Create a blob for the CSV data
    let blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });

    // Create a link element to download the CSV file
    let link = document.createElement("a");
    let url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);

    // Programmatically click the link to trigger the download
    link.click();

    // Clean up
    document.body.removeChild(link);
};


function formatNumber(value, thousandsSeparator = ',', decimalSeparator = '.') {
    value = parseFloat(value).toFixed(2);  // Convert to a fixed 2 decimal places
    const parts = value.split('.');  // Split integer and decimal parts

    // Add thousands separator to the integer part
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, thousandsSeparator);

    // Join integer part and decimal part with decimal separator
    return parts.join(decimalSeparator);
};

function unformatNumber(value, thousandsSeparator = ',', decimalSeparator = '.') {
    if (typeof value === 'string') {
        value = value.replace(new RegExp('\\' + thousandsSeparator, 'g'), '');  // Remove thousands separator
        value = value.replace(decimalSeparator, '.');  // Convert decimal separator back to dot
    }
    return parseFloat(value);  // Convert string back to number
}