var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else
                    loadDataTable("all");
            }
        }
    }
    
});


var inprocess = "text-primary";
var pending = "text-primary";
var completed = "text-primary";
var approved = "text-primary";
var all = "text-primary";


function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/Order/getall?status='+status },
        "columns": [
            { data: 'id' ,"width":"8%"},
            { data: 'name', "width": "17%" },
            { data: 'phoneNumber', "width": "12%" },
            { data: 'applicationUser.email', "width": "16%" },
            { data: 'orderStatus', "width": "12%" },
            { data: 'orderTotal', "width": "12%", render: DataTable.render.number(null, null, 2, '$') },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group"><a href="/Admin/Order/Details?OrderID=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Manage</a>
                    </div>`
                }, "width": "20%"
            }
        ]
    });
}

