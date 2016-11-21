    window.addEventListener("popstate", function (e) {
        $.ajax({
            url: location.href,
            success: function (result) {
                $('#AjaxPaginationList').html(result);
            }
        });
    });

function ChangeUrl(page, url) {
    if (typeof (history.pushState) != "undefined") {
        var obj = { Page: page, Url: url };
        history.pushState(null, obj.Page, obj.Url);
    } else {
        alert("Browser does not support HTML5.");
    }
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

$(function() {
    $('body').on('click', '#AjaxPaginationList .pagination a', function(event) {
        event.preventDefault();
        var url = $(this).attr('href');
        $.ajax({
            url: url,
            success: function(result) {
               // ChangeUrl('index', url);
                $('#AjaxPaginationList').html(result);
            }
        });
    });
            
});