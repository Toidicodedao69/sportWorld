var dataTable;
$(document).ready(function () {
    loadDataTable()
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/category/getall'},
        columns: [
            { data: 'id', width: '15%' },
            { data: 'name', width: '40%' },
            { data: 'displayOrder', width: '10%' },
            {
                data: 'id',
                'render': function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href='/admin/category/edit?id=${data}' class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <a href='/admin/category/delete?id=${data}' class="btn btn-danger mx-2">
                            <i class="bi bi-trash3-fill"></i> Delete
                        </a>
                    </div>`
                },
                width: '25%'
            },

        ]
    });
}

