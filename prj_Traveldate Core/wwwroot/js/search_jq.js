///////////////////////////////////////////////////////收合大標簽下的小標籤/////////////////////////////////////////////////////////////
$(".category-click").on('click', function () {
    const $arrowBottomDown = $(this).find("#arrow_bottom_down");
    const $arrowTopUp = $(this).find("#arrow_top_up");
    $(this).next(".filter-block-toggle").slideToggle(); //.drop_down_list_center 是下一個同級元素
    $arrowBottomDown.toggle();
    $arrowTopUp.toggle();
});

//////////////////////////////////////////////////checkbox跟標籤button連動/////////////////////////////////////////////////////////////////
var isFirstButton = true;
var cleanAll = $('<button>', {
    text: 'X',
    value: "cleanAll",
    id: "btn-clean-all",
    class: "btn btn-outline-secondary mr-1",
    click: function () {
        $('#selected-checkboxes').empty(); // 清除全部按鈕
        isFirstButton = true; // 重設為第一次新增狀態
        $('.uncheckbox').show();
        $('.checkbox').hide();
        $('.calendar-container').find('.selected').remove();
    }
});

$('.uncheckbox').on('click', function () {
    if (isFirstButton) {
        $('#selected-checkboxes').append(cleanAll);
        isFirstButton = false;
    }
    
    var originalCheckbox= $(this).hide();
    var originalUncheckbox =$(this).next(".checkbox").show();
        var text = $(this).siblings("span").text(); // 獲取span中的文字
        var button = $('<button>', {
            text: text + ' X',
            value: text,
            class: "btn btn-outline-secondary mr-1",
            click: function () {
                $(this).remove();
                originalCheckbox.show(); // 顯示對應的 .checkbox
                originalUncheckbox.hide(); // 顯示對應的 .uncheckbox
                checkCleanAllButtonExistence();
            }
        });
    $('#selected-checkboxes').append(button); // 將按鈕追加到id為displayText的元素中
    if ($('#selected-checkboxes').children('button').length === 1) {
        $('#selected-checkboxes').empty(); // 清除全部按鈕
    }
    });

$('.checkbox').on('click', function () {
    if (isFirstButton) {
        $('#selected-checkboxes').append(cleanAll);
        isFirstButton = false;
    }
        $(this).hide();
        $(this).prev(".uncheckbox").show();
        var text = $(this).siblings("span").text(); // 獲取span中的文字
        var buttonsToRemove = $('#selected-checkboxes').find('button[value="' + text + '"]'); // 尋找需要刪除的按鈕
    buttonsToRemove.remove(); // 將按鈕從 #selected-checkboxes 刪除
    checkCleanAllButtonExistence();
    });

function checkCleanAllButtonExistence() {
    if ($('#selected-checkboxes').children('button').length === 1) {
        $('#selected-checkboxes').empty(); // 清除全部按鈕
        isFirstButton = true;
    }
}



///////////////////////////////////////////日期篩選////////////////////////////////////////////////////////////

var first_click = true;
var date_string = '';
$('#calendar_first').on('click', function () {
    if ($('#selected-checkboxes').find('.date_button').length > 0) {
        $('.date_button').remove();
    }
    if (first_click) {
        var year_first = $(this).find('#year_f').text();
        var month_first = $(this).find('#month_f').text();
        var date_first = $(this).find('.selected').text();
        date_string = year_first + '/' + month_first + '/' + date_first;
        first_click = false;
    }
    else {        
       var year_second = $(this).find('#year_f').text();
       var month_second = $(this).find('#month_f').text();
        var date_second = $(this).find('.selected').last().text();
        first_click = true;
        date_string += '~' + year_second + '/' + month_second + '/' + date_second + '  X';
        var button = $('<button>', {
            text: date_string,// 設定按鈕文字為 div 的文字內容
            class: "btn btn-outline-secondary mr-1 date_button",
            click: function () {
                $(this).remove(); // 點擊按鈕時刪除該按鈕
                $('.calendar-container').find('.selected').remove();
            }
        });
        date_string = '';
        $('#selected-checkboxes').append(button); 
    }
});


$('#calendar_second').on('click', function () {
    if ($('#selected-checkboxes').find('.date_button').length > 0) {
        $('.date_button').remove();
    }
    if (first_click) {
        var year_first = $(this).find('#year_s').text();
        var month_first = $(this).find('#month_s').text();
        var date_first = $(this).find('.selected').text();
        date_string = year_first + '/' + month_first + '/' + date_first;
        first_click = false;
    }
    else {
        var year_second = $(this).find('#year_s').text();
        var month_second = $(this).find('#month_s').text();
        var date_second = $(this).find('.selected').last().text();
        first_click = true;
        date_string += '~' + year_second + '/' + month_second + '/' + date_second + 'X';
        var button = $('<button>', {
            text: date_string,// 設定按鈕文字為 div 的文字內容
            class: "btn btn-outline-secondary mr-1 date_button",
            click: function () {
                $(this).remove(); // 點擊按鈕時刪除該按鈕
                $('.calendar-container').find('.selected').remove();
            }
        });
        date_string = '';
        $('#selected-checkboxes').append(button);
    }
});
        

///////////////////////////////////////////搜尋價格////////////////////////////////////////////////////////////
$('#btn_filter_price').on('click', function () {
    if ($('#selected-checkboxes').find('.price_button').length > 0) {
        $('.price_button').remove();
    }
    var first_price = $('.outputOne').text();
    var second_price = $('.outputTwo').text();
    var button = $('<button>', {
        text: '$  ' + first_price + '~' + '$  '+second_price+'  X',// 設定按鈕文字為 div 的文字內容
        class: "btn btn-outline-secondary mr-1 price_button",
        click: function () {
            $(this).remove(); // 點擊按鈕時刪除該按鈕
        }
    });
    $('#selected-checkboxes').append(button);
})
     









