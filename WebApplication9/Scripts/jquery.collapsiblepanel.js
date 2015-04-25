$(document).ready(function () {

    // hide all div containers  
    $('#collapsible-panels .check').hide();
    // append click event to the a element  
    $('#collapsible-panels .expandable-panel-heading').click(function (e) {
        // slide down the corresponding div if hidden, or slide up if shown  
        $(this).next('#collapsible-panels .check').slideToggle('slow');
        // set the current item as active  
        $(this).toggleClass('active');
        e.preventDefault();
    });
});