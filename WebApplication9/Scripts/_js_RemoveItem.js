$(function () {
    var $elems;
    $(".remove-item").click(function () {
        $(this).parent().fadeOut('slow', function () {
            $(this).remove();
            update();
            $elems = $('#itemEditor #reqToCount');

            if ($elems.length == 0)
                $('#reqSubmitBlock').css('display', 'none');
        })
    });
});