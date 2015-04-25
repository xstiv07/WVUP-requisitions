var successMsg = 'Success';
var errorMsg = 'Something failed, please try again';

function SetSpinner() {
    $('.spinner').fadeIn('slow');
}

function Failure() {
    $('.spinner').fadeOut('slow');
    $('.ajax-message').text(errorMsg);
    $('.ajax-message').css('backgroundColor', '#F1A9A0');
    $('.ajax-message').css('color', 'white');
    $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
}

function SetCompleted(id) {
    var currentBlock = $('#blk-' + id);
    currentBlock.fadeOut('slow');
    $('.spinner').fadeOut('slow');
    $('.ajax-message').css('backgroundColor', '#2ABB9B');
    $('.ajax-message').css('color', 'white');
    $('.ajax-message').text(successMsg);
    $('.ajax-message').fadeIn('slow').delay(4000).fadeOut('slow');
}