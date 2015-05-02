var errorMsg = 'Something failed, please try again';
var successMsg = 'Success';
$('.mark-complete').click(function () {
    var id = $(this).attr('data-attr');
    var elToFade = $(this).closest('.req-block');
    bootbox.confirm('Are you sure ?', function (result) {
        if (result) {
            $.ajax({
                type: "GET",
                url: "/Requisition/MarkComplete",
                data: {
                    "id": id
                },
                beforeSend: function () {
                    $('.spinner').fadeIn('slow')
                },
                success: function () {
                    elToFade.fadeOut('slow');
                    $('.ajax-message').css('backgroundColor', '#2ABB9B');
                    $('.ajax-message').css('color', 'white');
                    $('.ajax-message').text(successMsg);
                    $('.ajax-message').fadeIn('slow').delay(4000).fadeOut('slow');
                },
                error: function (xhr, status, error) {
                    $('.spinner').fadeOut('slow');
                    $('.ajax-message').text(errorMsg);
                    $('.ajax-message').css('backgroundColor', '#F1A9A0');
                    $('.ajax-message').css('color', 'white');
                    $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
                },
                complete: function () {
                    $('.spinner').fadeOut('slow');
                }
            });
        }
    })
})