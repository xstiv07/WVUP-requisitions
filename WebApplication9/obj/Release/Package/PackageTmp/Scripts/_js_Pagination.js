$(document).ready(function () {
    var errorMsg = 'Something failed, please try again';
    $('#myPager').on('click', 'a', function () {
        $.ajax({
            url: this.href,
            beforeSend: function () {
                $('.spinner').fadeIn('slow')
            },
            type: 'GET',
            error: function (xhr, status, error) {
                $('.spinner').fadeOut('slow');
                $('.ajax-message').text(errorMsg);
                $('.ajax-message').css('backgroundColor', '#F1A9A0');
                $('.ajax-message').css('color', 'white');
                $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
            },
            cache: false,
            success: function (result) {
                $('#hty').html(result);
                $('.spinner').fadeOut('slow')
            }
        });
        return false;
    });
});