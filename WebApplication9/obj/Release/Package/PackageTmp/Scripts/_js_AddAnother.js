$(document).ready(function () {
    var errorMsg = 'Something failed, please try again';
    var $elems;

    $("#itemEditor").sortable();

    $("#addAnother").click(function () { 
        $.ajax({
            type: "GET",
            url: "/Requisition/ItemEntryRow",
            contentType: "application/json;charset=utf-8",
            beforeSend: function () {
                $('.spinner').fadeIn('slow')
            },
            error: function (xhr, status, error) {
                $('.spinner').fadeOut('slow')
                $('.ajax-message').text(errorMsg);
                $('.ajax-message').css('backgroundColor', '#F1A9A0');
                $('.ajax-message').css('color', 'white');
                $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
            },
            success: function (template) {
                $("#itemEditor").append(template);
            },
            complete: function () {
                $('.spinner').fadeOut('slow')
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
                $elems = $('#itemEditor #reqToCount');

                if ($elems.length > 0)
                    $('#reqSubmitBlock').css('display', 'block');
            }
        })
    });
});
$("#cancel").click(function () {
    $(location).attr('href', 'Index');
})

function update() {
    var sum = 0;
    $('#itemEditor > .holder1').each(function () {
        var qty = $(this).find('.qty').val();
        var price = $(this).find('.price').val();
        var fileInput = $(this).find('input[type="file"]');
        var amount = price * qty;
        sum += amount;
        $(this).find('.totalEach').text('$' + amount)

        if (amount > 3000) {
            fileInput.attr('data-val', 'true');
            fileInput.next().css('display', 'block');
        } else {
            fileInput.attr('data-val', 'false');
            fileInput.next().css('display', 'none');
        }
    });
    $('#cool').text('$' + sum);
    reparseDOM();
};

function reparseDOM() {
    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");
}

$(document).on('change paste keyup', '.price', function () {
    update();
});
$(document).on('change paste keyup', '.qty', function () {
    update();
});