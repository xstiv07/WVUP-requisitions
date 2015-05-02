$(document).ready(function () {
    var errorMsg = 'Something failed, please try again';
    $("#division").change(function () {
        $("#department").empty();
        $("#account").empty();
        if ($("#division").val() != "") {
            $.ajax({
                type: "POST",
                url: '/Requisition/GetActiveDepartments',
                dataType: 'json',
                beforeSend: function () {
                    $('.spinner').fadeIn('slow')
                },
                data: {
                    id: $("#division").val()
                },
                success: function (departments) {
                    $("#department").append('<option value="">---Please Select</option>');
                    $.each(departments, function (i, department) {
                        $("#department")
                            .append('<option value="' + department.Value + '">' + department.Text + '</option>')
                    });
                },
                error: function (xhr, status, error) {
                    $('.spinner').fadeOut('slow')
                    $('.ajax-message').text(errorMsg);
                    $('.ajax-message').css('backgroundColor', '#F1A9A0');
                    $('.ajax-message').css('color', 'white');
                    $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
                },
                complete: function () {
                    $('.spinner').fadeOut('slow')
                }
            });
        }
        return false;
    });

    $("#department").change(function () {
        $("#account").empty();
        if ($("#department").val() != "") {
            $.ajax({
                type: "POST",
                url: '/Requisition/GetActiveAccounts',
                dataType: 'json',
                beforeSend: function () {
                    $('.spinner').fadeIn('slow')
                },
                data: {
                    id: $("#department").val()
                },
                success: function (accounts) {
                    $("#account").append('<option value="">---Please Select</option>');
                    $.each(accounts, function (i, account) {
                        $("#account")
                            .append('<option value="' + account.Value + '">' + account.Text + '</option>')
                    });
                },
                error: function (xhr, status, error) {
                    $('.spinner').fadeOut('slow')
                    $('.ajax-message').text(errorMsg);
                    $('.ajax-message').css('backgroundColor', '#F1A9A0');
                    $('.ajax-message').css('color', 'white');
                    $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
                },
                complete: function () {
                    $('.spinner').fadeOut('slow')
                }
            });
        }

        return false;
    });
});