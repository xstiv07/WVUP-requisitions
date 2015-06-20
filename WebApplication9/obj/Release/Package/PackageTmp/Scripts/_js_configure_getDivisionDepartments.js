$(document).ready(function () {
    $("#division").change(function () {
        $("#department").empty();
        $.ajax({
            type: "POST",
            url: '/Configure/GetDivisionDepartments',
            dataType: 'json',
            data: {
                id: $("#division").val()
            },
            success: function (divisions) {
                $.each(divisions, function (i, division) {
                    $("#department")
                        .append('<option value="' + division.Value + '">' + division.Text + '</option>')
                });
            },
            error: function (data) {
                console.log('error');
            }
        });
        return false;
    });
});