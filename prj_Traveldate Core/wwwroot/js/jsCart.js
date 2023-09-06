$('.uncheckbox').on('click', function () {
    $(this).hide();
    $(this).prev('.checkbox').show();
    $('.uncheckall').hide();
    $('.uncheckall').prev('.checkall').show();
    $(this).closest('.Cart-Items').removeClass("cartChecked");
    $(this).siblings('.prodcb').prop("checked", false);
    calculateTotalPrice();
});

$('.checkbox').on('click', function () {
    $(this).hide();
    $(this).next('.uncheckbox').show();
    $(this).closest('.Cart-Items').addClass("cartChecked");
    $(this).siblings('.prodcb').prop("checked", true);
    calculateTotalPrice();
});

$('.uncheckall').on('click', function () {
    $(this).hide();
    $(this).prev('.checkall').show();
    $('.Cart-Items .uncheckbox').hide();
    $('.Cart-Items .checkbox').show();
    $('.Cart-Items').removeClass("cartChecked");
    $('.Cart-Items .prodcb').prop("checked", false);
    calculateTotalPrice();
});

$('.checkall').on('click', function () {
    $(this).hide();
    $(this).next('.uncheckall').show();
    $('.Cart-Items .uncheckbox').show();
    $('.Cart-Items .checkbox').hide();
    $('.Cart-Items').addClass("cartChecked");
    $('.Cart-Items .prodcb').prop("checked", true);
    calculateTotalPrice();
});

//$('.cartLike').on('click', function () {
//    $(this).hide();
//    $(this).next('.cartUnlike').show();
//});

//$('.cartUnlike').on('click', function () {
//    $(this).hide();
//    $(this).prev('.cartLike').show();
//});

function calculateTotalPrice() {
    let totalPrice = 0;
    let count = 0;
    $('.cartChecked').find('.itemprice').each(function () {
        let price = Number($(this).text());
        totalPrice += price;
        count++;
    });
    $('#totalPrice').text(totalPrice);
    $('#earnpoint').text("可獲得" + Math.ceil(totalPrice / 100) + "點!")
    $('#prodcount').text( count + " 項旅程")
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
    $(this).closest('.Cart-Items').removeClass("cartChecked");
    $(this).closest('.Cart-Items').fadeOut();
    calculateTotalPrice();
    $(this).closest('.Cart-Items').remove();
})

$('#usepoint').on('input', function () {
/*    $(this).closest('.Confirm-Container-Content').next('.Confirm-summary').text("共可折抵 " + $('#usepoint').val() + " 元");*/
    refreshDiscount();
})

function refreshDiscount() {
    const totalD = Number($('#usepoint').val()) + Number($('#coupondisc').val());
    const checkout = Number($('#prodsum').val()) - totalD;
    $('#totalDiscount').text("共可折抵 " + totalD + " 元")
    $('#totalDsc').text(totalD);
    $('#checkuotamount').val(checkout);
    $('#checkuotamount_lbl').text("總金額 NTD " + checkout);
    $('#getpt').text("共可獲得 " + Math.ceil(checkout / 100) + " 點!");
}

$('.useCouponButton').on('click', function () {
    const Cid = $(this).prev('input').val();
    const CNameID = "#couponName_" + Cid;
    const CDiscountID = "#couponDiscount_" + Cid;
    const CID = "#couponID_" + Cid;
    const CORG = "#couponOriginal_" + Cid;


    $('#usecouponID').val($(CID).val());
    $('#usecoupon').text($(CNameID).text() + " - " + $(CDiscountID).text());


    if (Number($(CORG).val()) < 1) {
        $('#coupondisc').val(Math.ceil((Number($('#prodsum').val()) * (1 - Number($(CORG).val())))).toFixed(0));
        console.log(Number($('#prodsum').val()));
        console.log(Number($('#coupondisc').val()));
        console.log(Number($(CORG).val()));

    }
    else {
        $('#coupondisc').val(Number($(CORG).val()));
    }

    refreshDiscount();

})

$('.companion_choose').on('change', function () {
    if ($(this).is(':checked')) {
        $(this).siblings('.chooseCompanion').show();
    }
})

$('.companion_input').on('change', function () {
    if ($(this).is(':checked')) {
        $(this).parent().next('div').find('.chooseCompanion').hide();
        // 找到父級元素的下一個Confirm-Container-Content容器
        var $container = $(this).closest('.radio-group').next('.Confirm-Container-Content');

        // 在該容器中找到對應的輸入欄位並清空值
        $container.find('.lastname').val("");
        $container.find('.firstname').val("");
        $container.find('.idnumber').val("");
        $container.find('.phone').val("");
        $container.find('.bdate').val("");
        $container.find('.companionList').val("");

        $container.find('.lastname').prop("disabled", false);
        $container.find('.firstname').prop("disabled", false);
        $container.find('.idnumber').prop("disabled", false);
        $container.find('.phone').prop("disabled", false);
        $container.find('.bdate').prop("disabled", false);


    }
})

