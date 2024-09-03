const fs = require("fs");
const path = require("path");
const csv = require("csv-parse/sync");

// Function to read the CSV file
function readK6Output(filePath) {
    try {
        var data = fs.readFileSync(filePath, "utf-8");
        return csv.parse(data, { delimiter: ",", columns: true });
    } catch (error) {
        console.error("Error reading the file:", error);
        return null;
    }
}

// Function to group metrics by route
function groupMetricsByRoute(metrics) {
    const routeMetrics = {};

    metrics.forEach((metric) => {
        // Extract the route from the URL (assuming the route is the path)
        const route = new URL(metric.name).pathname;
        const type = metric.metric_name;

        if (!routeMetrics[route]) routeMetrics[route] = {};

        if (!routeMetrics[route][type]) {
            routeMetrics[route][type] = {
                totalRequests: 0,
                totalTime: 0.0,
                statuses: {}
            };
        }

        routeMetrics[route][type].totalRequests += 1;
        routeMetrics[route][type].totalTime += parseFloat(metric.metric_value);
        const status = metric.status;

        if (!routeMetrics[route][type].statuses[status]) {
            routeMetrics[route][type].statuses[status] = 0;
        }
        routeMetrics[route][type].statuses[status] += 1;
    });

    return routeMetrics;
}

// Function to print the grouped metrics
function printMetrics(routeMetrics) {
    console.log("HTTP Request Metrics Grouped by Route and Type:");
    Object.entries(routeMetrics).forEach(([route, typeMetrics]) => {
        const firstMetric = Object.entries(typeMetrics)[0][1];
        console.log(`\n ===\n\nRoute: ${route}`);
        console.log(`Total Requests: ${firstMetric.totalRequests}`);
        console.log("Status Codes:");
        Object.entries(firstMetric.statuses).forEach(([status, count]) => {
            console.log(`  ${status}: ${count}`);
        });
        console.log("Timings:");
        Object.entries(typeMetrics).forEach(([type, metrics]) => {
            const averageTime = (metrics.totalTime / metrics.totalRequests).toFixed(2);
            console.log(`${type}: \t ${averageTime} ms`);
        });
    });
}

// Main function to process the K6 output
function processK6Output(filePath) {
    const k6Data = readK6Output(filePath);
    if (k6Data) {
        const httpMetrics = k6Data.filter((x) => x.metric_name.indexOf("http_req_") == 0);
        const groupedMetrics = groupMetricsByRoute(httpMetrics);
        printMetrics(groupedMetrics);
    } else {
        console.error("Failed to process K6 output.");
    }
}

const args = process.argv.slice(2);
var filename = "../result.csv";
if (args[0] === undefined) {
    console.log(`Loading default file "${filename}". Pass a custom filename if you want to process it instead.`);
} else {
    filename = args[0];
}

// Run the program
processK6Output(filename);
