$(document).ready(function () {
    loadDataTable()
});

function loadDataTable() {
    $('#tblData').DataTable({
        ajax: { url: '/admin/product/getall'},
        columns: [
            { data: 'name', width: '15%' },
            { data: 'description', width: '15%' },
            { data: 'brand', width: '15%' },
            { data: 'listPrice', width: '15%' },
            { data: 'category.name', width: '15%' },
        ]
    });
}