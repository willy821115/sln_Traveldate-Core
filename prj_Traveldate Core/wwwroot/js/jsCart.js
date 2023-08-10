$('.checkbox').on('click', function () {
    $(this).hide();
    $(this).prev('.uncheckbox').show()
});