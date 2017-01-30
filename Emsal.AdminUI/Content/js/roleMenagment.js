function SearchForProduct() {
    var actionName = $("#actionName").val();

    $.ajax({
        url: '/RoleManagement/Index' + '?name=' + $("#pNameInput").val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}