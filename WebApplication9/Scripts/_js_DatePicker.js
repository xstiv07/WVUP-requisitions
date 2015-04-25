$(document).ready(function () {
    $(".date-field").each(function () {
        $(this).datepicker({
            dateFormat: 'mm/dd/yy',
            showStatus: true,
            showWeeks: true,
            highlightWeek: true,
            numberOfMonths: 1,
            showAnim: "scale",
            showOptions: {
                origin: ["top", "left"]
            }
        });
    })
});