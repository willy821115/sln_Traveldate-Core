﻿$('.uncheckbox').on('click', function () {
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

function calculateTotalPrice() {
    let totalPrice = 0;
    $('.itemprice').each(function () {
        let price = Number($(this).text());
        totalPrice += price;
    });
    $('#totalPrice').text(totalPrice);
    $('#earnpoint').text("可獲得" + Math.ceil(totalPrice / 100) + "點!")
}

// 初始化頁面時計算總價
calculateTotalPrice();

// 點擊 .cartMinus 時
$('.cartMinus').click(function () {
    var $itemPrice = $(this).closest('.counter').next('.prices').find('.itemprice');
    var quantity = Number($(this).next('.count').text());
    var unitPrice = Number($itemPrice.text()) / quantity;

    if (quantity > 1) {
        quantity--;
        $(this).next('.count').text(quantity);
        $itemPrice.text(unitPrice * quantity);
    }
    calculateTotalPrice();
});

// 點擊 .cartPlus 時
$('.cartPlus').click(function () {
    var $itemPrice = $(this).closest('.counter').next('.prices').find('.itemprice');
    var quantity = Number($(this).prev('.count').text());
    var unitPrice = Number($itemPrice.text()) / quantity;

    quantity++;
    $(this).prev('.count').text(quantity);
    $itemPrice.text(unitPrice * quantity);
    calculateTotalPrice();
});

$('.cartDele').on('click', function () {
    $(this).closest('.Cart-Items').fadeOut();
    calculateTotalPrice();
})



