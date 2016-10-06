localStorage.clear();
function getOrganisation(id) {
    if (localStorage.getItem(id) === "entered") {
        $("#tree1" + id).hide();
        localStorage.setItem(id, "closed");
        return 0;
    }
    if (localStorage.getItem(id) === "closed") {
        $("#tree1" + id).show();
        localStorage.setItem(id, "entered");
        return 0;
    }
    $.ajax({
        url: '/GovernmentOrganisation/GetOrganisations?orgId=' + id,
        type: 'GET',
        success: function (resultt) {
            if (resultt.length != 0) {
                var value = $('#parentId').val()
                $("#" + id).prepend("<i class='indicator glyphicon glyphicon-chevron-right'></i>");
                $("#" + id).append("<ul id=tree1" + id + "></ul>");
                resultt.map(function (itemmm) {
                    $("#tree1" + id).append("<li id='" + itemmm.Id + "'>" + "<a href='#' onclick='getOrganisation(" + itemmm.Id + ")'>" + ' &nbsp; &nbsp;' + itemmm.name + "</a>&nbsp;" +
                                        "&nbsp;<a href='/GovernmentOrganisation/OrganisationInfo?redirect=" + value + "&Id=" + itemmm.Id + "' class='badge bg-red'><span class='glyphicon glyphicon-info-sign'></span></a>&nbsp;" +
                                        "<a href='/GovernmentOrganisation/EditChildOrganisation?redirect=" + value + "&Id=" + itemmm.Id + "' class='badge bg-light-blue'><span class='glyphicon glyphicon-pencil'></span></a>&nbsp;" +
                                        "<a href='/GovernmentOrganisation/DeleteChildOrganisation?redirect=" + value + "&Id=" + itemmm.userId + "&parentId=" + id + "'class='badge bg-red'><span class='glyphicon glyphicon-remove'></span></a>&nbsp;" +
                                        "<a href='/GovernmentOrganisation/AddChildOrganisation?redirect=" + value + "&parentId=" + itemmm.Id + "'class='badge bg-light-blue'><span class='glyphicon glyphicon-plus-sign'></span></a></li>"
                                        + "</li>");
                });
                localStorage.setItem(id, "entered");
            }

        },
        error: function (err) {
            console.log(err);

        }
    })

}