
var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loaddatatable("inprocess")

    }
    else {
        if (url.includes("completed")) {
            loaddatatable("completed")
        }
        else {
            if (url.includes("pending")) {
                loaddatatable("pending")
            }
            else {
                if (url.includes("approved")) {
                    loaddatatable("approved")
                }
                else {
                    loaddatatable("all")
                }
            }
        }
    }
    
})

function loaddatatable(status) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status, dataSrc: ''
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return ` <div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" asp-route-id="@item.Id" asp-action="Upsert" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                       
                    </div>`
                }, "width":"25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}




