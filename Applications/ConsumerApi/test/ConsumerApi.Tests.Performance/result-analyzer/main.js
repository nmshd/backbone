const fs = require("fs");
const path = require("path");

// Function to read the JSON file
function readK6Output(filePath) {
    try {
        var data = fs.readFileSync(filePath, "utf-8");
        data = "[" + data.replaceAll("\x0A", ",\n");
        data = data.slice(0, -2) + "]";
        return JSON.parse(data);
    } catch (error) {
        console.error("Error reading the file:", error);
        return null;
    }
}

// Function to group metrics by route
function groupMetricsByRoute(metrics) {
    const routeMetrics = {};

    metrics.forEach((metric) => {
        const { url, status } = metric.data.tags;

        // Extract the route from the URL (assuming the route is the path)
        const route = new URL(url).pathname;

        if (!routeMetrics[route]) {
            routeMetrics[route] = {
                totalRequests: 0,
                totalTime: 0,
                statuses: {}
            };
        }

        routeMetrics[route].totalRequests += 1;
        routeMetrics[route].totalTime += metric.data.value;

        if (!routeMetrics[route].statuses[status]) {
            routeMetrics[route].statuses[status] = 0;
        }
        routeMetrics[route].statuses[status] += 1;
    });

    return routeMetrics;
}

// Function to print the grouped metrics
function printMetrics(routeMetrics) {
    console.log("HTTP Request Metrics Grouped by Route:");
    Object.entries(routeMetrics).forEach(([route, metrics]) => {
        const averageTime = (metrics.totalTime / metrics.totalRequests).toFixed(2);
        console.log(`\nRoute: ${route}`);
        console.log(`Total Requests: ${metrics.totalRequests}`);
        console.log(`Average Duration: ${averageTime} ms`);
        console.log("Status Codes:");
        Object.entries(metrics.statuses).forEach(([status, count]) => {
            console.log(`  ${status}: ${count}`);
        });
    });
}

// Main function to process the K6 output
function processK6Output(filePath) {
    const k6Data = readK6Output(filePath);
    if (k6Data) {
        const httpMetrics = k6Data.filter((x) => x.type == "Point" && x.metric == "http_req_duration");
        const groupedMetrics = groupMetricsByRoute(httpMetrics);
        printMetrics(groupedMetrics);
    } else {
        console.error("Failed to process K6 output.");
    }
}

// Specify the path to your K6 JSON output file
const k6OutputFile = path.join(__dirname, "../results.json");

// Run the program
processK6Output(k6OutputFile);
