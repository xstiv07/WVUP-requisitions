function processReview(url) {
    var errorMsg = 'Something failed, please try again';
    var successMsg = 'Success';
    var minExplanationLength = 10;

    $('.approve').click(function (e) {
        var decision = $(this).attr('id');
        var id = $(this).attr('data-attr');
        var explanation = $('#text' + id).val();

        var elToFade = $(this).closest('.review-block');
        SendRequest(id, decision, explanation, elToFade);
    })

    $('.reject').click(function (e) {
        var decision = $(this).attr('id');
        var id = $(this).attr('data-attr');
        var explanation = $('#text' + id).val();

        var elToFade = $(this).closest('.review-block');
        console.log(elToFade)
        SendRequest(id, decision, explanation, elToFade);
    })

    function SendRequest(id, decision, expl, elToFade) {
        if (expl.length >= minExplanationLength) {
            $.ajax({
                type: "POST",
                url: url,
                data: {
                    "id": id,
                    "decision": decision,
                    "explanation": expl
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
                    $('.spinner').fadeOut('slow')
                }
            });
        } else {
            $('#text' + id).css('borderColor', '#F1A9A0');
        }
    }
};