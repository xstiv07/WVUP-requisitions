var errorMsg = 'Something failed, please try again';
$('.requisition-void').click(function () {
    console.log('in here')
    var id = $(this).attr('data-attr');
    bootbox.confirm('Are you sure?', function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "/Requisition/Void",
                data: {
                    "id": id
                },
                beforeSend: function () {
                    $('.spinner').fadeIn('slow')
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
                    $('#status_' + id).text('Void');
                    $("#req-void_" + id).css('display', 'none');
                    $('#prog_' + id).attr('class', 'void');
                }
            });
        }
    })
})