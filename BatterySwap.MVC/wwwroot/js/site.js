document.addEventListener("DOMContentLoaded", () => {
    const tables = document.querySelectorAll(".js-datatable");

    tables.forEach((table) => {
        if (table.dataset.dtInitialized === "true" || typeof DataTable === "undefined") {
            return;
        }

        const exportTitle = table.dataset.exportTitle || "BatterySwap Export";
        const pageLength = Number.parseInt(table.dataset.pageLength || "10", 10);

        new DataTable(table, {
            responsive: true,
            autoWidth: false,
            pageLength,
            lengthMenu: [5, 10, 25, 50],
            order: [],
            pagingType: "simple_numbers",
            language: {
                search: "",
                searchPlaceholder: "Quick search",
                lengthMenu: "_MENU_ rows",
                info: "Showing _START_ to _END_ of _TOTAL_",
                infoEmpty: "No rows available",
                zeroRecords: "No matching records found"
            },
            layout: {
                topStart: {
                    buttons: [
                        {
                            extend: "copyHtml5",
                            text: "Copy",
                            className: "btn btn-outline-secondary btn-xs"
                        },
                        {
                            extend: "csvHtml5",
                            title: exportTitle,
                            text: "CSV",
                            className: "btn btn-outline-secondary btn-xs"
                        },
                        {
                            extend: "print",
                            title: exportTitle,
                            text: "Print",
                            className: "btn btn-outline-secondary btn-xs"
                        },
                        {
                            extend: "colvis",
                            text: "Columns",
                            className: "btn btn-outline-secondary btn-xs"
                        }
                    ]
                },
                topEnd: {
                    search: {
                        placeholder: "Quick search"
                    }
                },
                bottomStart: "info",
                bottomEnd: "paging"
            }
        });

        table.dataset.dtInitialized = "true";
    });
});
