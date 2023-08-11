$('.uncheckbox').on('click', function () {
    $(this).hide();
    $(this).next('.checkbox').show();
});

$('.checkbox').on('click', function () {
    $(this).hide();
    $(this).prev('.uncheckbox').show();
});

$('.uncheckall').on('click', function () {
    $(this).hide();
    $(this).next('.checkall').show();
    $('.Cart-Items .uncheckbox').hide();
    $('.Cart-Items .checkbox').show();
});

$('.checkall').on('click', function () {
    $(this).hide();
    $(this).prev('.uncheckall').show();
    $('.Cart-Items .uncheckbox').show();
    $('.Cart-Items .checkbox').hide();
});

$('.cartLike').on('click', function () {
    $(this).hide();
    $(this).next('.cartUnlike').show();
});

$('.cartUnlike').on('click', function () {
    $(this).hide();
    $(this).prev('.cartLike').show();
});
$('.cartPlus').on('click', function () {
    const c = $(this).prev('.count').text();
    $(this).prev('.count').text(Number(c) + 1);
});

$('.cartMinus').on('click', function () {
    const c = Number($(this).next('.count').text());
    if (c > 1) {
        $(this).next('.count').text(c - 1);
    }
});